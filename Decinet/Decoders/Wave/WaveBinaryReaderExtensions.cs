namespace Decinet.Decoders.Wave;

internal static class WaveBinaryReaderExtensions
{
    public static string ReadFourCc(this BinaryReader reader, bool bigEndian = false)
    {
        var a = (char)reader.ReadByte();
        var b = (char)reader.ReadByte();
        var c = (char)reader.ReadByte();
        var d = (char)reader.ReadByte();

        return bigEndian
            ? new string(new[] { d, c, b, a })
            : new string(new[] { a, b, c, d });
    }
}