namespace Decinet.Backend.macOS.AudioUnit;

[Flags]
public enum AudioUnitParameterFlag : uint // UInt32 in AudioUnitParameterInfo
{
    CFNameRelease = 1 << 4,

    ////[iOS (8,0)]
    OmitFromPresets = 1 << 13,
    PlotHistory = 1 << 14,
    MeterReadOnly = 1 << 15,

    // bit positions 18,17,16 are set aside for display scales. bit 19 is reserved.
    DisplayMask = (7 << 16) | (1 << 22),
    DisplaySquareRoot = 1 << 16,
    DisplaySquared = 2 << 16,
    DisplayCubed = 3 << 16,
    DisplayCubeRoot = 4 << 16,
    DisplayExponential = 5 << 16,

    HasClump = 1 << 20,
    ValuesHaveStrings = 1 << 21,

    DisplayLogarithmic = 1 << 22,

    IsHighResolution = 1 << 23,
    NonRealTime = 1 << 24,
    CanRamp = 1 << 25,
    ExpertMode = 1 << 26,
    HasCFNameString = 1 << 27,
    IsGlobalMeta = 1 << 28,
    IsElementMeta = 1 << 29,
    IsReadable = 1 << 30,
    IsWritable = (uint) 1 << 31
}