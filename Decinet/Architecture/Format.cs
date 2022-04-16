namespace Decinet.Architecture;

public struct Format
{
    public Format(Type sampleType, int bytesPerSample, int sampleRate, int channelCount)
    {
        SampleType = sampleType;
        BytesPerSample = bytesPerSample;
        SampleRate = sampleRate;
        ChannelCount = channelCount;
    }

    Type SampleType { get; }
    int BytesPerSample { get; }
    int SampleRate { get; }
    int ChannelCount { get; }
}