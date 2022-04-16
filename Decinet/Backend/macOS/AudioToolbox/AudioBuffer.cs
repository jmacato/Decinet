using System.Runtime.InteropServices;

namespace Decinet.Backend.macOS.AudioToolbox;

[StructLayout(LayoutKind.Sequential)]
public struct AudioBuffer
{
    public int NumberChannels;
    public int DataByteSize;
    public IntPtr Data;

    public override string ToString()
    {
        return $"[channels={NumberChannels},dataByteSize={DataByteSize},ptrData=0x{Data:x}]";
    }
}