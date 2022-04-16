using System.Runtime.InteropServices;

namespace Decinet.Backend.macOS.AudioUnit;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AUParameterEvent
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

    internal byte reserved1, reserved2, reserved3;

    public uint RampDurationSampleFrames;

    public ulong ParameterAddress;

    public float Value;
}