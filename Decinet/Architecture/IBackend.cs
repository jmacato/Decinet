namespace Decinet.Architecture;

public interface IBackend : IProcessingNode<ISampleFrame, IDSPStack, IDecoder>
{
    Format DesiredFormat { get; }
}