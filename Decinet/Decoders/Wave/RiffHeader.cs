namespace Decinet.Decoders.Wave;

internal struct RiffHeader
{
    public string ChunkId;
    public uint ChunkSize;
    public string Format;

    public static RiffHeader Parse(BinaryReader reader)
    {
        var header = new RiffHeader
        {
            ChunkId = reader.ReadFourCc(),
            ChunkSize = reader.ReadUInt32(),
            Format = reader.ReadFourCc()
        };

        if (header.ChunkId != "RIFF" ||
            header.Format != "WAVE")
        {
            throw new InvalidDataException("Invalid or missing .wav file header!");
        }

        return header;
    }
}