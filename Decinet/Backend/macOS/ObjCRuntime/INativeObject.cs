#nullable enable

using NativeHandle = System.IntPtr;

namespace Decinet.Backend.macOS.ObjCRuntime;

public interface INativeObject
{
    NativeHandle Handle { get; }
}