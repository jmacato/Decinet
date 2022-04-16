using System.ComponentModel;

namespace Decinet.Architecture;

public interface IPlaybackController : INotifyPropertyChanged, IProcessingNode<ISampleFrame, IDecoder, IResampler> 
{
    public enum PlaybackStatus
    {
        Idle,
        Playing,
        Paused,
        Stopped,
        Seeking
    }

    PlaybackStatus Status { get; }

    void Play();
    void Pause();
    void Stop();
    bool Seek(TimeSpan time);
    TimeSpan? TotalDuration { get; }

}