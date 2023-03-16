using System.Buffers;
using Decinet.Architecture;

namespace Decinet.Samples;

public readonly struct DoubleSampleFrame : ISampleFrame<double>
{
    private DoubleSampleFrame(double[] interleavedSampleData, int sampleCount, int channelCount, AudioFormat audioFormat, TimeSpan frameTime)
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
    public double[] InterleavedSampleData { get; }

    public void Dispose()
    {
        ArrayPool<double>.Shared.Return(InterleavedSampleData);
    }
}