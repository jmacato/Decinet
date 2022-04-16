using System.Runtime.InteropServices;

namespace Decinet.Backend.macOS.AudioUnit;

[StructLayout(LayoutKind.Sequential)]
public struct AudioUnitParameterEvent
{
    public uint Scope;
    public uint Element;
    public uint Parameter;
    public AUParameterEventType EventType;

    [StructLayout(LayoutKind.Explicit)]
    public struct EventValuesStruct
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RampStruct
        {
            public int StartBufferOffset;
            public uint DurationInFrames;
            public float StartValue;
            public float EndValue;
        }


        [FieldOffset(0)] public RampStruct Ramp;

        [StructLayout(LayoutKind.Sequential)]
        public struct ImmediateStruct
        {
            public uint BufferOffset;
            public float Value;
        }

        [FieldOffset(0)] public ImmediateStruct Immediate;
    }

    public EventValuesStruct EventValues;
}