//
// AudioComponent.cs: AudioComponent wrapper class
//
// Author:
//   AKIHIRO Uehara (u-akihiro@reinforce-lab.com)
//
// Copyright 2010 Reinforce Lab.
// Copyright 2011, 2012 Xamarin Inc
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
using Decinet.Backend.macOS.CoreFoundation;
using Decinet.Backend.macOS.ObjCRuntime;

namespace Decinet.Backend.macOS.AudioUnit
{
    public class AudioComponent : DisposableObject
    {
        internal AudioComponent(IntPtr handle, bool owns)
            : base(handle, owns)
        {
        }

        public AudioUnit CreateAudioUnit()
        {
            return new AudioUnit(this);
        }

        public static AudioComponent? FindNextComponent(AudioComponent? cmp, ref AudioComponentDescription cd)
        {
            var handle = cmp.GetHandle();
            handle = AudioComponentFindNext(handle, ref cd);
            return handle != IntPtr.Zero ? new AudioComponent(handle, false) : null;
        }

        public static AudioComponent? FindComponent(ref AudioComponentDescription cd)
        {
            return FindNextComponent(null, ref cd);
        }

        public static AudioComponent? FindComponent(AudioTypeOutput output)
        {
            var cd = AudioComponentDescription.CreateOutput(output);
            return FindComponent(ref cd);
        }

        public static AudioComponent? FindComponent(AudioTypeMusicDevice musicDevice)
        {
            var cd = AudioComponentDescription.CreateMusicDevice(musicDevice);
            return FindComponent(ref cd);
        }

        public static AudioComponent? FindComponent(AudioTypeConverter conveter)
        {
            var cd = AudioComponentDescription.CreateConverter(conveter);
            return FindComponent(ref cd);
        }

        public static AudioComponent? FindComponent(AudioTypeEffect effect)
        {
            var cd = AudioComponentDescription.CreateEffect(effect);
            return FindComponent(ref cd);
        }

        public static AudioComponent? FindComponent(AudioTypeMixer mixer)
        {
            var cd = AudioComponentDescription.CreateMixer(mixer);
            return FindComponent(ref cd);
        }

        public static AudioComponent? FindComponent(AudioTypePanner panner)
        {
            var cd = AudioComponentDescription.CreatePanner(panner);
            return FindComponent(ref cd);
        }

        public static AudioComponent? FindComponent(AudioTypeGenerator generator)
        {
            var cd = AudioComponentDescription.CreateGenerator(generator);
            return FindComponent(ref cd);
        }

        [DllImport(Constants.AudioUnitLibrary)]
        private static extern IntPtr AudioComponentFindNext(IntPtr inComponent, ref AudioComponentDescription inDesc);

        [DllImport(Constants.AudioUnitLibrary, EntryPoint = "AudioComponentCopyName")]
        private static extern int /* OSStatus */ AudioComponentCopyName(IntPtr component, out IntPtr cfstr);

        public string? Name
        {
            get
            {
                if (AudioComponentCopyName(Handle, out var r) == 0)
                    return CfString.FromHandle(r);
                return null;
            }
        }

        [DllImport(Constants.AudioUnitLibrary)]
        private static extern int /* OSStatus */
            AudioComponentGetDescription(IntPtr component, out AudioComponentDescription desc);

        public AudioComponentDescription? Description
        {
            get
            {
                if (AudioComponentGetDescription(Handle, out var desc) == 0)
                    return desc;

                return null;
            }
        }

        [DllImport(Constants.AudioUnitLibrary)]
        private static extern int /* OSStatus */
            AudioComponentGetVersion(IntPtr component, out int /* UInt32* */ version);

        public Version? Version
        {
            get
            {
                if (AudioComponentGetVersion(Handle, out var ret) == 0)
                    return new Version(ret >> 16, (ret >> 8) & 0xff, ret & 0xff);

                return null;
            }
        }

        [DllImport(Constants.AudioUnitLibrary)]
        private static extern double AudioComponentGetLastActiveTime(IntPtr comp);

        public double LastActiveTime => AudioComponentGetLastActiveTime(Handle);
    }
}