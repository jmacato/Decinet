namespace Decinet.Backend.macOS.AudioUnit;

public enum AudioTypeMixer
{
    MultiChannel = 0x6d636d78, // 'mcmx'
    Matrix = 0x6d786d78, // 'mxmx'
    Spacial = 0x3364656d, // Same as Embedded3D
    Stereo = 0x736d7872, // 'smxr' 
}