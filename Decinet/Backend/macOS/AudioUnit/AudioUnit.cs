//
// AudioUnit.cs: AudioUnit wrapper class
//
// Authors:
//   AKIHIRO Uehara (u-akihiro@reinforce-lab.com)
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2010 Reinforce Lab.
// Copyright 2011-2013 Xamarin Inc
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

// Adding this warning disable since AudioUnitPropertyIDType is removed from public API but used internally
#pragma warning disable CS0618

using System.Runtime.InteropServices;
using Decinet.Backend.macOS.AudioToolbox;
using Decinet.Backend.macOS.CoreFoundation;
using Decinet.Backend.macOS.ObjCRuntime;

namespace Decinet.Backend.macOS.AudioUnit;

public class AudioUnit : DisposableObject
{
    private static readonly CallbackShared CreateRenderCallback = RenderCallbackImpl;
    private static readonly CallbackShared CreateInputCallback = InputCallbackImpl;

    private GCHandle gcHandle;
    private bool _isPlaying;

    private Dictionary<uint, RenderDelegate>? renderer;
    private Dictionary<uint, InputDelegate>? inputs;

    internal AudioUnit(IntPtr handle, bool owns)
        : base(handle, owns)
    {
    }

    private static IntPtr Create(AudioComponent component)
    {
        if (component is null)
            ThrowHelper.ThrowArgumentNullException(nameof(component));

        var err = AudioComponentInstanceNew(component.GetCheckedHandle(), out var handle);
        if (err != 0)
            throw new AudioUnitException(err);

        return handle;
    }

    public AudioUnit(AudioComponent component)
        : this(Create(component), true)
    {
    }

    public AudioComponent Component => new AudioComponent(AudioComponentInstanceGetComponent(Handle), false);

    public bool IsPlaying => _isPlaying;


    public AudioUnitStatus SetFormat(AudioStreamBasicDescription audioFormat, AudioUnitScopeType scope,
        uint audioUnitElement = 0)
    {
        return (AudioUnitStatus) AudioUnitSetProperty(Handle,
            AudioUnitPropertyIDType.StreamFormat,
            scope,
            audioUnitElement,
            ref audioFormat,
            (uint) Marshal.SizeOf(audioFormat));
    }

    public uint GetCurrentDevice(AudioUnitScopeType scope, uint audioUnitElement = 0)
    {
        uint device = 0;
        var size = Marshal.SizeOf(typeof(uint));
        var err = AudioUnitGetProperty(Handle,
            AudioUnitPropertyIDType.CurrentDevice,
            scope,
            audioUnitElement,
            ref device,
            ref size);
        if (err != 0)
            throw new AudioUnitException((int) err);
        return device;
    }

    public AudioUnitStatus SetCurrentDevice(uint inputDevice, AudioUnitScopeType scope, uint audioUnitElement = 0)
    {
        return AudioUnitSetProperty(Handle,
            AudioUnitPropertyIDType.CurrentDevice,
            scope,
            audioUnitElement,
            ref inputDevice,
            (uint) Marshal.SizeOf(inputDevice));
    }

    public AudioStreamBasicDescription GetAudioFormat(AudioUnitScopeType scope, uint audioUnitElement = 0)
    {
        var audioFormat = new AudioStreamBasicDescription();
        var size = (uint) Marshal.SizeOf(audioFormat);

        var err = AudioUnitGetProperty(Handle,
            AudioUnitPropertyIDType.StreamFormat,
            scope,
            audioUnitElement,
            ref audioFormat,
            ref size);
        if (err != 0)
            throw new AudioUnitException((int) err);

        return audioFormat;
    }

    public unsafe AudioUnitParameterInfo[]? GetParameterList(AudioUnitScopeType scope = AudioUnitScopeType.Global,
        uint audioUnitElement = 0)
    {
        if (AudioUnitGetPropertyInfo(Handle, AudioUnitPropertyIDType.ParameterList, scope, audioUnitElement,
                out var size, out _) != 0)
            return null;

        // Array of AudioUnitParameterID = UInt32
        var data = new uint [size / sizeof(uint)];
        fixed (uint* ptr = data)
        {
            if (AudioUnitGetProperty(Handle, AudioUnitPropertyIDType.ParameterList, scope, audioUnitElement, ptr,
                    ref size) != 0)
                return null;
        }

        var info = new AudioUnitParameterInfo [data.Length];
        size = (uint) sizeof(AudioUnitParameterInfoNative);

        for (var i = 0; i < data.Length; ++i)
        {
            var native = new AudioUnitParameterInfoNative();
            if (AudioUnitGetProperty(Handle, AudioUnitPropertyIDType.ParameterInfo, scope, data[i], ref native,
                    ref size) != 0)
                return null;

            info[i] = AudioUnitParameterInfo.Create(native, (AudioUnitParameterType) data[i]);
        }

        return info;
    }
 
