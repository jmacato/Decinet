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
    private WaveParser _variant;

    // public override bool IsFinished => _samplesLeft == 0;
    //
    // public override TimeSpan Position => TimeSpan.MinValue;
    //
    // public override bool HasPosition { get; } = false;

    // public   long GetSamples(int samples, ref byte[] data)
    // {
    //     var numSamples = Math.Min(samples, _samplesLeft);
    //     long byteSize = _audioFormat.BytesPerSample * numSamples;
    //     long byteOffset = (_numSamples - _samplesLeft) * _audioFormat.BytesPerSample;
    //
    //     data = _decodedData.AsSpan<byte>().Slice((int) (byteOffset), (int) byteSize).ToArray();
    //     _samplesLeft -= numSamples;
    //
    //     return numSamples;
    // }

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
        
        _variant = WaveParser.GetParser(_dataStream, _format, _audioChunkStart);
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

    public Format CurrentStreamFormat { get; }
    public bool IsSeekable { get; }
    public TimeSpan? TotalDuration { get; }
    public TimeSpan? Position { get; }
    

    public bool TrySeek(TimeSpan time)
    {
        return false;
    }

    public bool TryRequestNewFrame()
    {
        return false;
    }
}