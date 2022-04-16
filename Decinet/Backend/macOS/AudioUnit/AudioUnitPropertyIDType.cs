namespace Decinet.Backend.macOS.AudioUnit;

[Obsolete("Please use the strongly typed properties instead.")]
internal enum AudioUnitPropertyIDType
{
    // Audio Unit Properties
    ClassInfo = 0,
    MakeConnection = 1,
    SampleRate = 2,
    ParameterList = 3,
    ParameterInfo = 4,
    CPULoad = 6,
    StreamFormat = 8,
    ElementCount = 11,
    Latency = 12,
    SupportedNumChannels = 13,
    MaximumFramesPerSlice = 14,
    ParameterValueStrings = 16,
    AudioChannelLayout = 19,
    TailTime = 20,
    BypassEffect = 21,
    LastRenderError = 22,
    SetRenderCallback = 23,
    FactoryPresets = 24,
    RenderQuality = 26,
    HostCallbacks = 27,
    InPlaceProcessing = 29,
    ElementName = 30,
    SupportedChannelLayoutTags = 32,
    PresentPreset = 36,
    DependentParameters = 45,
    InputSampleInOutput = 49,
    ShouldAllocateBuffer = 51,
    FrequencyResponse = 52,
    ParameterHistoryInfo = 53,
    Nickname = 54,
    OfflineRender = 37,

    ParameterIDName = 34,

    ParameterStringFromValue = 33,
    ParameterClumpName = 35,

    ParameterValueFromString = 38,
    ContextName = 25,
    PresentationLatency = 40,
    ClassInfoFromDocument = 50,
    RequestViewController = 56,
    ParametersForOverview = 57,

    SupportsMpe = 58,

    LastRenderSampleTime = 61,

    LoadedOutOfProcess = 62,

    MIDIOutputEventListCallback = 63,

    AudioUnitMIDIProtocol = 64,

    HostMIDIProtocol = 65,

    FastDispatch = 5,
    SetExternalBuffer = 15,
    GetUIComponentList = 18,
    CocoaUI = 31,
    IconLocation = 39,
    AUHostIdentifier = 46,
    MIDIOutputCallbackInfo = 47,
    MIDIOutputCallback = 48,
    RemoteControlEventListener = 100,
    IsInterAppConnected = 101,
    PeerURL = 102,

    // Output Unit
    IsRunning = 2001,

    // Music Effects and Instruments
    AllParameterMIDIMappings = 41,
    AddParameterMIDIMapping = 42,
    RemoveParameterMIDIMapping = 43,
    HotMapParameterMIDIMapping = 44,

    // Music Device
    MIDIXMLNames = 1006,
    PartGroup = 1010,
    DualSchedulingMode = 1013,
    SupportsStartStopNote = 1014,

    // Offline Unit
    InputSize = 3020,
    OutputSize = 3021,
    StartOffset = 3022,
    PreflightRequirements = 3023,
    PreflightName = 3024,

    // Translation Service
    FromPlugin = 4000,
    OldAutomation = 4001,


    // Apple Specific Properties
    // AUConverter
    SampleRateConverterComplexity = 3014,

    // AUHAL and device units
    CurrentDevice = 2000,
    ChannelMap = 2002, // this will also work with AUConverter
    EnableIO = 2003,
    StartTime = 2004,
    SetInputCallback = 2005,
    HasIO = 2006,
    StartTimestampsAtZero = 2007, // this will also work with AUConverter

    MIDICallbacks = 2010,
    HostReceivesRemoteControlEvents = 2011,
    RemoteControlToHost = 2012,
    HostTransportState = 2013,
    NodeComponentDescription = 2014,

    // AUVoiceProcessing unit
    BypassVoiceProcessing = 2100,
    VoiceProcessingEnableAGC = 2101,
    MuteOutput = 2104,

    ////[iOS (15, 0), MacCatalyst (15, 0), NoMac, NoWatch, NoTV]
    MutedSpeechActivityEventListener = 2106,

    // AUNBandEQ unit
    NumberOfBands = 2200,
    MaxNumberOfBands = 2201,
    BiquadCoefficients = 2203,

    // Mixers
    // General mixers
    MeteringMode = 3007,

    // Matrix Mixer
    MatrixLevels = 3006,
    MatrixDimensions = 3009,
    MeterClipping = 3011,

    ////[iOS (10,0), Mac (10,12)]
    InputAnchorTimeStamp = 3016,

    // SpatialMixer
    ReverbRoomType = 10,
    UsesInternalReverb = 1005,
    SpatializationAlgorithm = 3000,

    ////[Deprecated (PlatformName.iOS, 9, 0)]
    DistanceParams = 3010,

    ////[Deprecated (PlatformName.iOS, 9, 0)]
    AttenuationCurve = 3013,

    ////[Deprecated (PlatformName.iOS, 9, 0)]
    RenderingFlags = 3003,

    // AUScheduledSoundPlayer
    ScheduleAudioSlice = 3300,
    ScheduleStartTimeStamp = 3301,
    CurrentPlayTime = 3302,

    // AUAudioFilePlayer
    ScheduledFileIDs = 3310,
    ScheduledFileRegion = 3311,
    ScheduledFilePrime = 3312,
    ScheduledFileBufferSizeFrames = 3313,
    ScheduledFileNumberBuffers = 3314,

    // OS X-specific Music Device Properties
    SoundBankData = 1008,
    StreamFromDisk = 1011,
    SoundBankFSRef = 1012,


    // Music Device Properties
    InstrumentName = 1001,
    InstrumentNumber = 1004,

    // Music Device Properties used by DLSMusicDevice and AUMIDISynth
    InstrumentCount = 1000,
    BankName = 1007,
    SoundBankURL = 1100,

    // AUMIDISynth
    MidiSynthEnablePreload = 4119,

    // AUSampler
    LoadInstrument = 4102,
    LoadAudioFiles = 4101,

    // AUDeferredRenderer
    DeferredRendererPullSize = 3320,
    DeferredRendererExtraLatency = 3321,
    DeferredRendererWaitFrames = 3322,

    // AUNetReceive
    Hostname = 3511,
    NetReceivePassword = 3512,

    // AUNetSend
    PortNum = 3513,
    TransmissionFormat = 3514,
    TransmissionFormatIndex = 3515,
    ServiceName = 3516,
    Disconnect = 3517,
    NetSendPassword = 3518,
}