    public AudioUnitStatus MakeConnection(AudioUnit sourceAudioUnit, uint sourceOutputNumber, uint destInputNumber)
    {
        var auc = new AudioUnitConnection
        {
            SourceAudioUnit = sourceAudioUnit.GetHandle(),
            SourceOutputNumber = sourceOutputNumber,
            DestInputNumber = destInputNumber
        };

        return AudioUnitSetProperty(Handle, AudioUnitPropertyIDType.MakeConnection, AudioUnitScopeType.Input, 0,
            ref auc, Marshal.SizeOf(typeof(AudioUnitConnection)));
    }

    public AudioUnitStatus SetEnableIO(bool enableIO, AudioUnitScopeType scope, uint audioUnitElement = 0)
    {
        // EnableIO: UInt32          
        var flag = enableIO ? (uint) 1 : 0;
        return AudioUnitSetProperty(Handle, AudioUnitPropertyIDType.EnableIO, scope, audioUnitElement, ref flag,
            sizeof(uint));
    }

    public AudioUnitStatus SetMaximumFramesPerSlice(uint value, AudioUnitScopeType scope, uint audioUnitElement = 0)
    {
        // MaximumFramesPerSlice: UInt32
        return AudioUnitSetProperty(Handle, AudioUnitPropertyIDType.MaximumFramesPerSlice, scope, audioUnitElement,
            ref value, sizeof(uint));
    }

    public uint GetMaximumFramesPerSlice(AudioUnitScopeType scope = AudioUnitScopeType.Global,
        uint audioUnitElement = 0)
    {
        // MaximumFramesPerSlice: UInt32
        uint value = 0;
        uint size = sizeof(uint);
        var res = AudioUnitGetProperty(Handle, AudioUnitPropertyIDType.MaximumFramesPerSlice, scope,
            audioUnitElement, ref value, ref size);

        if (res != 0)
            throw new AudioUnitException((int) res);

        return value;
    }

    public AudioUnitStatus SetElementCount(AudioUnitScopeType scope, uint count)
    {
        // ElementCount: UInt32
        return AudioUnitSetProperty(Handle, AudioUnitPropertyIDType.ElementCount, scope, 0, ref count,
            sizeof(uint));
    }

    public uint GetElementCount(AudioUnitScopeType scope)
    {
        // ElementCount: UInt32
        uint value = 0;
        uint size = sizeof(uint);
        var res = AudioUnitGetProperty(Handle, AudioUnitPropertyIDType.ElementCount, scope,
            0, ref value, ref size);

        if (res != 0)
            throw new AudioUnitException((int) res);

        return value;
    }

    public AudioUnitStatus SetSampleRate(double sampleRate, AudioUnitScopeType scope = AudioUnitScopeType.Output,
        uint audioUnitElement = 0)
    {
        // ElementCount: Float64
        return AudioUnitSetProperty(Handle, AudioUnitPropertyIDType.SampleRate, scope, 0, ref sampleRate,
            sizeof(double));
    }

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitGetProperty(IntPtr inUnit, AudioUnitPropertyIDType inID,
        AudioUnitScopeType inScope, uint inElement, ref double outData, ref uint ioDataSize);

    public double GetLatency()
    {
        uint size = sizeof(double);
        double latency = 0;
        var err = AudioUnitGetProperty(Handle, AudioUnitPropertyIDType.Latency, AudioUnitScopeType.Global, 0,
            ref latency, ref size);
        if (err != 0)
            throw new AudioUnitException((int) err);
        return latency;
    }

