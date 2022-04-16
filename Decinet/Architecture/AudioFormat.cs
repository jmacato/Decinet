namespace Decinet.Architecture;

public readonly struct AudioFormat : IEquatable<AudioFormat>
{
    public AudioFormat(Type sampleType, int bytesPerSample, int sampleRate, int channelCount)
    {
        SampleType = sampleType;
        BytesPerSample = bytesPerSample;
        SampleRate = sampleRate;
        ChannelCount = channelCount;
    }

    public Type SampleType { get; }
    public int BytesPerSample { get; }
    public int SampleRate { get; }
    public int ChannelCount { get; }


    /// <inheritdoc />
    public bool Equals(AudioFormat other)
    {
        return SampleType == other.SampleType && BytesPerSample == other.BytesPerSample &&
               SampleRate == other.SampleRate && ChannelCount == other.ChannelCount;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is AudioFormat other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(SampleType, BytesPerSample, SampleRate, ChannelCount);
    }

    public static bool operator ==(AudioFormat left, AudioFormat right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(AudioFormat left, AudioFormat right)
    {
        return !(left == right);
    }
}