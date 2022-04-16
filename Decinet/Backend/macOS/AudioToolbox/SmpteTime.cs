using System.Runtime.InteropServices;

namespace Decinet.Backend.macOS.AudioToolbox;

[StructLayout(LayoutKind.Sequential)]
public struct SmpteTime
{
    // CoreAudio.framework - CoreAudioTypes.h
    public short Subframes;
    public short SubframeDivisor;
    public uint Counter;
    public uint Type;
    public uint Flags;
    public short Hours;
    public short Minutes;
    public short Seconds;
    public short Frames;

    public SmpteTimeFlags FlagsStrong
    {
        get => (SmpteTimeFlags) Flags;
        set => Flags = (uint) value;
    }

    public SmpteTimeType TypeStrong
    {
        get => (SmpteTimeType) Type;
        set => Type = (uint) value;
    }

    public override string ToString()
    {
        return
            $"[Subframes={Subframes},Divisor={SubframeDivisor},Counter={Counter},Type={Type},Flags={Flags},Hours={Hours},Minutes={Minutes},Seconds={Seconds},Frames={Frames}]";
    }
}