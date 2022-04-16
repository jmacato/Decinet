namespace Decinet.Architecture;

public interface IBackend : IProcessingNode<ISampleFrame, IDSPStack, IDecoder>
{
    AudioFormat DesiredAudioFormat { get; }
}