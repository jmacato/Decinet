namespace Decinet.Architecture;

public interface IDSPNode : IProcessingNode<ISampleFrame, IDSPNode, IDSPNode>
{
}