using System.Reflection.Metadata.Ecma335;
using Decinet.Architecture;

namespace Decinet;

public class ShortToFloatResampler : IResampler
{
    private IBackend? _backend;
    private IPlaybackController? _playbackController;
    private IDSPStack? _dspStack;
    private bool _isDisposed = false;

    /// <inheritdoc />
    public void Receive(ISampleFrame data)
    {
        if (_isDisposed && _backend is null) return;

        var incomingFormat = data.AudioFormat;
        var receivingFormat = _backend!.DesiredAudioFormat;

        if (incomingFormat == receivingFormat)
            _dspStack?.Receive(data);
        
        
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