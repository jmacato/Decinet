using System.Numerics;

namespace Decinet.Architecture;

public interface ISampleFrame : IDisposable
{ 
    AudioFormat AudioFormat { get; }
    int ChannelCount { get; }
    int SampleCount { get; }
}

public interface ISampleFrame<out TSample> : ISampleFrame where TSample : INumber<TSample>
{
    TSample[] InterleavedSampleData { get; }
}