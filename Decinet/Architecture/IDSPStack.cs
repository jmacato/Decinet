namespace Decinet.Architecture;

public interface IDspStack : IProcessingNode<ISampleFrame, IResampler, IBackend>
{
    void Initialize(AudioFormat incomingAudioFormat);
    void Add(IDspNode node);
    void Remove(IDspNode node);
}