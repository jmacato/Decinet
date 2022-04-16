namespace Decinet.Backend.macOS.AudioUnit;

public enum AudioTypeEffect
{
    PeakLimiter = 0x6c6d7472, // 'lmtr'
    DynamicsProcessor = 0x64636d70, // 'dcmp'
    LowPassFilter = 0x6c706173, // 'lpas'
    HighPassFilter = 0x68706173, // 'hpas'
    HighShelfFilter = 0x68736866, // 'hshf'
    LowShelfFilter = 0x6c736866, // 'lshf' 
    ParametricEQ = 0x706d6571, // 'pmeq'
    Delay = 0x64656c79, // 'dely'
    SampleDelay = 0x73646c79, // 'sdly'
    Distortion = 0x64697374, // 'dist'
    BandPassFilter = 0x62706173, // 'bpas'
    GraphicEQ = 0x67726571, // 'greq'
    MultiBandCompressor = 0x6d636d70, // 'mcmp'
    MatrixReverb = 0x6d726576, // 'mrev'
    Pitch = 0x70697463, // 'pitc'
    AUFilter = 0x66696c74, // 'filt'
    NetSend = 0x6e736e64, // 'nsnd'
    RogerBeep = 0x726f6772, // 'rogr' 
    Reverb2 = 0x72766232, // 'rvb2'
    NBandEq = 0x6e626571 // 'nbeq'
}