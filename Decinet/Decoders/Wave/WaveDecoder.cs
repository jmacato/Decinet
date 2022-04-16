using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using Decinet.Architecture;

namespace Decinet.Decoders.Wave;

public class WaveDecoder : IDecoder
{
    private RiffHeader _header;
    private WaveFormat _format;
    private WaveFact _fact;
    private WaveData _metadata;
    private int _samplesLeft;
    private byte[] _decodedData;

    private int _numSamples;
    private BinaryReader _dataStream;
    private long _audioChunkStart;
    private IPlaybackController _playbackController;
    private WaveParser _waveParser;
    private TimeSpan? _duration;
    private TimeSpan? _position;
    private AudioFormat _csaf;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Receive(Stream data)
    {
        _dataStream = new BinaryReader(data);

        _header = RiffHeader.Parse(_dataStream);
        _format = WaveFormat.Parse(_dataStream);

        if (_format.WaveType != WaveFormatType.Pcm)
        {
            _fact = WaveFact.Parse(_dataStream);
        }

        _metadata = WaveData.Parse(_dataStream);

        _audioChunkStart = _dataStream.BaseStream.Position;

        _waveParser = WaveParser.GetParser(_dataStream, _format, _audioChunkStart);

        CurrentStreamAudioFormat = _waveParser.AudioFormat;
    }

    public void Connect(IBackend priorNode, IPlaybackController targetNode)
    {
        _playbackController = targetNode;
    }

    public void Dispose()
    {
        _dataStream?.Close();
        _dataStream?.Dispose();
    }

    /// <inheritdoc />
    public AudioFormat CurrentStreamAudioFormat
    {
        get => _csaf;
        set
        {
            if (_csaf == value) return;
            _csaf = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentStreamAudioFormat)));
        }
    }

    /// <inheritdoc />
    public bool IsSeekable => _dataStream.BaseStream?.CanRead ?? false;


    /// <inheritdoc />
    public TimeSpan? Duration
    {
        get => _duration;
        set
        {
            if (_duration == value) return;
            _duration = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Duration)));
        }
    }

    /// <inheritdoc />
    public TimeSpan? Position
    {
        get => _position;
        set
        {
            if (_position == value) return;
            _position = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
        }
    }

    public bool TrySeek(TimeSpan time)
    {
        return false;
    }

    public bool TryRequestNewFrame(int samplesRequested)
    {
        return _waveParser.TryGetBytes(samplesRequested, out var sampleFrame);
    }
}