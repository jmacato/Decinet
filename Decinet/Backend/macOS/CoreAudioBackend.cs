using System.Buffers;
using System.ComponentModel;
using System.Reflection.Emit;
using Decinet.Architecture;
using Decinet.Backend.macOS.AudioToolbox;
using Decinet.Backend.macOS.AudioUnit;
using Decinet.Samples;
using Decinet.Utilities;

namespace Decinet.Backend.macOS;

public class CoreAudioBackend : IBackend
{
    private AudioFormat _desiredAudioFormat;
    private IDSPStack _dspEngine;
    private IDecoder _decoder;
    private readonly AudioUnit.AudioUnit _audioUnit;
    private readonly FileStream _tempStream;

    public CoreAudioBackend()
    {
        var op = new AudioComponentDescription()
        {
            ComponentFlags = 0,
            ComponentFlagsMask = 0,
            ComponentManufacturer = AudioComponentManufacturerType.Apple,
            ComponentType = AudioComponentType.Output,
            ComponentSubType = AudioUnitSubType.SystemOutput
        };

        var audioComponent = AudioComponent.FindNextComponent(null, ref op);

        if (audioComponent is null)
        {
            throw new InvalidOperationException("Cannot find the AudioComponent for the macOS CoreAudio backend.");
        }

        _audioUnit = new AudioUnit.AudioUnit(audioComponent);

        _audioUnit.SetEnableIO(true, AudioUnitScopeType.Output);

        var hwFormat = _audioUnit.GetAudioFormat(AudioUnitScopeType.Output);

        Type formatType = null;

        if (hwFormat.FormatFlags.HasFlag(AudioFormatFlags.IsFloat))
        {
            formatType = typeof(float);
        }
        else if (hwFormat.FormatFlags.HasFlag(AudioFormatFlags.IsSignedInteger))
        {
            formatType = typeof(short);
        }

        _desiredAudioFormat = new AudioFormat(formatType, hwFormat.BitsPerChannel / 8, (int) hwFormat.SampleRate,
            hwFormat.ChannelsPerFrame);

        _audioBuffer = new CircularBuffer(TimeSpan.FromMilliseconds(300), _desiredAudioFormat.SampleRate,
            _desiredAudioFormat.BytesPerSample, _desiredAudioFormat.ChannelCount);

        _audioUnit.SetRenderCallback(render_CallBack, AudioUnitScopeType.Output, 0);

        _audioUnit.Initialize();

        _timer = new Thread(FrameCallback);
    }

    private void FrameCallback()
    {
        do
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(1));

            if (incomingFrames.Count == 0)
            {
                _decoder.TryRequestNewFrame(TimeSpan.FromMilliseconds(100));
                continue;
            }

            using var currentFrame = incomingFrames.Dequeue();

            var tempByteArray = new byte[currentFrame.InterleavedSampleData.Length * sizeof(float)];

            Buffer.BlockCopy(currentFrame.InterleavedSampleData, 0, tempByteArray, 0, tempByteArray.Length);

            int tbaCount = 0;

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
        } while (_audioUnit.IsPlaying);
    }


    private int backendCalls = 0;
    private int bufferCalls = 0;
    private float[] _data;
    private bool _hasData;
    private Queue<FloatSampleFrame> incomingFrames = new();
    private readonly Thread _timer;
    private bool _connected;
    private readonly CircularBuffer _audioBuffer;

    private unsafe AudioUnitStatus render_CallBack(AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp,
        uint busNumber, uint numberFrames, AudioBuffers data)
    {
        if (_audioBuffer.Count == 0)
        {
            return AudioUnitStatus.OK;
        }

        var channelCounters = new int[data.Count];

        var tempBufferCounter = 0;

        for (var samples = 0; samples < numberFrames; samples++)
        for (var channels = 0; channels < data.Count; channels++)
        for (var sampleByteCounter = 0; sampleByteCounter < sizeof(float); sampleByteCounter++)
        {
            if (_audioBuffer.Count == 0) break;
            var sampleByte = _audioBuffer.Read();
            var curDat = (byte*) data[channels].Data;
            curDat[channelCounters[channels]++] = sampleByte;
            tempBufferCounter++;
        }


        return AudioUnitStatus.NoError;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public void Receive(ISampleFrame data)
    {
        if (data is FloatSampleFrame floatFrame &&
            data.AudioFormat == _desiredAudioFormat)
        {
            incomingFrames.Enqueue(floatFrame);
        }
    }

    /// <inheritdoc />
    public void Connect(IDSPStack priorNode, IDecoder targetNode)
    {
        _dspEngine = priorNode;
        _decoder = targetNode;
        _audioUnit.Start();
        _timer.Start();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _connected = false;

        if (_audioUnit.IsPlaying)
            _audioUnit?.Stop();
    }

    /// <inheritdoc />
    public AudioFormat DesiredAudioFormat => _desiredAudioFormat;
}