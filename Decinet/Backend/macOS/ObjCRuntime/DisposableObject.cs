//
// DisposableObject.cs: A base class for many native data types without assuming any particular lifecycle.

// Authors:
//   Rolf Bjarne Kvinge
//
// Copyright 2021 Microsoft Corp
//

#nullable enable
using NativeHandle = System.IntPtr;

namespace Decinet.Backend.macOS.ObjCRuntime;

//
// The DisposableObject class is intended to be a base class for many native data
// data types, without assuming any particular lifecycle (might be reference counted,
// might not be).
//
// It provides the common boilerplate for this kind of objects and the Dispose
// pattern.
//
public abstract class DisposableObject : INativeObject, IDisposable
{
    private IntPtr _handle;

    public IntPtr Handle
    {
        get => _handle;
        protected init => InitializeHandle(value);
    }

    protected bool Owns { get; }

    protected DisposableObject()
    {
    }

    protected DisposableObject(NativeHandle handle, bool owns)
        : this(handle, owns, true)
    {
    }

    protected DisposableObject(NativeHandle handle, bool owns, bool verify)
    {
        InitializeHandle(handle, verify);
        Owns = owns;
    }

    ~DisposableObject()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _handle = NativeHandle.Zero;
    }

    private void InitializeHandle(NativeHandle handle, bool verify)
    {
        if (verify && handle == NativeHandle.Zero)
            throw new Exception(
                $"Could not initialize an instance of the type '{GetType().FullName}': handle is null..");
        _handle = handle;
    }

    protected virtual void InitializeHandle(NativeHandle handle)
    {
        InitializeHandle(handle, true);
    }

    public NativeHandle GetCheckedHandle()
    {
        var h = _handle;
        if (h == NativeHandle.Zero)
            ThrowHelper.ThrowObjectDisposedException(this);
        return h;
    }
}