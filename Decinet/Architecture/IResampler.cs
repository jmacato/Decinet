namespace Decinet.Architecture;

public interface IResampler : IProcessingNode<ISampleFrame, IPlaybackController, IDSPStack>
{
    void ConnectBackend(IBackend backend);
}