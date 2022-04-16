namespace Decinet.Architecture;

public interface IBackend : IProcessingNode<IDSPStack>
{
    Format DesiredFormat { get; }
}