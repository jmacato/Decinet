namespace Decinet.Architecture;

public interface IResampler : IProcessingNode<IPlaybackController, IDSPStack>
{
    void Initialize(Format incomingFormat, Format outgoingFormat);
}