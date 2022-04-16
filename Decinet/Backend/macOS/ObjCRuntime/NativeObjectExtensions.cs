namespace Decinet.Backend.macOS.ObjCRuntime;

public static class NativeObjectExtensions
{
    // help to avoid the (too common pattern)
    // 	var p = x == null ? IntPtr.Zero : x.Handle;
    public static IntPtr GetHandle(this INativeObject? self)
    {
        return self is null ? IntPtr.Zero : self.Handle;
    }

    public static IntPtr GetNonNullHandle(this INativeObject? self, string argumentName)
    {
        if (self is null)
            ThrowHelper.ThrowArgumentNullException(argumentName);
        if (self.Handle == IntPtr.Zero)
            ThrowHelper.ThrowObjectDisposedException(self);
        return self.Handle;
    }

    public static IntPtr GetCheckedHandle(this INativeObject? self)
    {
        if (self is null)
            ThrowHelper.ThrowArgumentNullException(nameof(self));
        
        var h = self.Handle;
        if (h == IntPtr.Zero)
            ThrowHelper.ThrowObjectDisposedException(self);

        return h;
    }
}