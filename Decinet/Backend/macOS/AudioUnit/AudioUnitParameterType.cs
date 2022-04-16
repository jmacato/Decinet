namespace Decinet.Backend.macOS.AudioUnit;

public enum AudioUnitParameterType // UInt32 in AudioUnitParameterInfo
{
    // AUMixer3D unit
    Mixer3DAzimuth = 0,
    Mixer3DElevation = 1,
    Mixer3DDistance = 2,
    Mixer3DGain = 3,
    Mixer3DPlaybackRate = 4,
#if MONOMAC
		Mixer3DReverbBlend = 5,
		Mixer3DGlobalReverbGain = 6,
		Mixer3DOcclusionAttenuation = 7,
		Mixer3DObstructionAttenuation = 8,
		Mixer3DMinGain = 9,
		Mixer3DMaxGain = 10,
		Mixer3DPreAveragePower = 1000,
		Mixer3DPrePeakHoldLevel = 2000,
		Mixer3DPostAveragePower = 3000,
		Mixer3DPostPeakHoldLevel = 4000,
#else
    Mixer3DEnable = 5,
    Mixer3DMinGain = 6,
    Mixer3DMaxGain = 7,
    Mixer3DReverbBlend = 8,
    Mixer3DGlobalReverbGain = 9,
    Mixer3DOcclusionAttenuation = 10,
    Mixer3DObstructionAttenuation = 11,
#endif

    // AUSpatialMixer unit
    SpatialAzimuth = 0,
    SpatialElevation = 1,
    SpatialDistance = 2,
    SpatialGain = 3,
    SpatialPlaybackRate = 4,
    SpatialEnable = 5,
    SpatialMinGain = 6,
    SpatialMaxGain = 7,
    SpatialReverbBlend = 8,
    SpatialGlobalReverbGain = 9,
    SpatialOcclusionAttenuation = 10,
    SpatialObstructionAttenuation = 11,

    // Reverb applicable to the 3DMixer or AUSpatialMixer
    ReverbFilterFrequency = 14,
    ReverbFilterBandwidth = 15,
    ReverbFilterGain = 16,

    ////[iOS (8, 0)]
    ReverbFilterType = 17,

    ////[iOS (8, 0)]
    ReverbFilterEnable = 18,

    // AUMultiChannelMixer
    MultiChannelMixerVolume = 0,
    MultiChannelMixerEnable = 1,
    MultiChannelMixerPan = 2,

    // AUMatrixMixer unit
    MatrixMixerVolume = 0,
    MatrixMixerEnable = 1,

    // AudioDeviceOutput, DefaultOutputUnit, and SystemOutputUnit units
    HALOutputVolume = 14,

    // AUTimePitch, AUTimePitch (offline), AUPitch units
    TimePitchRate = 0,
#if MONOMAC
		TimePitchPitch = 1,
		TimePitchEffectBlend = 2,
#endif

    // AUNewTimePitch
    NewTimePitchRate = 0,
    NewTimePitchPitch = 1,
    NewTimePitchOverlap = 4,
    NewTimePitchEnablePeakLocking = 6,

    // AUSampler unit
    AUSamplerGain = 900,
    AUSamplerCoarseTuning = 901,
    AUSamplerFineTuning = 902,
    AUSamplerPan = 903,

    // AUBandpass
    BandpassCenterFrequency = 0,
    BandpassBandwidth = 1,

    // AUHipass
    HipassCutoffFrequency = 0,
    HipassResonance = 1,

    // AULowpass
    LowPassCutoffFrequency = 0,
    LowPassResonance = 1,

    // AUHighShelfFilter
    HighShelfCutOffFrequency = 0,
    HighShelfGain = 1,

    // AULowShelfFilter
    AULowShelfCutoffFrequency = 0,
    AULowShelfGain = 1,

    // [Obsoleted (PlatformName.iOS, 7, 0)]
    // AUDCFilterDecayTime					= 0,

    // AUParametricEQ
    ParametricEQCenterFreq = 0,
    ParametricEQQ = 1,
    ParametricEQGain = 2,

    // AUPeakLimiter
    LimiterAttackTime = 0,
    LimiterDecayTime = 1,
    LimiterPreGain = 2,

    // AUDynamicsProcessor
    DynamicsProcessorThreshold = 0,
    DynamicsProcessorHeadRoom = 1,
    DynamicsProcessorExpansionRatio = 2,
    DynamicsProcessorExpansionThreshold = 3,
    DynamicsProcessorAttackTime = 4,
    DynamicsProcessorReleaseTime = 5,

    ////[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'DynamicsProcessorOverallGain' instead.")]
    ////[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'DynamicsProcessorOverallGain' instead.")]
    ////[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'DynamicsProcessorOverallGain' instead.")]
    ////[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'DynamicsProcessorOverallGain' instead.")]
    DynamicsProcessorMasterGain = 6,

    ////[iOS (15,0), TV (15,0), Mac (12,0), MacCatalyst (15,0)]
    DynamicsProcessorOverallGain = 6,
    DynamicsProcessorCompressionAmount = 1000,
    DynamicsProcessorInputAmplitude = 2000,
    DynamicsProcessorOutputAmplitude = 3000,

    // AUVarispeed
    VarispeedPlaybackRate = 0,
    VarispeedPlaybackCents = 1,

    // Distortion unit 
    DistortionDelay = 0,
    DistortionDecay = 1,
    DistortionDelayMix = 2,
    DistortionDecimation = 3,
    DistortionRounding = 4,
    DistortionDecimationMix = 5,
    DistortionLinearTerm = 6,
    DistortionSquaredTerm = 7,
    DistortionCubicTerm = 8,
    DistortionPolynomialMix = 9,
    DistortionRingModFreq1 = 10,
    DistortionRingModFreq2 = 11,
    DistortionRingModBalance = 12,
    DistortionRingModMix = 13,
    DistortionSoftClipGain = 14,
    DistortionFinalMix = 15,

    // AUDelay
    DelayWetDryMix = 0,
    DelayTime = 1,
    DelayFeedback = 2,
    DelayLopassCutoff = 3,

    // AUNBandEQ
    AUNBandEQGlobalGain = 0,
    AUNBandEQBypassBand = 1000,
    AUNBandEQFilterType = 2000,
    AUNBandEQFrequency = 3000,
    AUNBandEQGain = 4000,
    AUNBandEQBandwidth = 5000,

    // AURandomUnit
    RandomBoundA = 0,
    RandomBoundB = 1,
    RandomCurve = 2,

#if !MONOMAC
    // iOS reverb
    Reverb2DryWetMix = 0,
    Reverb2Gain = 1,
    Reverb2MinDelayTime = 2,
    Reverb2MaxDelayTime = 3,
    Reverb2DecayTimeAt0Hz = 4,
    Reverb2DecayTimeAtNyquist = 5,
    Reverb2RandomizeReflections = 6,
#endif

    // RoundTripAAC
    RoundTripAacFormat = 0,
    RoundTripAacEncodingStrategy = 1,
    RoundTripAacRateOrQuality = 2,

    // Spacial Mixer
    SpacialMixerAzimuth = 0,
    Elevation = 1,
    Distance = 2,
    Gain = 3,
    PlaybackRate = 4,
    Enable = 5,
    MinGain = 6,
    MaxGain = 7,
    ReverbBlend = 8,
    GlobalReverbGain = 9,
    OcclussionAttenuation = 10,
    ObstructionAttenuation = 11
}