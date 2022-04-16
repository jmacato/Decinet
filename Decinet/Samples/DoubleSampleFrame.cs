using System.Buffers;
using Decinet.Architecture;

namespace Decinet.Samples;

public readonly struct DoubleSampleFrame : ISampleFrame<double>
{
    public DoubleSampleFrame(double[] interleavedSampleData, int sampleCount, int channelCount, AudioFormat audioFormat)
    {
        InterleavedSampleData = interleavedSampleData;
        SampleCount = sampleCount;
        ChannelCount = channelCount;
        AudioFormat = audioFormat;
    }

    public AudioFormat AudioFormat { get; }
    public int ChannelCount { get; }
    public int SampleCount { get; }
    public double[] InterleavedSampleData { get; }

    public void Dispose()
    {
        ArrayPool<double>.Shared.Return(InterleavedSampleData);
    }
}