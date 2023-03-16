using System.Buffers;
using Decinet.Architecture;

namespace Decinet.Samples;

public readonly struct FloatSampleFrame : ISampleFrame<float>
{
    private FloatSampleFrame(float[] interleavedSampleData, int sampleCount, int channelCount, AudioFormat audioFormat, TimeSpan frameTime)
    {
        InterleavedSampleData = interleavedSampleData;
        SampleCount = sampleCount;
        ChannelCount = channelCount;
        AudioFormat = audioFormat;
        FrameTime = frameTime;
    }

    public static FloatSampleFrame Create(int samplesCount, int channelCount, AudioFormat audioFormat, TimeSpan frameTime)
    {
        return new FloatSampleFrame(new float[samplesCount * channelCount], samplesCount, channelCount, audioFormat, frameTime);
    }

    public AudioFormat AudioFormat { get; }
    public int ChannelCount { get; }
    public int SampleCount { get; }
    
    public TimeSpan FrameTime { get; }
    public float[] InterleavedSampleData { get; }

    public void Dispose()
    {

    }
}