namespace Decinet.Backend.macOS.AudioUnit;

[Flags]
public enum AudioUnitRenderActionFlags
{
    // UInt32 AudioUnitRenderActionFlags
    PreRender = 1 << 2,
    PostRender = 1 << 3,
    OutputIsSilence = 1 << 4,
    OfflinePreflight = 1 << 5,
    OfflineRender = 1 << 6,
    OfflineComplete = 1 << 7,
    PostRenderError = 1 << 8,
    DoNotCheckRenderArgs = 1 << 9
}