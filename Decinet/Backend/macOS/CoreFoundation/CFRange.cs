using System.Runtime.InteropServices;

namespace Decinet.Backend.macOS.CoreFoundation;

[StructLayout(LayoutKind.Sequential)]
public readonly struct CFRange
{
    private readonly nint loc; // defined as 'long' in native code
    private readonly nint len; // defined as 'long' in native code

    public int Location => (int) loc;

    public int Length => (int) len;

    public long LongLocation => loc;

    public long LongLength => len;

    public CFRange(int loc, int len)
    {
        this.loc = loc;
        this.len = len;
    }

    public CFRange(long l, long len)
    {
        loc = (nint) l;
        this.len = (nint) len;
    }

    public CFRange(nint l, nint len)
    {
        loc = l;
        this.len = len;
    }

    public override string ToString()
    {
        return $"CFRange [Location: {loc} Length: {len}]";
    }
}