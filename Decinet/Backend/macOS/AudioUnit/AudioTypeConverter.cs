namespace Decinet.Backend.macOS.AudioUnit;

public enum AudioTypeConverter
{
    // OSType in AudioComponentDescription
    AU = 0x636f6e76, // 'conv'
    Varispeed = 0x76617269, // 'vari'
    DeferredRenderer = 0x64656672, // 'defr'
    Splitter = 0x73706c74, // 'splt'
    Merger = 0x6d657267, // 'merg'
    NewTimePitch = 0x6e757470, // 'nutp'
    AUiPodTimeOther = 0x6970746f, // 'ipto
    RoundTripAAC = 0x72616163, // 'raac'
    MultiSplitter = 0x6d73706c, // 'mspl'
#if MONOMAC
		TimePitch = 0x746d7074, // 'tmpt'
#else
#if NETXXX
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if TVOS
		[Obsolete ("Starting with tvos13.0 use 'AudioTypeConverter.NewTimePitch' instead.", DiagnosticId =
 "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'AudioTypeConverter.NewTimePitch' instead.", DiagnosticId =
 "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
    ////[Deprecated (PlatformName.iOS, 13,0, message: "Use 'AudioTypeConverter.NewTimePitch' instead.")]
    ////[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'AudioTypeConverter.NewTimePitch' instead.")]
    ////[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'AudioTypeConverter.NewTimePitch' instead.")]
#endif
    AUiPodTime = 0x6970746d, // 'iptm'
#endif
}