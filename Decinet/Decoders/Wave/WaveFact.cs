namespace Decinet.Decoders.Wave;

internal struct WaveFact
{
    public string SubChunkID;
    public uint SubChunkSize;
    // Technically this chunk could contain arbitrary data. But in practice
    // it only ever contains a single UInt32 representing the number of
    // samples.
    public uint NumSamples;

    public static WaveFact Parse(BinaryReader reader)
    {
        var fact = new WaveFact
        {
            SubChunkID = reader.ReadFourCc()
        };
        
        if (fact.SubChunkID != "fact")
        {
            throw new InvalidDataException("Invalid or missing .wav file fact chunk!");
        }
        fact.SubChunkSize = reader.ReadUInt32();
        if (fact.SubChunkSize != 4)
        {
            throw new NotSupportedException("Invalid or unknown .wav compression format!");
        }
        fact.NumSamples = reader.ReadUInt32();
        return fact;
    }
}