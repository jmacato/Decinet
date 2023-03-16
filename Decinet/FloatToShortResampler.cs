using Decinet.Architecture;
using Decinet.Samples;

namespace Decinet;

public class FloatToShortResampler : IResampler
{
    private IBackend? _backend;
    private IPlaybackController? _playbackController;
    private IDspStack? _dspStack;
    private bool _isDisposed = false;
    private IResampler? _outResampler;

    public void ConnectOutToResampler(IResampler resampler)
    {
        _outResampler = resampler;
    }
    public FloatToShortResampler()
    {
    }

    /// <inheritdoc />
    public void Receive(ISampleFrame? data)
    {
        if (data is null || _isDisposed || _backend is null) return;

        var incomingFormat = data.AudioFormat;
        var receivingFormat = _backend!.DesiredAudioFormat;

        if (incomingFormat == receivingFormat)
            _dspStack?.Receive(data);
  


        if (data is not FloatSampleFrame frame)
        {

            if (_outResampler is not null)
            {
                _outResampler?.Receive(data);
            }
            else
            {
                _dspStack?.Receive(data);
            }
            
            return;
        }

        ProcessFloatToShort(frame,
            receivingFormat,
            out var sampleFrame);

        if (_outResampler is not null)
        {
            _outResampler?.Receive(sampleFrame);
        }
        else
        {
            _dspStack?.Receive(sampleFrame);
        }
    }

    private void ProcessFloatToShort(FloatSampleFrame data, AudioFormat receivingFormat,
        out ShortSampleFrame shortSampleFrame)
    {
        shortSampleFrame = ShortSampleFrame.Create(data.SampleCount, data.ChannelCount, receivingFormat, data.FrameTime);
        for (var i = 0; i < data.InterleavedSampleData.Length; i++)
        {
            var shortSample = (short) (data.InterleavedSampleData[i] * (short.MaxValue - 1));
            shortSampleFrame.InterleavedSampleData[i] = shortSample;
        }
    }

    /// <inheritdoc />
    public void Connect(IPlaybackController priorNode, IDspStack? targetNode)
    {
        _playbackController = priorNode;
        _dspStack = targetNode;
    }

    /// <inheritdoc />
    public void ConnectBackend(IBackend backend)
    {
        _backend = backend;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _isDisposed = true;
        _backend = null!;
        _playbackController = null!;
        _dspStack = null!;
    }
}