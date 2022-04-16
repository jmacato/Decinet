// 
// CFUrl.cs: Implements the managed CFUrl
//
// Authors:
//     Miguel de Icaza
//     Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2009 Novell, Inc
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#nullable enable

using System.Runtime.InteropServices;
using Decinet.Backend.macOS.ObjCRuntime;
using NativeHandle = System.IntPtr;

namespace Decinet.Backend.macOS.CoreFoundation
{
    public class CFUrl : NativeObject
    {
        [DllImport(Constants.CoreFoundationLibrary)]
        private static extern /* CFURLRef */ IntPtr CFURLCreateWithFileSystemPath( /* CFAllocatorRef */
            IntPtr allocator,
            /* CFStringRef */ IntPtr filePath,
            /* CFURLPathStyle */ nint pathStyle,
            /* Boolean */ [MarshalAs(UnmanagedType.I1)] bool isDirectory);

        private CFUrl(NativeHandle handle, bool owns)
            : base(handle, owns)
        {
        }

        public static CFUrl? FromFile(string filename)
        {
            if (filename is null)
                throw new ArgumentNullException(nameof(filename));
            var strHandle = CfString.CreateNative(filename);
            try
            {
                var handle =
                    CFURLCreateWithFileSystemPath(IntPtr.Zero, strHandle, (nint) (long) CFUrlPathStyle.POSIX, false);
                if (handle == IntPtr.Zero)
                    return null;
                return new CFUrl(handle, true);
            }
            finally
            {
                CfString.ReleaseNative(strHandle);
            }
        }

        [DllImport(Constants.CoreFoundationLibrary)]
        private static extern /* CFURLRef */ IntPtr CFURLCreateWithString( /* CFAllocatorRef */ IntPtr allocator,
            /* CFStringRef */ IntPtr URLString,
            /* CFStringRef */ IntPtr baseURL);

        public static CFUrl? FromUrlString(string url, CFUrl? baseurl)
        {
            if (url is null)
                throw new ArgumentNullException(nameof(url));
            var strHandle = CfString.CreateNative(url);
            try
            {
                return FromStringHandle(strHandle, baseurl);
            }
            finally
            {
                CfString.ReleaseNative(strHandle);
            }
        }

        internal static CFUrl? FromStringHandle(IntPtr cfstringHandle, CFUrl? baseurl)
        {
            var handle = CFURLCreateWithString(IntPtr.Zero, cfstringHandle, baseurl.GetHandle());
            if (handle == IntPtr.Zero)
                return null;
            return new CFUrl(handle, true);
        }

        [DllImport(Constants.CoreFoundationLibrary)]
        private static extern /* CFStringRef */ IntPtr CFURLGetString( /* CFURLRef */ IntPtr anURL);

        public override string? ToString()
        {
            return CfString.FromHandle(CFURLGetString(Handle));
        }

        [DllImport(Constants.CoreFoundationLibrary)]
        private static extern /* CFStringRef */ IntPtr CFURLCopyFileSystemPath( /* CFURLRef */ IntPtr anURL,
            /* CFURLPathStyle */ nint style);

        public string? FileSystemPath => GetFileSystemPath(Handle);

        public static string? GetFileSystemPath(IntPtr hcfurl)
        {
            return CfString.FromHandle(CFURLCopyFileSystemPath(hcfurl, 0), true);
        }

        [DllImport(Constants.CoreFoundationLibrary)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern /* Boolean */ bool CFURLIsFileReferenceURL( /* CFURLRef */ IntPtr url);

        public bool IsFileReference => CFURLIsFileReferenceURL(Handle);

        [DllImport(Constants.CoreFoundationLibrary, EntryPoint = "CFURLGetTypeID")]
        public static extern /* CFTypeID */ nint GetTypeID();
    }
}