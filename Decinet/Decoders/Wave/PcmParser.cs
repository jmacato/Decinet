using Decinet.Architecture;
using Decinet.Samples;

namespace Decinet.Decoders.Wave;

internal class PcmParser : WaveParser
{
    public PcmParser(BinaryReader currentStream, WaveFormat format, long rawDataStart) : base(currentStream, format, rawDataStart)
    {
    }

    public override ISampleFrame GetBytes(TimeSpan position, int numSamples)
    {
        throw new NotImplementedException();
    }
}