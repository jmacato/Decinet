using Decinet.Architecture;

namespace Decinet.Decoders.Wave;

internal abstract class WaveParser
{
    internal WaveParser(BinaryReader currentStream,
        WaveFormat format,
        long rawDataStartOffset)
    {
        CurrentStream = currentStream;
        RawDataStartOffset = rawDataStartOffset;
        TotalRawDataBytes = currentStream.BaseStream.Length - rawDataStartOffset;
        Format = format;
        Duration = TimeSpan.FromSeconds(TotalRawDataBytes / (float)format.ByteRate);
    }

    protected readonly BinaryReader CurrentStream;
    protected readonly long RawDataStartOffset;
    protected readonly long TotalRawDataBytes;
    protected readonly TimeSpan Duration;
    protected readonly WaveFormat Format;
    protected TimeSpan CurrentPosition;

    public abstract bool TryGetBytes(int numSamples, out ISampleFrame sampleFrame);

    public static WaveParser GetParser(
        BinaryReader br,
        WaveFormat waveFormat,
        long rawDataStart)
    {
        return waveFormat.WaveType switch
        {
            WaveFormatType.Pcm => new PcmParser(br, waveFormat, rawDataStart),
            WaveFormatType.DviAdpcm => new DviAdpcmParser(br, waveFormat, rawDataStart),
            _ => throw new NotSupportedException("Invalid or unknown .wav compression format!")
        };
    }
}