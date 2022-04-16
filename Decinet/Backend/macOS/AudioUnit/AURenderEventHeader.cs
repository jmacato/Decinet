using System.Runtime.InteropServices;

namespace Decinet.Backend.macOS.AudioUnit;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AURenderEventHeader
{
    public AURenderEvent* UnsafeNext;

    public AURenderEvent? Next
    {
        get
        {
            if (UnsafeNext is not null)
                return (AURenderEvent?) Marshal.PtrToStructure((IntPtr) UnsafeNext, typeof(AURenderEvent));
            return null;
        }
    }

    public long EventSampleTime;

    public AURenderEventType EventType;

    public byte Reserved;
}