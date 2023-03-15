 
using System.ComponentModel;
using System.Runtime.InteropServices;
using Decinet.Architecture;
using Decinet.Samples;
using Decinet.Utilities;

namespace Decinet.Backend.macOS;

public class MacOsAudioToolkitBackend : IBackend
{
    private readonly AudioFormat _desiredAudioFormat;
    private IDecoder? _decoder;
    private volatile bool _isAudioStopped ;

    public MacOsAudioToolkitBackend()
    {
        _desiredAudioFormat = new AudioFormat(typeof(short), 2, 44100, 2);

        _audioBuffer = new CircularBuffer(KAudioQueueBufferSize);

        _timer = new Thread(FrameCallback);
    }

    private void FrameCallback()
    {
        do
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(1));

            if (_incomingFrames.Count == 0)
            {
                _decoder?.TryRequestNewFrame(TimeSpan.FromMilliseconds(10));
                continue;
            }

            using var currentFrame = _incomingFrames.Dequeue();

            var tempByteArray = new byte[currentFrame.InterleavedSampleData.Length * sizeof(short)];

            Buffer.BlockCopy(currentFrame.InterleavedSampleData, 0, tempByteArray, 0, tempByteArray.Length);

            var tbaCount = 0;

            while (true)
            {
                if (tbaCount >= tempByteArray.Length)
                {
                    break;
                }

                var canAdd = _audioBuffer.TryAdd(tempByteArray[tbaCount], 2);

                if (!canAdd)
                {
                    Thread.Sleep(1);
                    continue;
                }

                tbaCount++;
            }
        } while (!_isAudioStopped);
    }


    private readonly Queue<ShortSampleFrame> _incomingFrames = new();
    private readonly Thread _timer;
    private readonly CircularBuffer _audioBuffer;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public void Receive(ISampleFrame data)
    {
        if (data is ShortSampleFrame floatFrame &&
            data.AudioFormat == _desiredAudioFormat)
        {
            _incomingFrames.Enqueue(floatFrame);
        }
    }

    /// <inheritdoc />
    public void Connect(IDspStack? priorNode, IDecoder? targetNode)
    {
        _decoder = targetNode;
        InitializeAudioToolkit();
        _timer.Start();
    }

    private void InitializeAudioToolkit()
    {
        var audioFormat = new AudioStreamBasicDescription
        {
            mSampleRate = _desiredAudioFormat.SampleRate,
            mFormatID = 0x6C70636D, // kAudioFormatLinearPCM
            mFormatFlags = 0x4 /* kAudioFormatFlagIsSignedInteger */ | 0x8 /* kAudioFormatFlagIsPacked */,
            mBytesPerPacket = sizeof(short) * _desiredAudioFormat.ChannelCount,
            mFramesPerPacket = 1,
            mBytesPerFrame = sizeof(short) * _desiredAudioFormat.ChannelCount,
            mChannelsPerFrame = _desiredAudioFormat.ChannelCount,
            mBitsPerChannel = sizeof(short) * 8,
            mReserved = 0
        };

        _callback = (_, queue, buffer) =>
        {
            var bytesRead = FillBuffer(buffer, _audioBuffer.ReadBytes(KAudioQueueBufferSize));
            if (bytesRead > 0)
            {
                AudioQueueEnqueueBuffer(queue, buffer, 0, IntPtr.Zero);
            }
            else
            {
                AudioQueueStop(queue, false);
            }
        };

        AudioQueueNewOutput(ref audioFormat, _callback, nint.Zero, nint.Zero, nint.Zero, 0, out _audioQueue);

        for (var i = 0; i < _audioQueueBuffers.Length; i++)
        {
            AudioQueueAllocateBuffer(_audioQueue, KAudioQueueBufferSize, out _audioQueueBuffers[i]);
            var bytesRead = FillBuffer(_audioQueueBuffers[i], new byte[0x1]);
            if (bytesRead > 0)
            {
                AudioQueueEnqueueBuffer(_audioQueue, _audioQueueBuffers[i], 0, nint.Zero);
            }
            else
            {
                break;
            }
        }

        AudioQueueStart(_audioQueue, nint.Zero);
    }


    /// <inheritdoc />
    public AudioFormat DesiredAudioFormat => _desiredAudioFormat;


    private int FillBuffer(nint buffer, byte[] soundSamples)
    {
        var audioQueueBuffer = Marshal.PtrToStructure<AudioQueueBufferStruct>(buffer);

        // Calculate the number of bytes to copy
        var bytesToCopy = Math.Min(soundSamples.Length, KAudioQueueBufferSize);

        // Copy the data from the soundSamples to the buffer
        Marshal.Copy(soundSamples, 0, audioQueueBuffer.AudioData, bytesToCopy);

        // Update the audio queue buffer structure
        audioQueueBuffer.AudioDataByteSize = (uint)bytesToCopy;
        Marshal.StructureToPtr(audioQueueBuffer, buffer, false);

        // Update the soundSamples array
        if (bytesToCopy < soundSamples.Length)
        {
            Array.Copy(soundSamples, bytesToCopy, soundSamples, 0, soundSamples.Length - bytesToCopy);
            Array.Resize(ref soundSamples, soundSamples.Length - bytesToCopy);
        }

        return bytesToCopy;
    }

    public void Dispose()
    {
        _isAudioStopped = true;

        AudioQueueStop(_audioQueue, true);
        
        foreach (var t in _audioQueueBuffers)
        {
            AudioQueueDispose(t, true);
        }

        AudioQueueDispose(_audioQueue, true);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AudioQueueBufferStruct
    {
        private readonly uint AudioDataBytesCapacity;
        public readonly nint AudioData;
        public uint AudioDataByteSize;
        private readonly nint UserData;
        private readonly uint PacketDescriptionCapacity;
        private readonly nint IntPtrPacketDescriptions;
        private readonly int PacketDescriptionCount;
    }


    private const int KAudioQueueBufferSize = 0x10000;

    private nint _audioQueue;
    private readonly nint[] _audioQueueBuffers = new nint[2];
    private AudioQueueOutputCallback? _callback;

    [DllImport("/System/Library/Frameworks/AudioToolbox.framework/AudioToolbox")]
    private static extern int AudioQueueNewOutput(ref AudioStreamBasicDescription inFormat,
        AudioQueueOutputCallback? inCallbackProc, nint inUserData, nint inCallbackRunLoop, nint inCallbackRunLoopMode,
        uint inFlags, out nint outAq);

    [DllImport("/System/Library/Frameworks/AudioToolbox.framework/AudioToolbox")]
    private static extern int AudioQueueDispose(nint inAq, bool inImmediate);

    [DllImport("/System/Library/Frameworks/AudioToolbox.framework/AudioToolbox")]
    private static extern int AudioQueueAllocateBuffer(nint inAq, uint inBufferByteSize, out nint outBuffer);

    [DllImport("/System/Library/Frameworks/AudioToolbox.framework/AudioToolbox")]
    private static extern int AudioQueueEnqueueBuffer(nint inAq, nint inBuffer, uint inNumPacketDescs,
        nint inPacketDescs);

    [DllImport("/System/Library/Frameworks/AudioToolbox.framework/AudioToolbox")]
    private static extern int AudioQueueStart(nint inAq, nint inStartTime);

    [DllImport("/System/Library/Frameworks/AudioToolbox.framework/AudioToolbox")]
    private static extern int AudioQueueStop(nint inAq, bool inImmediate);

    [StructLayout(LayoutKind.Sequential)]
    private struct AudioStreamBasicDescription
    {
        public double mSampleRate;
        public int mFormatID;
        public int mFormatFlags;
        public int mBytesPerPacket;
        public int mFramesPerPacket;
        public int mBytesPerFrame;
        public int mChannelsPerFrame;
        public int mBitsPerChannel;
        public int mReserved;
    }

    private delegate void AudioQueueOutputCallback(nint inUserData, nint inAq, nint inBuffer);
}