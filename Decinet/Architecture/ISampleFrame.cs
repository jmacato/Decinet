namespace Decinet.Architecture;

public interface ISampleFrame<TSample> where TSample : INumber<TSample>
{
    static Format Format { get; }
    int ChannelNumber { get; }
    int SampleCount { get; }
    bool Reset { get; }
    Span<TSample> Samples { get; }
}