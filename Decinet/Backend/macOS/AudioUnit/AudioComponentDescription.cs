//
// AudioComponentDescription.cs: AudioComponentDescription wrapper class
//
// Author:
//   AKIHIRO Uehara (u-akihiro@reinforce-lab.com)
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2010 Reinforce Lab.
// Copyright 2010 Novell, Inc
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

// using AudioToolbox;

namespace Decinet.Backend.macOS.AudioUnit;

#if NETXXX
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
[StructLayout(LayoutKind.Sequential)]
public struct AudioComponentDescription
{
    [MarshalAs(UnmanagedType.U4)] public AudioComponentType ComponentType;

    [MarshalAs(UnmanagedType.U4)]
// #if NETXXX && !COREBUILD
    public AudioUnitSubType ComponentSubType;
// #else
// 		public int ComponentSubType;
// #endif

    [MarshalAs(UnmanagedType.U4)] public AudioComponentManufacturerType ComponentManufacturer;

    public AudioComponentFlag ComponentFlags;
    public int ComponentFlagsMask;

    internal AudioComponentDescription(AudioComponentType type, int subType)
    {
        ComponentType = type;
        ComponentSubType = (AudioUnitSubType) subType;
        ComponentManufacturer = AudioComponentManufacturerType.Apple;
        ComponentFlags = (AudioComponentFlag) 0;
        ComponentFlagsMask = 0;
    }

    public static AudioComponentDescription CreateGeneric(AudioComponentType type, int subType)
    {
        return new AudioComponentDescription(type, subType);
    }

    public static AudioComponentDescription CreateOutput(AudioTypeOutput outputType)
    {
        return new AudioComponentDescription(AudioComponentType.Output, (int) outputType);
    }

    public static AudioComponentDescription CreateMusicDevice(AudioTypeMusicDevice musicDevice)
    {
        return new AudioComponentDescription(AudioComponentType.MusicDevice, (int) musicDevice);
    }

    public static AudioComponentDescription CreateConverter(AudioTypeConverter converter)
    {
        return new AudioComponentDescription(AudioComponentType.FormatConverter, (int) converter);
    }

    public static AudioComponentDescription CreateEffect(AudioTypeEffect effect)
    {
        return new AudioComponentDescription(AudioComponentType.Effect, (int) effect);
    }

    public static AudioComponentDescription CreateMixer(AudioTypeMixer mixer)
    {
        return new AudioComponentDescription(AudioComponentType.Mixer, (int) mixer);
    }

    public static AudioComponentDescription CreatePanner(AudioTypePanner panner)
    {
        return new AudioComponentDescription(AudioComponentType.Panner, (int) panner);
    }

    public static AudioComponentDescription CreateGenerator(AudioTypeGenerator generator)
    {
        return new AudioComponentDescription(AudioComponentType.Generator, (int) generator);
    }

    public override string ToString()
    {
        const string fmt = "[componentType={0}, subType={1}]";

        return ComponentType switch
        {
            AudioComponentType.Output => string.Format(fmt, ComponentType, (AudioTypeOutput) ComponentSubType),
            AudioComponentType.MusicDevice =>
                string.Format(fmt, ComponentType, (AudioTypeMusicDevice) ComponentSubType),
            AudioComponentType.FormatConverter => string.Format(fmt, ComponentType,
                (AudioTypeConverter) ComponentSubType),
            AudioComponentType.Effect => string.Format(fmt, ComponentType, (AudioTypeEffect) ComponentSubType),
            AudioComponentType.Mixer => string.Format(fmt, ComponentType, (AudioTypeMixer) ComponentSubType),
            AudioComponentType.Panner => string.Format(fmt, ComponentType, (AudioTypePanner) ComponentSubType),
            AudioComponentType.Generator => string.Format(fmt, ComponentType, (AudioTypeGenerator) ComponentSubType),
            _ => string.Format(fmt, ComponentType, ComponentSubType)
        };
    }
}