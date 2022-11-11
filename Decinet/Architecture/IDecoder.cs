using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Decinet.Architecture;

public interface IDecoder : INotifyPropertyChanged, IProcessingNode<Stream, IBackend, IPlaybackController>
{
    AudioFormat CurrentStreamAudioFormat { get; }
    bool IsSeekable { get; }
    TimeSpan? Duration { get; }
    TimeSpan? Position { get; }
    bool Ready { get; }
    bool TrySeek(TimeSpan time);
    bool TryRequestNewFrame(TimeSpan sampleTime);
}