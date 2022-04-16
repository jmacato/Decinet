namespace Decinet.Backend.macOS.AudioToolbox;

[Flags]
public enum AudioFormatFlags : uint // UInt32 in AudioStreamBasicDescription
{
    IsFloat = 1 << 0, // 0x1
    IsBigEndian = 1 << 1, // 0x2
    IsSignedInteger = 1 << 2, // 0x4
    IsPacked = 1 << 3, // 0x8
    IsAlignedHigh = 1 << 4, // 0x10
    IsNonInterleaved = 1 << 5, // 0x20
    IsNonMixable = 1 << 6, // 0x40
    FlagsAreAllClear = unchecked((uint) (1 << 31)),

    LinearPCMIsFloat = 1 << 0, // 0x1
    LinearPCMIsBigEndian = 1 << 1, // 0x2
    LinearPCMIsSignedInteger = 1 << 2, // 0x4
    LinearPCMIsPacked = 1 << 3, // 0x8
    LinearPCMIsAlignedHigh = 1 << 4, // 0x10
    LinearPCMIsNonInterleaved = 1 << 5, // 0x20
    LinearPCMIsNonMixable = 1 << 6, // 0x40

    LinearPCMSampleFractionShift = 7,
    LinearPCMSampleFractionMask = 0x3F << (int) LinearPCMSampleFractionShift,
    LinearPCMFlagsAreAllClear = FlagsAreAllClear,

    AppleLossless16BitSourceData = 1,
    AppleLossless20BitSourceData = 2,
    AppleLossless24BitSourceData = 3,
    AppleLossless32BitSourceData = 4,

    CafIsFloat = 1 << 0,
    CafIsLittleEndian = 1 << 1
}