namespace Decinet.Architecture;

public interface IDspNode : IProcessingNode<ISampleFrame, IDspNode, IDspNode>
{
}