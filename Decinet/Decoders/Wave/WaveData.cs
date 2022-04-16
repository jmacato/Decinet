namespace Decinet.Decoders.Wave;

internal struct WaveData
{
    public string SubChunkID; // should contain the word data
    public uint SubChunkSize; // Stores the size of the data block

    public static WaveData Parse(BinaryReader reader)
    {
        var data = new WaveData
        {
            SubChunkID = reader.ReadFourCc()
        };
        if (data.SubChunkID != "data")
        {
            throw new InvalidDataException("Invalid or missing .wav file data chunk!");
        }
        data.SubChunkSize = reader.ReadUInt32();
        return data;
    }
};