using System.Runtime.InteropServices;

namespace Decinet.Backend.macOS.AudioUnit;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct AudioUnitParameterInfoNative // AudioUnitParameterInfo in Obj-C
{
    private fixed byte /* char[52] */
        name[52]; // unused

    public IntPtr /* CFStringRef */
        UnitName;

    public AudioUnitClumpID /* UInt32 */
        ClumpID;

    public IntPtr /* CFStringRef */
        NameString;

    public AudioUnitParameterUnit /* AudioUnitParameterUnit */
        Unit;

    public float /* AudioUnitParameterValue = Float32 */
        MinValue;

    public float /* AudioUnitParameterValue = Float32 */
        MaxValue;

    public float /* AudioUnitParameterValue = Float32 */
        DefaultValue;

    public AudioUnitParameterFlag /* UInt32 */
        Flags;
}