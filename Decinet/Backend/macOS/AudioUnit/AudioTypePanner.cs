namespace Decinet.Backend.macOS.AudioUnit;

public enum AudioTypePanner
{
    // OSType in AudioComponentDescription
    SphericalHead = 0x73706872, // 'sphr'
    Vector = 0x76626173, // 'vbas'
    SoundField = 0x616d6269, // 'ambi'
    rHRTF = 0x68727466, // 'hrtf'
}