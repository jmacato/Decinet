using Decinet.Architecture;

namespace Decinet.Samples;

public struct UShortSampleFrame : ISampleFrame<ushort>
{
    public Format Format { get; }
    public int ChannelCount { get; }
    public int SampleCount { get; }
    public ushort[,] SampleData { get; }
}
