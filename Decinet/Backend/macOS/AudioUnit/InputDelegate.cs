using Decinet.Backend.macOS.AudioToolbox;

namespace Decinet.Backend.macOS.AudioUnit;

public delegate AudioUnitStatus InputDelegate(AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp,
    uint busNumber, uint numberFrames, AudioUnit audioUnit);