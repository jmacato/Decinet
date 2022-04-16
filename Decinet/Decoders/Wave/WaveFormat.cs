namespace Decinet.Decoders.Wave;

internal struct WaveFormat
{
    public string SubChunkID;
    public uint SubChunkSize;
    public WaveFormatType WaveType;
    public ushort NumChannels;
    public uint SampleRate;
    public uint ByteRate;
    public ushort BlockAlign;
    public ushort BitsPerSample;
    public ushort ExtraBytesSize; // Only used in certain compressed formats
    public byte[] ExtraBytes; // Only used in certain compressed formats

    public static WaveFormat Parse(BinaryReader reader)
    {
        var format = new WaveFormat
        {
            SubChunkID = reader.ReadFourCc()
        };
        
        if (format.SubChunkID != "fmt ")
        {
            throw new InvalidDataException("Invalid or missing .wav file format chunk!");
        }
        
        format.SubChunkSize = reader.ReadUInt32();
        format.WaveType = (WaveFormatType) reader.ReadUInt16();
        format.NumChannels = reader.ReadUInt16();
        format.SampleRate = reader.ReadUInt32();
        format.ByteRate = reader.ReadUInt32();
        format.BlockAlign = reader.ReadUInt16();
        format.BitsPerSample = reader.ReadUInt16();

        if (format.SubChunkSize == 18)
        {
            reader.ReadInt16();
        }

        switch (format.WaveType)
        {
            case WaveFormatType.Pcm:
                format.ExtraBytesSize = 0;
                format.ExtraBytes = Array.Empty<byte>();
                break;
            case WaveFormatType.DviAdpcm:
                if (format.NumChannels != 1)
                {
                    throw new NotSupportedException("Only single channel DVI ADPCM compressed .wavs are supported.");
                }
                format.ExtraBytesSize = reader.ReadUInt16();
                if (format.ExtraBytesSize != 2)
                {
                    throw new InvalidDataException("Invalid .wav DVI ADPCM format!");
                }
                format.ExtraBytes = reader.ReadBytes(format.ExtraBytesSize);
                break;
            default:
                throw new NotSupportedException("Invalid or unknown .wav compression format!");
        }
        return format;
    }
};