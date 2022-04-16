namespace Decinet.Architecture;

public interface IDSPStack : IProcessingNode<ISampleFrame, IResampler, IBackend>
{
    void Initialize(AudioFormat incomingAudioFormat);
    void Add(IDSPNode node);
    void Remove(IDSPNode node);
}