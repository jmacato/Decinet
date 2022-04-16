namespace Decinet.Architecture;

public interface IProcessingNode<in TInput, in TTarget> : IProcessingNode<TInput>
{
    void Connect(TTarget target);
}