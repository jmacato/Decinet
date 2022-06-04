using Decinet.Architecture;
using Decinet.Samples;
using NWaves.Operations;
using NWaves.Signals;

namespace Decinet;

public class NWaveResampler : IResampler
{
    private IPlaybackController _playback;
    private IDSPStack _dsp;
    private IBackend _backend;
    private readonly Resampler _resamplerC;

    public NWaveResampler()
    {
        _resamplerC = new NWaves.Operations.Resampler();
    }

    /// <inheritdoc />
    public void Receive(ISampleFrame data)
    {
        if (data is not FloatSampleFrame frame)
        {
            return;
        }

        var ratio =  (float) frame.AudioFormat.SampleRate /  (float)_backend.DesiredAudioFormat.SampleRate;

        if (_backend.DesiredAudioFormat.SampleRate == frame.AudioFormat.SampleRate)
        {
            _dsp.Receive(data);
            return;
        }
        
        var signals = new float[frame.ChannelCount][];

        for (var i = 0; i < signals.Length; i++)
        {
            signals[i] = new float[frame.SampleCount];
        }

        var interleaveIndex = 0;

        for (var i = 0; i < frame.SampleCount; i++)
        {
            for (var j = 0; j < frame.ChannelCount; j++)
            {
                signals[j][i] = frame.InterleavedSampleData[interleaveIndex++];
            }
        }

        var ratiodSampleCount = (int) Math.Round(ratio * frame.SampleCount);
        
        var output = new float[frame.ChannelCount][];

        for (var i = 0; i < signals.Length; i++)
        {
            output[i] = new float[ratiodSampleCount];
        }


        int prevSampleCount = 0;
        for (var j = 0; j < frame.ChannelCount; j++)
        {

            var re = _resamplerC.Resample(new DiscreteSignal(frame.AudioFormat.SampleRate, signals[j]),
                _backend.DesiredAudioFormat.SampleRate);

            output[j] = re.Samples;

            if (prevSampleCount != output[j].Length && prevSampleCount != 0)
            {
                
            }
            else
            {
                prevSampleCount = output[j].Length;
            }
        }
        
        interleaveIndex = 0;
        
        var nF = FloatSampleFrame.Create(output[0].Length, _backend.DesiredAudioFormat.ChannelCount,
            _backend.DesiredAudioFormat);
        
        for (var i = 0; i < nF.SampleCount; i++)
        {
            for (var j = 0; j < nF.ChannelCount; j++)
            {
                nF.InterleavedSampleData[interleaveIndex++] =
                    output[j][i];
            }
        }
        
        frame.Dispose();
        _dsp?.Receive(nF);
    }

    /// <inheritdoc />
    public void Connect(IPlaybackController priorNode, IDSPStack targetNode)
    {
        _playback = priorNode;
        _dsp = targetNode;
    }

    /// <inheritdoc />
    public void ConnectBackend(IBackend backend)
    {
        _backend = backend;
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}