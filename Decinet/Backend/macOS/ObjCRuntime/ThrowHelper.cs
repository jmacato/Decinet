

#nullable enable

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

// the linker will remove the attributes
namespace Decinet.Backend.macOS.ObjCRuntime;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowArgumentException(string argumentName)
    {
        throw new ArgumentException(argumentName);
    }

    [DoesNotReturn]
    public static void ThrowArgumentException(string argumentName, string message)
    {
        throw new ArgumentException(message, argumentName);
    }

    [DoesNotReturn]
    public static void ThrowArgumentNullException(string argumentName)
    {
        throw new ArgumentNullException(argumentName);
    }

    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException(string argumentName, string message)
    {
        throw new ArgumentOutOfRangeException(argumentName, message);
    }

    [DoesNotReturn]
    public static void ThrowObjectDisposedException(object? o)
    {
        throw new ObjectDisposedException(o.GetType().ToString());
    }
}