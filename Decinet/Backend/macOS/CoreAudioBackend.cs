using System.Buffers;
using System.ComponentModel;
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
    private readonly CircularBuffer _circularBuffer;
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

        _circularBuffer =
            new CircularBuffer((int) (_desiredAudioFormat.BytesPerSample *
                                      _desiredAudioFormat.ChannelCount *
                                      _desiredAudioFormat.SampleRate *
                                      TimeSpan.FromSeconds(0.1).TotalSeconds));

        _audioUnit.SetRenderCallback(render_CallBack, AudioUnitScopeType.Output, 0);

        _audioUnit.Initialize();

        _tempStream = File.Create($"{Guid.NewGuid():N}.float16.raw");
    }

    private int backendCalls = 0;
    private int bufferCalls = 0;
    private float[] _data;
    private bool _hasData;

    private unsafe AudioUnitStatus render_CallBack(AudioUnitRenderActionFlags actionFlags, AudioTimeStamp timeStamp,
        uint busNumber, uint numberFrames, AudioBuffers data)
    {
        _decoder.TryRequestNewFrame((int) numberFrames);

        if (_hasData)
        {
            _hasData = false;
        }
        else
        {
            return AudioUnitStatus.NoError;
        }
        
        backendCalls++;

        var tempBuffer = ArrayPool<byte>.Shared.Rent(sizeof(float) * _data.Length);

        Buffer.BlockCopy(_data, 0, tempBuffer, 0, tempBuffer.Length);

        var channelCounters = new int[data.Count];

        var tempBufferCounter = 0;

        for (var samples = 0; samples < numberFrames; samples++)
        for (var channels = 0; channels < data.Count; channels++)
        for (var sampleByteCounter = 0; sampleByteCounter < _desiredAudioFormat.BytesPerSample; sampleByteCounter++)
        {
            var sampleByte = tempBuffer[tempBufferCounter];
            var curDat = (byte*) data[channels].Data;
            curDat[channelCounters[channels]++] = sampleByte;
            tempBufferCounter++;
        }

        ArrayPool<byte>.Shared.Return(tempBuffer, true);
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
            _data = floatFrame.InterleavedSampleData;
            _hasData = true;

            /// _circularBuffer.Write(floatFrame.InterleavedSampleData, 0, floatFrame.InterleavedSampleData.Length);
        }
    }

    /// <inheritdoc />
    public void Connect(IDSPStack priorNode, IDecoder targetNode)
    {
        _dspEngine = priorNode;
        _decoder = targetNode;
        _audioUnit.Start();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_audioUnit.IsPlaying)
            _audioUnit?.Stop();
    }

    /// <inheritdoc />
    public AudioFormat DesiredAudioFormat => _desiredAudioFormat;
}