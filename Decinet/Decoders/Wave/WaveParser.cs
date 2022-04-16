using Decinet.Architecture;

namespace Decinet.Decoders.Wave;

internal abstract class WaveParser
{
    internal WaveParser(BinaryReader currentStream,
        WaveFormat format,
        long rawDataStart)
    {
        CurrentStream = currentStream;
        RawDataStart = rawDataStart;
        var totalRawDataBytes = currentStream.BaseStream.Length - rawDataStart;
        var duration = (float)totalRawDataBytes / (float)format.ByteRate;
        var t = TimeSpan.FromSeconds(duration);
    }

    protected readonly BinaryReader CurrentStream;
    protected readonly long RawDataStart;
    protected readonly TimeSpan Duration;

    public abstract ISampleFrame GetBytes(
        TimeSpan position,
        int numSamples);

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