using Decinet.Backend.macOS.AudioToolbox;

namespace Decinet.Backend.macOS.AudioUnit;

public delegate AudioUnitStatus RenderDelegate(AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp,
    uint busNumber, uint numberFrames, AudioBuffers data);