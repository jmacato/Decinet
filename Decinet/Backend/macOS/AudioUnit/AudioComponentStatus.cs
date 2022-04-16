namespace Decinet.Backend.macOS.AudioUnit;

public enum AudioComponentStatus
{
    OK = 0,
    DuplicateDescription = -66752,
    UnsupportedType = -66751,
    TooManyInstances = -66750,
    InstanceInvalidated = -66749,
    NotPermitted = -66748,
    InitializationTimedOut = -66747,
    InvalidFormat = -66746,
    RenderTimeout = -66745
}