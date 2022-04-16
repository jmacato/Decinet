using Decinet.Backend.macOS.AudioToolbox;

namespace Decinet.Backend.macOS.AudioUnit;

internal delegate AudioUnitStatus CallbackShared(IntPtr /* void* */ clientData,
    ref AudioUnitRenderActionFlags /* AudioUnitRenderActionFlags* */ actionFlags,
    ref AudioTimeStamp /* AudioTimeStamp* */ timeStamp, uint /* UInt32 */ busNumber, uint /* UInt32 */ numberFrames,
    IntPtr /* AudioBufferList* */ data);