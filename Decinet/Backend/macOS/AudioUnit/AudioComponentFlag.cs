namespace Decinet.Backend.macOS.AudioUnit;

[Flags]
public enum AudioComponentFlag // UInt32 in AudioComponentDescription
{
    Unsearchable = 1,
    SandboxSafe = 2,
    IsV3AudioUnit = 4,
    RequiresAsyncInstantiation = 8,
    CanLoadInProcess = 0x10
}