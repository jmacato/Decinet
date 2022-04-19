using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using Decinet.Architecture;
using Decinet.Samples;
using Decinet.Utilities;
using Microsoft.Win32.SafeHandles;

namespace Decinet;

public class ShortToFloatResampler : IResampler
{
    private IBackend? _backend;
    private IPlaybackController? _playbackController;
    private IDSPStack? _dspStack;
    private bool _isDisposed = false;
    private FileStream _pip;

    public ShortToFloatResampler()
    {
        _pip = File.Create($"{Guid.NewGuid():N}.resampl.rawshort");
    }

    /// <inheritdoc />
    public void Receive(ISampleFrame? data)
    {
        if (data is null || _isDisposed || _backend is null) return;

        var incomingFormat = data.AudioFormat;
        var receivingFormat = _backend!.DesiredAudioFormat;

        if (incomingFormat == receivingFormat)
            _dspStack?.Receive(data);

        if (incomingFormat.SampleType != typeof(short) || receivingFormat.SampleType != typeof(float)) return;
        
        if (incomingFormat.SampleRate != receivingFormat.SampleRate)
        {
            // TODO: Warn here?
        }

        if (data is not ShortSampleFrame frame)
        {
            throw new InvalidDataException();
        }

        ProcessShortToFloat(frame,
            receivingFormat,
            out FloatSampleFrame sampleFrame);

        _dspStack?.Receive(sampleFrame);

    }

    private void ProcessShortToFloat(ShortSampleFrame data, AudioFormat receivingFormat,
        out FloatSampleFrame floatSampleFrame)
    {
        floatSampleFrame = FloatSampleFrame.Create(data.SampleCount, data.ChannelCount, receivingFormat);
        for (var i = 0; i < data.InterleavedSampleData.Length; i++)
        {
            var shortSample = data.InterleavedSampleData[i] / (float) (short.MaxValue - 1);
            floatSampleFrame.InterleavedSampleData[i] = shortSample;
        }
    }

    /// <inheritdoc />
    public void Connect(IPlaybackController priorNode, IDSPStack targetNode)
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