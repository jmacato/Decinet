namespace Decinet.Architecture;

public interface IResampler : IProcessingNode<ISampleFrame, IPlaybackController, IDspStack>
{
    void ConnectBackend(IBackend backend);
}