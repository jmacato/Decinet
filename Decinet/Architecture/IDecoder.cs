using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Decinet.Architecture;

public interface IDecoder : INotifyPropertyChanged, IProcessingNode<Stream, IBackend, IPlaybackController>
{
    Format CurrentStreamFormat { get; }
    bool IsSeekable { get; }
    TimeSpan? TotalDuration { get; }
    TimeSpan? Position { get; }
    bool TrySeek(TimeSpan time);
    bool TryRequestNewFrame();
}