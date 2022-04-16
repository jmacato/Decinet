namespace Decinet.Architecture;

public interface IResampler : IProcessingNode<ISampleFrame, IPlaybackController, IDSPStack>
{
    void Initialize(AudioFormat incomingAudioFormat, AudioFormat outgoingAudioFormat);
}