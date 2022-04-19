using System.ComponentModel;
using Decinet.Architecture;
using Decinet.Samples;
using NVorbis;

namespace Decinet.Decoders.Vorbis;

public class VorbisDecoder : IDecoder
{
    private VorbisReader _reader;
    private float[] _readBuf;
    private AudioFormat _currentStreamAudioFormat;
    private bool _isSeekable = false;
    private TimeSpan? _duration = TimeSpan.Zero;
    private bool _ready;
    private AudioFormat _currentStreamAudioFormat1;
    private bool _isSeekable1;
    private TimeSpan? _duration1 = TimeSpan.Zero;
    private TimeSpan? _position = TimeSpan.Zero;
    private bool _ready1;
    private IBackend _backend;
    private IPlaybackController _playback;

    //
    // private static void CastBuffer(float[] inBuffer, byte[] outBuffer, int length)
    // {
    //     for (int i = 0; i < length; i++)
    //     {
    //         var temp = (int) (short.MaxValue * inBuffer[i]);
    //
    //         if (temp > short.MaxValue)
    //         {
    //             temp = short.MaxValue;
    //         }
    //         else if (temp < short.MinValue)
    //         {
    //             temp = short.MinValue;
    //         }
    //
    //         outBuffer[2 * i] = (byte) (((short) temp) & 0xFF);
    //         outBuffer[2 * i + 1] = (byte) (((short) temp) >> 8);
    //     }
    // // }
    //
    // public long GetSamples(int samples, ref byte[] data)
    // {
    //     int bytes = _audioFormat.BytesPerSample * samples;
    //     Array.Resize(ref data, bytes);
    //
    //     Array.Resize(ref _readBuf, samples);
    //     _reader.ReadSamples(_readBuf, 0, samples);
    //
    //     CastBuffer(_readBuf, data, samples);
    //
    //     return samples;
    // }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public void Receive(Stream data)
    {
        _reader = new VorbisReader(data, true);

        _currentStreamAudioFormat1 =
            new AudioFormat(typeof(float), sizeof(float), _reader.SampleRate, _reader.Channels);

        _ready1 = true;
    }

    /// <inheritdoc />
    public void Connect(IBackend priorNode, IPlaybackController targetNode)
    {
        _backend = priorNode;
        _playback = targetNode;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _reader?.Dispose();
    }

    /// <inheritdoc />
    public AudioFormat CurrentStreamAudioFormat => _currentStreamAudioFormat1;

    /// <inheritdoc />
    public bool IsSeekable => _isSeekable1;

    /// <inheritdoc />
    public TimeSpan? Duration => _duration1;

    /// <inheritdoc />
    public TimeSpan? Position => _position;

    /// <inheritdoc />
    public bool Ready => _ready1;

    /// <inheritdoc />
    public bool TrySeek(TimeSpan time)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool TryRequestNewFrame(int samplesRequested)
    {
        var samples = FloatSampleFrame.Create(samplesRequested, _currentStreamAudioFormat1.ChannelCount,
            _currentStreamAudioFormat1);

        var res = _reader.ReadSamples(samples.InterleavedSampleData, 0, samples.InterleavedSampleData.Length);

        if (res <= 0) return false;
        
        _playback?.Receive(samples);
        return true;
    }
}