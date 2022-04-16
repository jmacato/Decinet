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

    public AudioFormat AudioFormat { get; }
    public int ChannelCount { get; }
    public int SampleCount { get; }
    public float[] InterleavedSampleData { get; }

    public void Dispose()
    {
        ArrayPool<float>.Shared.Return(InterleavedSampleData);
    }
}