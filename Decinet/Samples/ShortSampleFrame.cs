using System.Buffers;
using Decinet.Architecture;

namespace Decinet.Samples;

public readonly struct ShortSampleFrame : ISampleFrame<short>
{
    public ShortSampleFrame(short[] interleavedSampleData, int sampleCount, int channelCount, AudioFormat audioFormat)
    {
        InterleavedSampleData = interleavedSampleData;
        SampleCount = sampleCount;
        ChannelCount = channelCount;
        AudioFormat = audioFormat;
    }

    public AudioFormat AudioFormat { get; }
    public int ChannelCount { get; }
    public int SampleCount { get; }
    public short[] InterleavedSampleData { get; }
    
    public static ShortSampleFrame Create(int samplesCount, int channelCount, AudioFormat audioFormat)
    {
        return new ShortSampleFrame(ArrayPool<short>.Shared.Rent(samplesCount * channelCount), samplesCount, channelCount, audioFormat);
    }
    
    public void Dispose()
    {
        ArrayPool<short>.Shared.Return(InterleavedSampleData, true);
    }
}