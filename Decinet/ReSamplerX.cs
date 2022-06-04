/**************************************************************
 * License detail will be added later
 * 
 **************************************************************/

using System;
using System.Linq;

namespace LibResample.Sharp
{
    public class ReSamplerX
    {
        /// <summary>
        ///     Number of values per 1/delta in impulse response
        /// </summary>
        internal const int Npc = 4096;

        private readonly float[] _imp;
        private readonly float[] _impD;
        private readonly float _lpScl;
        private readonly double _maxFactor;
        private readonly double _minFactor;
        private readonly int _nmult;
        private readonly int _nwing;
        private readonly float[] _x;
        private readonly int _xoff;
        private readonly int _xSize;
        private readonly float[] _y;
        private double _time;
        private int _xp; // current "now"-sample pointer for input
        private int _xread; // position to put new samples
        private int _yp;

        /// <summary>
        ///     Clone an existing resampling session. Faster than creating one from scratch.
        /// </summary>
        /// <param name="other">another instance of resampler</param>
        public ReSamplerX(ReSamplerX other)
        {
            _imp = other._imp.ToArray();
            _impD = other._impD.ToArray();
            _lpScl = other._lpScl;
            _nmult = other._nmult;
            _nwing = other._nwing;
            _minFactor = other._minFactor;
            _maxFactor = other._maxFactor;
            _xSize = other._xSize;
            _x = other._x.ToArray();
            _xp = other._xp;
            _xread = other._xread;
            _xoff = other._xoff;
            _y = other._y;
            _yp = other._yp;
            _time = other._time;
        }

        public ReSamplerX(bool highQuality, double minFactor = 0.1, double maxFactor = 1.5)
        {
            if (minFactor <= 0.0 || maxFactor <= 0.0)
            {
                throw new ArgumentException("minFactor and maxFactor must be positive");
            }
            if (maxFactor < minFactor)
            {
                throw new ArgumentException("minFactor must be less or equal to maxFactor");
            }

            _minFactor = minFactor;
            _maxFactor = maxFactor;
            _nmult = highQuality ? 35 : 11;
            _lpScl = 1.0f;
            _nwing = Npc * (_nmult - 1) / 2; // # of filter coeffs in right wing

            const double rolloff = 0.90;
            const double beta = 6;

            var imp64 = new double[_nwing];

            FilterKit.LrsLpFilter(imp64, _nwing, 0.5 * rolloff, beta, Npc);
            _imp = new float[_nwing];
            _impD = new float[_nwing];

            for (var i = 0; i < _nwing; i++)
            {
                _imp[i] = (float)imp64[i];
            }

            for (var i = 0; i < _nwing-1; i++)
            {
                _impD[i] = _imp[i + 1] - _imp[i];
            }

            _impD[_nwing - 1] = -_imp[_nwing - 1];

            var xoffMin = (int)(((_nmult + 1) / 2.0) * Math.Max(1.0, 1.0 / minFactor) + 10);
            var xoffMax = (int)(((_nmult + 1) / 2.0) * Math.Max(1.0, 1.0 / maxFactor) + 10);
            _xoff = Math.Max(xoffMin, xoffMax);

            _xSize = Math.Max(2 * _xoff + 10, 4096);
            _x = new float[_xSize + _xoff];
            _xp = _xoff;
            _xread = _xoff;

            var ySize = (int)(_xSize * maxFactor + 2.0);
            _y = new float[ySize];
            _yp = 0;

            _time = _xoff;
        }

        public int GetfilterWidth()
        {
            return _xoff;
        }

        internal bool Process(double factor, ISampleBuffers buffers, bool lastBatch)
        {
            if (factor < _minFactor || factor > _maxFactor)
            {
                throw new ArgumentException("factor" + factor + "is not between minFactor=" + _minFactor + " and maxFactor=" + _maxFactor);
            }

            int outBufferLen = buffers.GetOutputBufferLength();
            int inBufferLen = buffers.GetInputBufferLenght();

            float[] imp = _imp;
            float[] impD = _impD;
            float lpScl = _lpScl;
            int nwing = _nwing;
            bool interpFilt = false;

            int inBufferUsed = 0;
            int outSampleCount = 0;

            if ((_yp != 0) && (outBufferLen - outSampleCount) > 0)
            {
                int len = Math.Min(outBufferLen - outSampleCount, _yp);

                buffers.ConsumeOutput(_y, 0, len);

                outSampleCount += len;
                for (int i = 0; i < _yp - len; i++)
                {
                    _y[i] = _y[i + len];
                }
                _yp -= len;
            }

            if (_yp != 0)
            {
                return inBufferUsed == 0 && outSampleCount == 0;
            }

            if (factor < 1)
            {
                lpScl = (float)(lpScl * factor);
            }

            while (true)
            {
                int len = _xSize - _xread;

                if (len >= inBufferLen - inBufferUsed)
                {
                    len = inBufferLen - inBufferUsed;
                }

                buffers.ProduceInput(_x, _xread, len);

                inBufferUsed += len;
                _xread += len;

                int nx;
                if (lastBatch && (inBufferUsed == inBufferLen))
                {
                    nx = _xread - _xoff;
                    for (int i = 0; i < _xoff; i++)
                    {
                        _x[_xread + i] = 0;
                    }
                }
                else
                {
                    nx = _xread - 2 * _xoff;
                }

                if (nx <= 0)
                {
                    break;
                }

                int nout;
                if (factor >= 1)
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    nout = LrsSrcUp(_x, _y, factor, nx, nwing, lpScl, imp, impD, interpFilt);
                }
                else
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    nout = LrsSrcUd(_x, _y, factor, nx, nwing, lpScl, imp, impD, interpFilt);
                }

