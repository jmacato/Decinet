using System.Buffers;
using Decinet.Architecture;

namespace Decinet.Samples;

public readonly struct FloatSampleFrame : ISampleFrame<float>
{
    public FloatSampleFrame(float[] interleavedSampleData, int sampleCount, int channelCount, AudioFormat audioFormat)
    {
        InterleavedSampleData = interleavedSampleData;
        SampleCount = sampleCount;
        ChannelCount = channelCount;
        AudioFormat = audioFormat;
    }

    public static FloatSampleFrame Create(int samplesCount, int channelCount, AudioFormat audioFormat)
    {
        return new FloatSampleFrame(ArrayPool<float>.Shared.Rent(samplesCount * channelCount), samplesCount, channelCount, audioFormat);
    }

    public AudioFormat AudioFormat { get; }
    public int ChannelCount { get; }
    public int SampleCount { get; }
    public float[] InterleavedSampleData { get; }

    public void Dispose()
    {
        ArrayPool<float>.Shared.Return(InterleavedSampleData, true);
    }
}