namespace Decinet.Backend.macOS.AudioUnit;

public enum AudioUnitParameterUnit // UInt32 AudioUnitParameterUnit
{
    Generic = 0,
    Indexed = 1,
    Boolean = 2,
    Percent = 3,
    Seconds = 4,
    SampleFrames = 5,
    Phase = 6,
    Rate = 7,
    Hertz = 8,
    Cents = 9,
    RelativeSemiTones = 10,
    MIDINoteNumber = 11,
    MIDIController = 12,
    Decibels = 13,
    LinearGain = 14,
    Degrees = 15,
    EqualPowerCrossfade = 16,
    MixerFaderCurve1 = 17,
    Pan = 18,
    Meters = 19,
    AbsoluteCents = 20,
    Octaves = 21,
    BPM = 22,
    Beats = 23,
    Milliseconds = 24,
    Ratio = 25,
    CustomUnit = 26,

    ////[iOS (15,0), TV (15,0), Mac (12,0), MacCatalyst (15,0)]
    MIDI2Controller = 27
}