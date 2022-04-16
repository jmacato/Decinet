namespace Decinet.Architecture;

public interface IProcessingNode<in TData> : IDisposable
{
    void Receive(TData data);
}

public interface IProcessingNode<in TData, in TTarget> : IProcessingNode<TData>
{
    void Connect(TTarget targetNode);
}


public interface IProcessingNode<in TData, in TPrior, in TTarget> : IProcessingNode<TData>
{
    void Connect(TPrior priorNode, TTarget targetNode);
}