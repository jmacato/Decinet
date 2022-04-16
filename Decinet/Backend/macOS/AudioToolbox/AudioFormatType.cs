namespace Decinet.Backend.macOS.AudioToolbox;

public enum AudioFormatType : uint
{
    // UInt32 in AudioStreamBasicDescription -- CoreAudio.framework CoreAudioTypes.h
    LinearPCM = 0x6c70636d,
    AC3 = 0x61632d33,
    CAC3 = 0x63616333,
    AppleIMA4 = 0x696d6134,
    MPEG4AAC = 0x61616320,
    MPEG4CELP = 0x63656c70,
    MPEG4HVXC = 0x68767863,
    MPEG4TwinVQ = 0x74777671,
    MACE3 = 0x4d414333,
    MACE6 = 0x4d414336,
    ULaw = 0x756c6177,
    ALaw = 0x616c6177,
    QDesign = 0x51444d43,
    QDesign2 = 0x51444d32,
    QUALCOMM = 0x51636c70,
    MPEGLayer1 = 0x2e6d7031,
    MPEGLayer2 = 0x2e6d7032,
    MPEGLayer3 = 0x2e6d7033,
    TimeCode = 0x74696d65,
    MIDIStream = 0x6d696469,
    ParameterValueStream = 0x61707673,
    AppleLossless = 0x616c6163,
    MPEG4AAC_HE = 0x61616368,
    MPEG4AAC_LD = 0x6161636c,
    MPEG4AAC_ELD = 0x61616365, // 'aace'
    MPEG4AAC_ELD_SBR = 0x61616366, // 'aacf',
    MPEG4AAC_ELD_V2 = 0x61616367, // 'aacg',    
    MPEG4AAC_HE_V2 = 0x61616370,
    MPEG4AAC_Spatial = 0x61616373,
    MpegD_Usac = 0x75736163, // 'usac' (Unified Speech and Audio Coding)
    AMR = 0x73616d72, // 'samr'
    AMRWideBand = 0x73617762, // 'sawb'
    Audible = 0x41554442,
    iLBC = 0x696c6263,
    DVIIntelIMA = 0x6d730011,
    MicrosoftGSM = 0x6d730031,
    AES3 = 0x61657333, // 'aes3'
    EnhancedAES3 = 0x65632d33, // 'ec-3'
    Flac = 0x666c6163, // 'flac' 
    LatmInLoas = 0x6c6f6173, // 'loas'
    Opus = 0x6f707573 // 'opus'
}