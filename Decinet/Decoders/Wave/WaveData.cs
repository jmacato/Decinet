namespace Decinet.Decoders.Wave;

internal struct WaveData
{
    public string SubChunkID; // should contain the word data
    public uint SubChunkSize; // Stores the size of the data block

    public static WaveData Parse(BinaryReader reader)
    {
        do
        {
            if (reader.ReadFourCc() == "LIST")
            {
                var skip = reader.ReadUInt32();
                reader.BaseStream.Position += skip;
                continue;
            }
            
            reader.BaseStream.Position -= 4;
            break;
        } while (reader.BaseStream.CanRead);

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