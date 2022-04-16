using System.Runtime.InteropServices;

namespace Decinet.Backend.macOS.AudioUnit;

[StructLayout(LayoutKind.Sequential)]
internal struct AudioUnitConnection
{
    public IntPtr SourceAudioUnit;

    public uint /* UInt32 */
        SourceOutputNumber;

    public uint /* UInt32 */
        DestInputNumber;
}