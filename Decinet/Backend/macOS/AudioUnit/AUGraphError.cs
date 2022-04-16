namespace Decinet.Backend.macOS.AudioUnit;

public enum AUGraphError // Implictly cast to OSType
{
    OK = 0,
    NodeNotFound = -10860,
    InvalidConnection = -10861,
    OutputNodeError = -10862,
    CannotDoInCurrentContext = -10863,
    InvalidAudioUnit = -10864,

    // Values returned & shared with other error enums
    FormatNotSupported = -10868,
    InvalidElement = -10877
}