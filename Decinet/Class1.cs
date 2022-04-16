using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using Decinet.Architecture;

namespace Decinet;

public class AudioPlayer
{
    public AudioPlayer(Stream audioStream)
    {
        var playerController = new AudioPlaybackController();
    }
}

public class AudioPlaybackController : IPlaybackController
{
    private IDecoder _decoder;
    private IResampler _resampler;
    private IPlaybackController.PlaybackStatus _status;
    private TimeSpan? _totalDuration;
    private TimeSpan? _position;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Receive(ISampleFrame data)
    {
        if (_status == IPlaybackController.PlaybackStatus.Playing)
        {
            _resampler.Receive(data);
        }
    }

    public void Connect(IDecoder priorNode, IResampler targetNode)
    {
        _decoder = priorNode;
        _resampler = targetNode;

        _decoder.PropertyChanged += DecoderOnPropertyChanged;
    }

    private void DecoderOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_decoder.Duration))
        {
            Duration = _decoder.Duration;
        }
        else if (e.PropertyName == nameof(_decoder.Position))
        {
            Position = _decoder.Position;
        }
    }

    public void Dispose()
    {
        _decoder.PropertyChanged -= DecoderOnPropertyChanged;
    }


    public IPlaybackController.PlaybackStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
        }
    }

    public void Play()
    {
        if (Status != IPlaybackController.PlaybackStatus.Playing ||
            Status != IPlaybackController.PlaybackStatus.Seeking)
        {
            Status = IPlaybackController.PlaybackStatus.Playing;
        }
    }

    public void Pause()
    {
        if (Status == IPlaybackController.PlaybackStatus.Playing)
        {
            Status = IPlaybackController.PlaybackStatus.Stopped;
        }
    }

    public void Stop()
    {
        Status = IPlaybackController.PlaybackStatus.Stopped;
    }
 

    public bool Seek(TimeSpan time)
    {
        var previousStatus = _status;
        Status = IPlaybackController.PlaybackStatus.Seeking;
        var res = _decoder.TrySeek(time);
        Status = previousStatus;
        return res;
    }

    public TimeSpan? Duration
    {
        get => _totalDuration;
        set
        {
            _totalDuration = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Duration)));
        }
    }

    public TimeSpan? Position
    {
        get => _position;
        set
        {
            if (value.HasValue && _decoder.TrySeek(value.Value))
            {
                _position = value;
            }

            // This call forces the old value back in case of GUI players.
            // if the decoder doesnt support seeking.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
        }
    }

    public bool IsSeekable => _decoder.IsSeekable;
}