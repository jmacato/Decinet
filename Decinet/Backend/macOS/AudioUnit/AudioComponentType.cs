namespace Decinet.Backend.macOS.AudioUnit;

public enum AudioComponentType : uint
{
    // OSType in AudioComponentDescription
    Output = 0x61756f75, //'auou',
    MusicDevice = 0x61756d75, // 'aumu'
    MusicEffect = 0x61756d66, // 'aumf'
    FormatConverter = 0x61756663, // 'aufc'
    Effect = 0x61756678, // 'aufx'
    Mixer = 0x61756d78, // 'aumx'
    Panner = 0x6175706e, // 'aupn'
    OfflineEffect = 0x61756f6c, // 'auol'
    Generator = 0x6175676e, // 'augn' 
    MIDIProcessor = 0x61756d69, // 'aumi'
    RemoteEffect = 0x61757278, // 'aurx', 
    RemoteGenerator = 0x61757267, // 'aurg', 
    RemoteInstrument = 0x61757269, // 'auri', 
    RemoteMusicEffect = 0x6174726d, // 'aurm'
 }