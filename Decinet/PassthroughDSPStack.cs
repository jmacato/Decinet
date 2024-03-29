using Decinet.Architecture;

namespace Decinet;

public class PassthroughDspStack : IDspStack
{
    private IResampler _resampler;
    private IBackend _backend;

    /// <inheritdoc />
    public void Receive(ISampleFrame data)
    {
        _backend?.Receive(data);
    }

    /// <inheritdoc />
    public void Connect(IResampler priorNode, IBackend targetNode)
    {
        _resampler = priorNode;
        _backend = targetNode;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _resampler = null!;
        _backend = null!;
    }

    /// <inheritdoc />
    public void Initialize(AudioFormat incomingAudioFormat)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Add(IDspNode node)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Remove(IDspNode node)
    {
        throw new NotImplementedException();
    }
}