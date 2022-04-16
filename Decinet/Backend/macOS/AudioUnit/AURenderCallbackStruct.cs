using System.Runtime.InteropServices;

namespace Decinet.Backend.macOS.AudioUnit;

[StructLayout(LayoutKind.Sequential)]
internal struct AURenderCallbackStruct
{
    public Delegate Proc;
    public IntPtr ProcRefCon;
}