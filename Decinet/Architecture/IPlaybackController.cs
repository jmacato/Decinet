using System.ComponentModel;

namespace Decinet.Architecture;

public interface IPlaybackController : INotifyPropertyChanged, IProcessingNode<IDecoder, IResampler>
{
    public enum PlaybackStatus
    {
        Idle,
        Playing,
        Paused,
        Stopped
    }

    PlaybackStatus Status { get; }

    void Play();
    void Pause();
    void Stop();
}