                _time -= nx;
                _xp += nx;

                int ncreep = (int)(_time) - _xoff;
                if (ncreep != 0)
                {
                    _time -= ncreep;
                    _xp += ncreep;
                }

                int nreuse = _xread - (_xp - _xoff);

                for (int i = 0; i < nreuse; i++)
                {
                    _x[i] = _x[i + (_xp - _xoff)];
                }

                _xread = nreuse;
                _xp = _xoff;

                _yp = nout;

                if (_yp != 0 && (outBufferLen - outSampleCount) > 0)
                {
                    len = Math.Min(outBufferLen - outSampleCount, _yp);

                    buffers.ConsumeOutput(_y, 0, len);
                    outSampleCount += len;
                    for (int i = 0; i < _yp - len; i++)
                    {
                        _y[i] = _y[i + len];
                    }
                    _yp -= len;
                }

                if (_yp != 0)
                {
                    break;
                }
            }

            return inBufferUsed == 0 && outSampleCount == 0;
        }

        internal bool Process(double factor, FloatBuffer inputbuffer, bool lastBatch, FloatBuffer outputBuffer)
        {
            return Process(factor, new SampleBuffers(inputbuffer, outputBuffer), lastBatch);
        }

        public Result Process(double factor, float[] inBuffer, int inBufferOffset, int inBufferLen, bool lastBatch,
            float[] outBuffer, int outBufferOffset, int outBufferLen)
        {
            var inputBuffer = FloatBuffer.Wrap(inBuffer, inBufferOffset, inBufferLen);
            var outputBuffer = FloatBuffer.Wrap(outBuffer, outBufferOffset, outBufferLen);
            Process(factor, inputBuffer, lastBatch, outputBuffer);

            return new Result(inputBuffer.Position, outputBuffer.Position);
        }

        private int LrsSrcUp(float[] x, float[] y, double factor, int nx, int nwing, float lpScl, float[] imp, float[] impD, bool interp)
        {
            float[] xpArray = x;
            int xpIndex;

            float[] ypArray = y;
            int ypIndex = 0;

            float v;

            double currentTime = _time;
            double dt;
            double endTime;

            dt = 1.0 / factor;

            endTime = currentTime + nx;
            while (currentTime < endTime)
            {
                double leftPhase = currentTime - Math.Floor(currentTime);
                double rightPhase = 1.0 - leftPhase;

                xpIndex = (int)currentTime;
                v = FilterKit.LrsFilterUp(imp, impD, nwing, interp, xpArray, xpIndex++, leftPhase, -1);
                v += FilterKit.LrsFilterUp(imp, impD, nwing, interp, xpArray, xpIndex, rightPhase, 1);
                v *= lpScl;

                ypArray[ypIndex++] = v;
                currentTime += dt;
            }

            _time = currentTime;

            return ypIndex;
        }

        private int LrsSrcUd(float[] x, float[] y, double factor, int nx, int nwing, float lpScl, float[] imp, float[] impD, bool interp)
        {
            float[] xpArray = x;
            int xpIndex;

            float[] ypArray = y;
            int ypIndex = 0;

            float v;

            double currentTime = _time;
            double dh;
            double dt;
            double endTime;

            dt = 1.0 / factor;

            dh = Math.Min(Npc, factor * Npc);

            endTime = currentTime + nx;
            while (currentTime < endTime)
            {
                double leftPhase = currentTime - Math.Floor(currentTime);
                double rightPhase = 1.0 - leftPhase;

                xpIndex = (int)currentTime;
                v = FilterKit.LrsFilterUd(imp, impD, nwing, interp, xpArray, xpIndex++, leftPhase, -1, dh);
                v += FilterKit.LrsFilterUd(imp, impD, nwing, interp, xpArray, xpIndex, rightPhase, 1, dh);
                v *= lpScl;

                ypArray[ypIndex++] = v;
                currentTime += dt;
            }

            _time = currentTime;
            return ypIndex;
        }

        public class Result
        {
            public Result(int inputSamplesconsumed, int outputSamplesGenerated)
            {
                InputSamplesConsumed = inputSamplesconsumed;
                OutputSamplesgenerated = outputSamplesGenerated;
            }

            public int InputSamplesConsumed { get; private set; }
            public int OutputSamplesgenerated { get; private set; }
        }

        internal class SampleBuffers : ISampleBuffers
        {
            private readonly FloatBuffer _inBuffer;
            private readonly FloatBuffer _outBuffer;
            public SampleBuffers(FloatBuffer inBuffer, FloatBuffer outBuffer)
            {
                _inBuffer = inBuffer;
                _outBuffer = outBuffer;
            }

            public int GetInputBufferLenght()
            {
                return _inBuffer.RemainLength;
            }

            public int GetOutputBufferLength()
            {
                return _outBuffer.RemainLength;
            }

            public void ProduceInput(float[] array, int offset, int length)
            {
                _inBuffer.Get(array, offset, length);
            }

            public void ConsumeOutput(float[] array, int offset, int length)
            {
                _outBuffer.Put(array, offset, length);
            }
        }
    }
}