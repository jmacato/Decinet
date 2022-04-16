using System.Runtime.InteropServices;

namespace Decinet.Backend.macOS.AudioToolbox;

[StructLayout(LayoutKind.Sequential)]
public struct AudioStreamPacketDescription
{
    public long StartOffset;
    public int VariableFramesInPacket;
    public int DataByteSize;

    public override string ToString()
    {
        return $"StartOffset={StartOffset} VariableFramesInPacket={VariableFramesInPacket} DataByteSize={DataByteSize}";
    }
}