﻿using System.Buffers;
using System.Xml.XPath;
using Decinet.Architecture;
using Decinet.Samples;
using Decinet.Utilities;

namespace Decinet.Decoders.Wave;

internal class PcmParser : WaveParser
{
    private readonly long _rawDataStartOffset;
    private readonly long _totalRawDataBytes;
    private int _samplesSentBytes;
    private readonly FileStream _tmp;
    private readonly SineWaveProvider32 _sineWaveProvider;
    private readonly byte[] _data;

    public PcmParser(BinaryReader currentStream, WaveFormat format, long rawDataStartOffset, long totalRawDataBytes) :
        base(currentStream, format, rawDataStartOffset)
    {
        _rawDataStartOffset = rawDataStartOffset;
        _totalRawDataBytes = totalRawDataBytes;

        // Sanity checks.
        if (currentStream.BaseStream.CanSeek &&
            _rawDataStartOffset + _totalRawDataBytes != currentStream.BaseStream.Length)
        {
            throw new InvalidDataException();
        }

        AudioFormat = Format.BitsPerSample switch
        {
            16 => new AudioFormat(typeof(short), sizeof(short), (int) format.SampleRate, format.NumChannels),
            32 => new AudioFormat(typeof(float), sizeof(float), (int) format.SampleRate, format.NumChannels),
            64 => new AudioFormat(typeof(long), sizeof(long), (int) format.SampleRate, format.NumChannels),
            _ => throw new Exception("Invalid bit size.")
        };

        _tmp = File.Create($"{Guid.NewGuid():N}.pcm.raws16x");
        _sineWaveProvider = new SineWaveProvider32();
        _data = CurrentStream.ReadBytes((int) _totalRawDataBytes);
    }

    public override AudioFormat AudioFormat { get; }

    public class SineWaveProvider32
    {
        int _sample;

        public SineWaveProvider32()
        {
            Frequency = 1000;
            Amplitude = 0.25f; // let's not hurt our ears
        }

        public float Frequency { get; set; }
        public float Amplitude { get; set; }
        private int _offset;

        public int Read(short[] buffer, int sampleCount, int sampleRate, int chCount)
        {
            for (var n = 0; n < sampleCount; n++)
            {
                for (var i = 0; i < chCount; i++)
                {
                    buffer[n + i] =
                        (short) (Amplitude * Math.Sin((2 * Math.PI * _sample * Frequency) / sampleRate));
                    _sample++;
                    if (_sample >= sampleRate) _sample = 0;
                }
            }

            return sampleCount;
        }
    }

    public override unsafe bool TryGetBytes(int numSamples, out ISampleFrame sampleFrame)
    {
        var bytesPerSample = Format.BitsPerSample / 8;
        var channelCount = Format.NumChannels;
        var samplesRequestedBytes = numSamples * bytesPerSample * channelCount;
        var samplesRemainingBytes = (int) _totalRawDataBytes - _samplesSentBytes;

        samplesRequestedBytes = MathExtensions.MinVal(samplesRequestedBytes, samplesRemainingBytes);

        var targetOffset = (int) _rawDataStartOffset + _samplesSentBytes;
        
        var data = _data[targetOffset..samplesRequestedBytes];
        var sdata = new short[data.Length / 2];
        Buffer.BlockCopy(data, 0, sdata, 0, data.Length);
        
        _tmp.Write(data);

        var normalizedTotalSent = _samplesSentBytes / (float) samplesRemainingBytes;
        var totalTime = (samplesRemainingBytes / (float) bytesPerSample / Format.SampleRate);

        CurrentPosition = TimeSpan.FromSeconds(totalTime * normalizedTotalSent);
        _samplesSentBytes += samplesRequestedBytes;
        
        var smpF = ShortSampleFrame.Create(samplesRequestedBytes, channelCount, AudioFormat, CurrentPosition);

        for (var i = 0; i < sdata.Length; i++)
        {
            smpF.InterleavedSampleData[i] = sdata[i];
        }
        
        sampleFrame = smpF;
        return true;
    }
}