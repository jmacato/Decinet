using System.Runtime.InteropServices;

namespace Decinet.Backend.macOS.AudioToolbox;

[StructLayout(LayoutKind.Sequential)]
public struct AudioStreamBasicDescription
{
    private double SampleRate;
    private AudioFormatType Format;
    private AudioFormatFlags FormatFlags;
    private int BytesPerPacket; // uint
    private int FramesPerPacket; // uint
    private int BytesPerFrame; // uint
    private int ChannelsPerFrame; // uint
    private int BitsPerChannel; // uint
    private int Reserved; // uint
    
    private AudioStreamBasicDescription(AudioFormatType formatType)
        : this()
    {
        Format = formatType;
    }

    public static AudioStreamBasicDescription CreateLinearPCM(double sampleRate = 44100, uint channelsPerFrame = 2,
        uint bitsPerChannel = 16, bool bigEndian = false)
    {
        var desc = new AudioStreamBasicDescription(AudioFormatType.LinearPCM);
        desc.SampleRate = sampleRate;
        desc.ChannelsPerFrame = (int) channelsPerFrame;
        desc.BitsPerChannel = (int) bitsPerChannel;
        desc.BytesPerPacket = desc.BytesPerFrame = (int) channelsPerFrame * sizeof(short);
        desc.FramesPerPacket = 1;
        desc.FormatFlags = AudioFormatFlags.IsSignedInteger | AudioFormatFlags.IsPacked;
        if (bigEndian)
            desc.FormatFlags |= AudioFormatFlags.IsBigEndian;

        return desc;
    }


    public override string ToString()
    {
        return $"[SampleRate={SampleRate} FormatID={Format} FormatFlags={FormatFlags} BytesPerPacket={BytesPerPacket} FramesPerPacket={FramesPerPacket} BytesPerFrame={BytesPerFrame} ChannelsPerFrame={ChannelsPerFrame} BitsPerChannel={BitsPerChannel}]";
    }
}