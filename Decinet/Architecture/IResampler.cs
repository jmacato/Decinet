namespace Decinet.Architecture;

public interface IResampler : IProcessingNode<ISampleFrame, IPlaybackController, IDSPStack>
{
    void Initialize(Format incomingFormat, Format outgoingFormat);
}