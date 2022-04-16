namespace Decinet.Backend.macOS.AudioToolbox;

public enum SmpteTimeType : uint // UInt32 in AudioFileRegionList
{
    [Obsolete("Value is not to be used with any API.")]
    None = uint.MaxValue,
    Type24 = 0,
    Type25 = 1,
    Type30Drop = 2,
    Type30 = 3,
    Type2997 = 4,
    Type2997Drop = 5,
    Type60 = 6,
    Type5994 = 7,
    Type60Drop = 8,
    Type5994Drop = 9,
    Type50 = 10,
    Type2398 = 11
}