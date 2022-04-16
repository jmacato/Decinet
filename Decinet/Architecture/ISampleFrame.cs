namespace Decinet.Architecture;

public interface ISampleFrame : IDisposable
{ 
    Format Format { get; }
    int ChannelCount { get; }
    int SampleCount { get; }
}

public interface ISampleFrame<out TSample> : ISampleFrame where TSample : INumber<TSample>
{
    TSample[] InterleavedSampleData { get; }
}