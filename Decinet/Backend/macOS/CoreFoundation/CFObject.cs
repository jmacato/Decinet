using System.Runtime.InteropServices;
using Decinet.Backend.macOS.ObjCRuntime;

namespace Decinet.Backend.macOS.CoreFoundation;

public static class CFObject
{
    [DllImport(Constants.CoreFoundationLibrary)]
    internal static extern void CFRelease(IntPtr obj);

    [DllImport(Constants.CoreFoundationLibrary)]
    internal static extern IntPtr CFRetain(IntPtr obj);
}