namespace Decinet.Architecture;

public interface IDSPStack : IProcessingNode<IResampler, IBackend>
{
    void Initialize(Format incomingFormat);
    void Add(IDSPNode node);
    void Remove(IDSPNode node);
}