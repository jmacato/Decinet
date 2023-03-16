using System.Buffers;
using Decinet.Architecture;

namespace Decinet.Samples;

public readonly struct ShortSampleFrame : ISampleFrame<short>
{
    private ShortSampleFrame(short[] interleavedSampleData, int sampleCount, int channelCount, AudioFormat audioFormat, TimeSpan frameTime)
    {
        InterleavedSampleData = interleavedSampleData;
        SampleCount = sampleCount;
        ChannelCount = channelCount;
        AudioFormat = audioFormat;
        FrameTime = frameTime;
    }

    public AudioFormat AudioFormat { get; }
    public int ChannelCount { get; }
    public int SampleCount { get; }
    public TimeSpan FrameTime { get; }
    public short[] InterleavedSampleData { get; }

    public static ShortSampleFrame Create(int samplesCount, int channelCount, AudioFormat audioFormat, TimeSpan frameTime)
    {
        return new ShortSampleFrame(new short[samplesCount * channelCount], samplesCount, channelCount, audioFormat, frameTime);
    }

    public void Dispose()
    {
    }
}