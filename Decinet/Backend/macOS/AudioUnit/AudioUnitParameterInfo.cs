using Decinet.Backend.macOS.CoreFoundation;

namespace Decinet.Backend.macOS.AudioUnit;

public class AudioUnitParameterInfo
{
    public string? UnitName { get; private set; }
    public AudioUnitClumpID ClumpID { get; private set; }
    public string? Name { get; private set; }
    public AudioUnitParameterUnit Unit { get; private set; }
    public float MinValue { get; private set; }
    public float MaxValue { get; private set; }
    public float DefaultValue { get; private set; }
    public AudioUnitParameterFlag Flags { get; private set; }
    public AudioUnitParameterType Type { get; private set; }

    internal static AudioUnitParameterInfo Create(AudioUnitParameterInfoNative native, AudioUnitParameterType type)
    {
        var info = new AudioUnitParameterInfo();

        // Remove obj-c noise
        info.Flags = native.Flags &
                     ~(AudioUnitParameterFlag.HasCFNameString | AudioUnitParameterFlag.CFNameRelease);
        info.Unit = native.Unit;
        info.MinValue = native.MinValue;
        info.MaxValue = native.MaxValue;
        info.DefaultValue = native.DefaultValue;
        info.ClumpID = native.ClumpID;
        info.Type = type;

        if ((native.Flags & AudioUnitParameterFlag.HasCFNameString) != 0)
        {
            info.Name = CfString.FromHandle(native.NameString);

            if ((native.Flags & AudioUnitParameterFlag.CFNameRelease) != 0)
                CFObject.CFRelease(native.NameString);
        }

        if (native.Unit == AudioUnitParameterUnit.CustomUnit) info.UnitName = CfString.FromHandle(native.UnitName);

        return info;
    }
}