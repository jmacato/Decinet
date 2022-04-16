namespace Decinet.Backend.macOS.AudioUnit;

public enum AudioTypeOutput
{
    Generic = 0x67656e72, // 'genr'
    HAL = 0x6168616c, // 'ahal'
    Default = 0x64656620, // 'def'
    System = 0x73797320, // 'sys'
    Remote = 0x72696f63, // 'rioc'
    VoiceProcessingIO = 0x7670696f // 'vpio'
}