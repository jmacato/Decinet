namespace Decinet.Decoders.Wave;

internal static class WaveBinaryReaderExtensions
{
    public static string ReadFourCc(this BinaryReader reader, bool bigEndian = false)
    {
        var a = reader.ReadChar();
        var b = reader.ReadChar();
        var c = reader.ReadChar();
        var d = reader.ReadChar();

        return bigEndian
            ? new string(new[] { d, c, b, a })
            : new string(new[] { a, b, c, d });
    }
}