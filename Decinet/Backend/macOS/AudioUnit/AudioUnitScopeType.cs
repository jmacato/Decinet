namespace Decinet.Backend.macOS.AudioUnit;

public enum AudioUnitScopeType
{
    // UInt32 AudioUnitScope
    Global = 0,
    Input = 1,
    Output = 2,
    Group = 3,
    Part = 4,
    Note = 5,
    Layer = 6,
    LayerItem = 7
}