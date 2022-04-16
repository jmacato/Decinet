//
// AUGraph.cs: AUGraph wrapper class
//
// Authors:
//   AKIHIRO Uehara (u-akihiro@reinforce-lab.com)
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2010 Reinforce Lab.
// Copyright 2010 Novell, Inc.
// Copyright 2012 Xamarin Inc
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
using Decinet.Backend.macOS.AudioToolbox;
using Decinet.Backend.macOS.ObjCRuntime;
#if !NETXXX
using NativeHandle = System.IntPtr;
#endif

namespace Decinet.Backend.macOS.AudioUnit
{
    [Obsolete("Starting with macos11.0 use 'AVAudioEngine' instead.", DiagnosticId = "BI1234",
        UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
    public class AUGraph : DisposableObject
    {
        private readonly GCHandle gcHandle;

        internal AUGraph(NativeHandle handle, bool owns)
            : base(handle, owns)
        {
            gcHandle = GCHandle.Alloc(this);
        }

        private static IntPtr Create()
        {
            var err = NewAUGraph(out var handle);
            if (err != 0)
                throw new InvalidOperationException($"Cannot create new AUGraph. Error code: {err}");
            return handle;
        }

        public AUGraph()
            : this(Create(), true)
        {
        }

        public static AUGraph? Create(out int errorCode)
        {
            errorCode = NewAUGraph(out var handle);

            if (errorCode != 0)
                return null;

            return new AUGraph(handle, true);
        }

        public bool IsInitialized => AUGraphIsInitialized(Handle, out var b) == AUGraphError.OK && b;

        public bool IsOpen => AUGraphIsOpen(Handle, out var b) == AUGraphError.OK && b;

        public bool IsRunning => AUGraphIsRunning(Handle, out var b) == AUGraphError.OK && b;

        public AudioUnitStatus AddRenderNotify(RenderDelegate callback)
        {
            if (callback is null)
                ThrowHelper.ThrowArgumentNullException(nameof(callback));

            var error = AudioUnitStatus.OK;
            if (graphUserCallbacks.Count == 0)
                error = (AudioUnitStatus) AUGraphAddRenderNotify(Handle, renderCallback, GCHandle.ToIntPtr(gcHandle));

            if (error == AudioUnitStatus.OK)
                graphUserCallbacks.Add(callback);
            return error;
        }

        public AudioUnitStatus RemoveRenderNotify(RenderDelegate callback)
        {
            if (callback is null)
                ThrowHelper.ThrowArgumentNullException(nameof(callback));
            if (!graphUserCallbacks.Contains(callback))
                throw new ArgumentException("Cannot unregister a callback that has not been registered");

            var error = AudioUnitStatus.OK;
            if (graphUserCallbacks.Count == 0)
                error = (AudioUnitStatus) AUGraphRemoveRenderNotify(Handle, renderCallback,
                    GCHandle.ToIntPtr(gcHandle));

            graphUserCallbacks.Remove(callback); // Remove from list even if there is an error
            return error;
        }

        private HashSet<RenderDelegate> graphUserCallbacks = new();

        // // [MonoPInvokeCallback (typeof(CallbackShared))]
        private static AudioUnitStatus renderCallback(IntPtr inRefCon,
            ref AudioUnitRenderActionFlags _ioActionFlags,
            ref AudioTimeStamp _inTimeStamp,
            uint _inBusNumber,
            uint _inNumberFrames,
            IntPtr _ioData)
        {
            // getting audiounit instance
            var handler = GCHandle.FromIntPtr(inRefCon);
            var inst = handler.Target as AUGraph;
            var renderers = inst?.graphUserCallbacks;

            if (renderers is null)
                return AudioUnitStatus.InvalidParameter;

            if (renderers.Count != 0)
                using (var buffers = new AudioBuffers(_ioData))
                {
                    foreach (var renderer in renderers)
                        renderer(_ioActionFlags, _inTimeStamp, _inBusNumber, _inNumberFrames, buffers);
                    return AudioUnitStatus.OK;
                }

            return AudioUnitStatus.InvalidParameter;
        }

        public void Open()
        {
            var err = AUGraphOpen(Handle);
            if (err != 0)
                throw new InvalidOperationException($"Cannot open AUGraph. Error code: {err}");
        }

        public int TryOpen()
        {
            var err = AUGraphOpen(Handle);
            return err;
        }

        public int AddNode(AudioComponentDescription description)
        {
            var err = AUGraphAddNode(Handle, ref description, out var node);
            if (err != 0)
                throw new ArgumentException($"Error code: {err}");

            return node;
        }

        public AUGraphError RemoveNode(int node)
        {
            return AUGraphRemoveNode(Handle, node);
        }

        public AUGraphError GetCPULoad(out float averageCPULoad)
        {
            return AUGraphGetCPULoad(Handle, out averageCPULoad);
        }

        public AUGraphError GetMaxCPULoad(out float maxCPULoad)
        {
            return AUGraphGetMaxCPULoad(Handle, out maxCPULoad);
        }

        public AUGraphError GetNode(uint index, out int node)
        {
            return AUGraphGetIndNode(Handle, index, out node);
        }

        public AUGraphError GetNodeCount(out int count)
        {
            return AUGraphGetNodeCount(Handle, out count);
        }

        public AudioUnit GetNodeInfo(int node)
        {
            AUGraphError error;
            var unit = GetNodeInfo(node, out error);

            if (error != AUGraphError.OK)
                throw new ArgumentException($"Error code:{error}");

            if (unit is null)
                throw new InvalidOperationException("Can not get object instance");

            return unit;
        }

        public AudioUnit? GetNodeInfo(int node, out AUGraphError error)
        {
            error = AUGraphNodeInfo(GetCheckedHandle(), node, IntPtr.Zero, out var ptr);

            if (error != AUGraphError.OK || ptr == IntPtr.Zero)
                return null;

            return new AudioUnit(ptr, false);
        }

        // AudioComponentDescription struct in only correctly fixed for unified
        // Following current Api behaviour of returning an AudioUnit instead of an error
        public AudioUnit? GetNodeInfo(int node, out AudioComponentDescription cd, out AUGraphError error)
        {
            error = AUGraphNodeInfo(GetCheckedHandle(), node, out cd, out var ptr);

            if (error != AUGraphError.OK || ptr == IntPtr.Zero)
                return null;

            return new AudioUnit(ptr, false);
        }

        public AUGraphError GetNumberOfInteractions(out uint interactionsCount)
        {
            return AUGraphGetNumberOfInteractions(Handle, out interactionsCount);
        }

        public AUGraphError GetNumberOfInteractions(int node, out uint interactionsCount)
        {
            return AUGraphCountNodeInteractions(Handle, node, out interactionsCount);
        }

/*
		// TODO: requires AudioComponentDescription type to be fixed
		public AUGraphError GetNodeInfo (int node, out AudioComponentDescription description, out AudioUnit audioUnit)
		{
			IntPtr au;
			var res = AUGraphNodeInfo (handle, node, out description, out au);
			if (res != AUGraphError.OK) {
				audioUnit = null;
				return res;
			}

			audioUnit = new AudioUnit (au);
			return res;
		}
*/
        public AUGraphError ConnnectNodeInput(int sourceNode, uint sourceOutputNumber, int destNode,
            uint destInputNumber)
        {
            return AUGraphConnectNodeInput(Handle,
                sourceNode, sourceOutputNumber,
                destNode, destInputNumber);
        }

        public AUGraphError DisconnectNodeInput(int destNode, uint destInputNumber)
        {
            return AUGraphDisconnectNodeInput(Handle, destNode, destInputNumber);
        }

        private Dictionary<uint, RenderDelegate>? nodesCallbacks;
        private static readonly CallbackShared CreateRenderCallback = RenderCallbackImpl;

        public AUGraphError SetNodeInputCallback(int destNode, uint destInputNumber, RenderDelegate renderDelegate)
        {
            if (nodesCallbacks is null)
                nodesCallbacks = new Dictionary<uint, RenderDelegate>();

            nodesCallbacks[destInputNumber] = renderDelegate;

            var cb = new AURenderCallbackStruct();
            cb.Proc = CreateRenderCallback;
            cb.ProcRefCon = GCHandle.ToIntPtr(gcHandle);
            return AUGraphSetNodeInputCallback(Handle, destNode, destInputNumber, ref cb);
        }

        // // [MonoPInvokeCallback (typeof (CallbackShared))]
        private static AudioUnitStatus RenderCallbackImpl(IntPtr clientData, ref AudioUnitRenderActionFlags actionFlags,
            ref AudioTimeStamp timeStamp, uint busNumber, uint numberFrames, IntPtr data)
        {
            var gch = GCHandle.FromIntPtr(clientData);
            var au = gch.Target as AUGraph;

            if (au?.nodesCallbacks is null)
                return AudioUnitStatus.InvalidParameter;

            if (!au.nodesCallbacks.TryGetValue(busNumber, out var callback))
                return AudioUnitStatus.InvalidParameter;

            using (var buffers = new AudioBuffers(data))
            {
                return callback(actionFlags, timeStamp, busNumber, numberFrames, buffers);
            }
        }

        public AUGraphError ClearConnections()
        {
            return AUGraphClearConnections(Handle);
        }

        public AUGraphError Start()
        {
            return AUGraphStart(Handle);
        }

        public AUGraphError Stop()
        {
            return AUGraphStop(Handle);
        }

        public AUGraphError Initialize()
        {
            return AUGraphInitialize(Handle);
        }

        public bool Update()
        {
            return AUGraphUpdate(Handle, out var isUpdated) == AUGraphError.OK && isUpdated;
        }

        // Quote from Learning CoreAudio Book:
        // The CAShow() function logs (to standard output) a list of all the nodes in the graph, 
        // along with the connections between them and the stream format used in each of those connections
        public void LogAllNodes()
        {
            CAShow(GetCheckedHandle());
        }

        protected override void Dispose(bool disposing)
        {
            if (Handle != IntPtr.Zero && Owns)
            {
                AUGraphUninitialize(Handle);
                AUGraphClose(Handle);
                DisposeAUGraph(Handle);
            }

            if (gcHandle.IsAllocated)
                gcHandle.Free();

            base.Dispose(disposing);
        }

        [DllImport(Constants.AudioToolboxLibrary, EntryPoint = "NewAUGraph")]
        private static extern int /* OSStatus */ NewAUGraph(out IntPtr outGraph);

        [DllImport(Constants.AudioToolboxLibrary, EntryPoint = "AUGraphOpen")]
        private static extern int /* OSStatus */ AUGraphOpen(IntPtr inGraph);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphAddNode(IntPtr inGraph, ref AudioComponentDescription inDescription,
            out int /* AUNode = SInt32* */ outNode);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphRemoveNode(IntPtr inGraph, int /* AUNode = SInt32 */ inNode);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphGetNodeCount(IntPtr inGraph, out int /* UInt32* */ outNumberOfNodes);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphGetIndNode(IntPtr inGraph, uint /* UInt32 */ inIndex,
            out int /* AUNode = SInt32* */ outNode);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphNodeInfo(IntPtr inGraph, int /* AUNode = SInt32 */ inNode,
            IntPtr outDescription, out IntPtr outAudioUnit);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphNodeInfo(IntPtr inGraph, int /* AUNode = SInt32 */ inNode,
            out AudioComponentDescription outDescription, out IntPtr outAudioUnit);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphClearConnections(IntPtr inGraph);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphConnectNodeInput(IntPtr inGraph,
            int /* AUNode = SInt32 */ inSourceNode, uint /* UInt32 */ inSourceOutputNumber,
            int /* AUNode = SInt32 */ inDestNode, uint /* UInt32 */ inDestInputNumber);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphDisconnectNodeInput(IntPtr inGraph,
            int /* AUNode = SInt32 */ inDestNode, uint /* UInt32 */ inDestInputNumber);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphGetNumberOfInteractions(IntPtr inGraph,
            out uint /* UInt32* */ outNumInteractions);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphCountNodeInteractions(IntPtr inGraph,
            int /* AUNode = SInt32 */ inNode, out uint /* UInt32* */ outNumInteractions);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphInitialize(IntPtr inGraph);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern int AUGraphAddRenderNotify(IntPtr inGraph, CallbackShared inCallback, IntPtr inRefCon);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern int AUGraphRemoveRenderNotify(IntPtr inGraph, CallbackShared inCallback, IntPtr inRefCon);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphStart(IntPtr inGraph);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphStop(IntPtr inGraph);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphUninitialize(IntPtr inGraph);

        [DllImport(Constants.AudioToolboxLibrary, EntryPoint = "AUGraphClose")]
        private static extern int /* OSStatus */ AUGraphClose(IntPtr inGraph);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern int /* OSStatus */ DisposeAUGraph(IntPtr inGraph);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphIsOpen(IntPtr inGraph,
            [MarshalAs(UnmanagedType.I1)] out bool outIsOpen);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphIsInitialized(IntPtr inGraph,
            [MarshalAs(UnmanagedType.I1)] out bool outIsInitialized);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphIsRunning(IntPtr inGraph,
            [MarshalAs(UnmanagedType.I1)] out bool outIsRunning);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError
            AUGraphGetCPULoad(IntPtr inGraph, out float /* Float32* */ outAverageCPULoad);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphGetMaxCPULoad(IntPtr inGraph, out float /* Float32* */ outMaxLoad);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphSetNodeInputCallback(IntPtr inGraph,
            int /* AUNode = SInt32 */ inDestNode, uint /* UInt32 */ inDestInputNumber,
            ref AURenderCallbackStruct inInputCallback);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern AUGraphError AUGraphUpdate(IntPtr inGraph,
            [MarshalAs(UnmanagedType.I1)] out bool outIsUpdated);

        [DllImport(Constants.AudioToolboxLibrary)]
        private static extern void CAShow(IntPtr handle);
    }
}