    public AudioUnitStatus SetRenderCallback(RenderDelegate renderDelegate,
        AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
    {
        if (renderer is null)
            Interlocked.CompareExchange(ref renderer, new Dictionary<uint, RenderDelegate>(), null);

        renderer[audioUnitElement] = renderDelegate;

        if (!gcHandle.IsAllocated)
            gcHandle = GCHandle.Alloc(this);

        var cb = new AURenderCallbackStruct();
        cb.Proc = CreateRenderCallback;
        cb.ProcRefCon = GCHandle.ToIntPtr(gcHandle);
        return AudioUnitSetProperty(Handle, AudioUnitPropertyIDType.SetRenderCallback, scope, audioUnitElement,
            ref cb, Marshal.SizeOf(cb));
    }

    private static AudioUnitStatus RenderCallbackImpl(IntPtr clientData, ref AudioUnitRenderActionFlags actionFlags,
        ref AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, IntPtr data)
    {
        var gch = GCHandle.FromIntPtr(clientData);
        var au = (AudioUnit?) gch.Target;
        var renderer = au?.renderer;

        if (renderer is null)
            return AudioUnitStatus.Uninitialized;

        if (!renderer.TryGetValue(busNumber, out var render))
            return AudioUnitStatus.Uninitialized;

        using var buffers = new AudioBuffers(data);
        return render(actionFlags, timeStamp, busNumber, numberFrames, buffers);
    }

    public AudioUnitStatus SetInputCallback(InputDelegate inputDelegate,
        AudioUnitScopeType scope = AudioUnitScopeType.Global, uint audioUnitElement = 0)
    {
        if (inputs is null)
            Interlocked.CompareExchange(ref inputs, new Dictionary<uint, InputDelegate>(), null);

        inputs[audioUnitElement] = inputDelegate;

        if (!gcHandle.IsAllocated)
            gcHandle = GCHandle.Alloc(this);

        var cb = new AURenderCallbackStruct();
        cb.Proc = CreateInputCallback;
        cb.ProcRefCon = GCHandle.ToIntPtr(gcHandle);
        return AudioUnitSetProperty(Handle, AudioUnitPropertyIDType.SetInputCallback, scope, audioUnitElement,
            ref cb, Marshal.SizeOf(cb));
    }

    // // [MonoPInvokeCallback (typeof (CallbackShared))]
    private static AudioUnitStatus InputCallbackImpl(IntPtr clientData, ref AudioUnitRenderActionFlags actionFlags,
        ref AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, IntPtr data)
    {
        var gch = GCHandle.FromIntPtr(clientData);
        var au = gch.Target as AudioUnit;
        if (au is null)
            return AudioUnitStatus.Uninitialized;

        var inputs = au.inputs;
        if (inputs is null)
            return AudioUnitStatus.Uninitialized;

        if (!inputs.TryGetValue(busNumber, out var input))
            return AudioUnitStatus.Uninitialized;

        return input(actionFlags, timeStamp, busNumber, numberFrames, au);
    }

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioComponentStatus AudioOutputUnitPublish(AudioComponentDescription inDesc,
        IntPtr /* CFStringRef */ inName, uint /* UInt32 */ inVersion, IntPtr /* AudioUnit */ inOutputUnit);

    public AudioComponentStatus AudioOutputUnitPublish(AudioComponentDescription description, string name,
        uint version = 1)
    {
        if (name is null)
            ThrowHelper.ThrowArgumentNullException(nameof(name));

        var nameHandle = CfString.CreateNative(name);
        try
        {
            return AudioOutputUnitPublish(description, nameHandle, version, Handle);
        }
        finally
        {
            CfString.ReleaseNative(nameHandle);
        }
    }

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern IntPtr AudioOutputUnitGetHostIcon(IntPtr /* AudioUnit */ au,
        float /* float */ desiredPointSize);


    public int Initialize()
    {
        return (int) AudioUnitInitialize(Handle);
    }

    public int Uninitialize()
    {
        return (int) AudioUnitUninitialize(Handle);
    }

    public void Start()
    {
        if (!_isPlaying)
        {
            AudioOutputUnitStart(Handle);
            _isPlaying = true;
        }
    }

    public void Stop()
    {
        if (_isPlaying)
        {
            AudioOutputUnitStop(Handle);
            _isPlaying = false;
        }
    }

    public AudioUnitStatus Render(ref AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp,
        uint busNumber, uint numberFrames, AudioBuffers data)
    {
        if ((IntPtr) data == IntPtr.Zero)
            ThrowHelper.ThrowArgumentNullException(nameof(data));
        return AudioUnitRender(Handle, ref actionFlags, ref timeStamp, busNumber, numberFrames, (IntPtr) data);
    }

    public AudioUnitStatus SetParameter(AudioUnitParameterType type, float value, AudioUnitScopeType scope,
        uint audioUnitElement = 0)
    {
        return AudioUnitSetParameter(Handle, type, scope, audioUnitElement, value, 0);
    }

    public AudioUnitStatus ScheduleParameter(AudioUnitParameterEvent inParameterEvent, uint inNumParamEvents)
    {
        return AudioUnitScheduleParameters(Handle, inParameterEvent, inNumParamEvents);
    }

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern int AudioComponentInstanceDispose(IntPtr inInstance);

    protected override void Dispose(bool disposing)
    {
        if (Handle != IntPtr.Zero && Owns)
        {
            Stop();
            AudioUnitUninitialize(Handle);
            AudioComponentInstanceDispose(Handle);
        }

        if (gcHandle.IsAllocated)
            gcHandle.Free();
        base.Dispose(disposing);
    }

    [DllImport(Constants.AudioUnitLibrary, EntryPoint = "AudioComponentInstanceNew")]
    private static extern AudioUnitStatus AudioComponentInstanceNew(IntPtr inComponent, out IntPtr inDesc);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern IntPtr AudioComponentInstanceGetComponent(IntPtr inComponent);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitInitialize(IntPtr inUnit);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitUninitialize(IntPtr inUnit);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioOutputUnitStart(IntPtr ci);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioOutputUnitStop(IntPtr ci);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitRender(IntPtr inUnit, ref AudioUnitRenderActionFlags ioActionFlags,
        ref AudioTimeStamp inTimeStamp,
        uint inOutputBusNumber, uint inNumberFrames, IntPtr ioData);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern int AudioUnitSetProperty(IntPtr inUnit, AudioUnitPropertyIDType inID,
        AudioUnitScopeType inScope,
        uint inElement,
        ref AudioStreamBasicDescription inData,
        uint inDataSize);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitSetProperty(IntPtr inUnit, AudioUnitPropertyIDType inID,
        AudioUnitScopeType inScope, uint inElement,
        ref uint inData, uint inDataSize);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitSetProperty(IntPtr inUnit, AudioUnitPropertyIDType inID,
        AudioUnitScopeType inScope, uint inElement,
        ref double inData, uint inDataSize);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitSetProperty(IntPtr inUnit, AudioUnitPropertyIDType inID,
        AudioUnitScopeType inScope, uint inElement,
        ref IntPtr inData, int inDataSize);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitSetProperty(IntPtr inUnit, AudioUnitPropertyIDType inID,
        AudioUnitScopeType inScope, uint inElement,
        ref AURenderCallbackStruct inData, int inDataSize);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitSetProperty(IntPtr inUnit, AudioUnitPropertyIDType inID,
        AudioUnitScopeType inScope, uint inElement,
        ref AudioUnitConnection inData, int inDataSize);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitGetProperty(IntPtr inUnit, AudioUnitPropertyIDType inID,
        AudioUnitScopeType inScope, uint inElement,
        ref AudioStreamBasicDescription outData,
        ref uint ioDataSize);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitGetProperty(IntPtr inUnit, AudioUnitPropertyIDType inID,
        AudioUnitScopeType inScope, uint inElement,
        ref IntPtr outData,
        ref int ioDataSize);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitGetProperty(IntPtr inUnit, AudioUnitPropertyIDType inID,
        AudioUnitScopeType inScope, uint inElement,
        ref uint outData,
        ref int ioDataSize);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern unsafe AudioUnitStatus AudioUnitGetProperty(IntPtr inUnit, AudioUnitPropertyIDType inID,
        AudioUnitScopeType inScope, uint inElement,
        uint* outData,
        ref uint ioDataSize);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern unsafe AudioUnitStatus AudioUnitGetProperty(IntPtr inUnit, AudioUnitPropertyIDType inID,
        AudioUnitScopeType inScope, uint inElement,
        ref AudioUnitParameterInfoNative outData,
        ref uint ioDataSize);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern int AudioUnitGetProperty(IntPtr inUnit,
        [MarshalAs(UnmanagedType.U4)] AudioUnitPropertyIDType inID,
        [MarshalAs(UnmanagedType.U4)] AudioUnitScopeType inScope,
        [MarshalAs(UnmanagedType.U4)] uint inElement,
        ref uint flag,
        ref uint ioDataSize
    );


    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitGetPropertyInfo(IntPtr inUnit, AudioUnitPropertyIDType inID,
        AudioUnitScopeType inScope, uint inElement,
        out uint outDataSize, [MarshalAs(UnmanagedType.I1)] out bool outWritable);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitSetParameter(IntPtr inUnit, AudioUnitParameterType inID,
        AudioUnitScopeType inScope,
        uint inElement, float inValue, uint inBufferOffsetInFrames);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitScheduleParameters(IntPtr inUnit,
        AudioUnitParameterEvent inParameterEvent, uint inNumParamEvents);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus MusicDeviceMIDIEvent(IntPtr /* MusicDeviceComponent = void* */ inUnit,
        uint /* UInt32 */ inStatus, uint /* UInt32 */ inData1, uint /* UInt32 */ inData2,
        uint /* UInt32 */ inOffsetSampleFrame);

    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitSetProperty(IntPtr inUnit, AudioUnitPropertyIDType inID,
        AudioUnitScopeType inScope, uint inElement,
        ref AudioTimeStamp inData, int inDataSize);


    [DllImport(Constants.AudioUnitLibrary)]
    private static extern AudioUnitStatus AudioUnitSetProperty(IntPtr inUnit, AudioUnitPropertyIDType inID,
        AudioUnitScopeType inScope, uint inElement,
        IntPtr inData, int inDataSize);
}