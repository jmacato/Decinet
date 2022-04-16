using System.Buffers;
using Decinet.Architecture;

namespace Decinet.Samples;

public readonly struct ShortSampleFrame : ISampleFrame<short>
{
    public ShortSampleFrame(short[] interleavedSampleData, int sampleCount, int channelCount, Format format)
    {
        InterleavedSampleData = interleavedSampleData;
        SampleCount = sampleCount;
        ChannelCount = channelCount;
        Format = format;
    }

    public Format Format { get; }
    public int ChannelCount { get; }
    public int SampleCount { get; }
    public short[] InterleavedSampleData { get; }
    
    public void Dispose()
    {
        ArrayPool<short>.Shared.Return(InterleavedSampleData);
    }
}