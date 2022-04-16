using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Decinet.Architecture;

// public abstract class AudioPlayer
// {
//     public IPlaybackController? Controller { get; private set; }
//
//     protected IDecoder Decoder { get; }
// }

public interface IProcessingNode<in TInput>
{
    void Receive(TInput data);
}

public interface IDecoder : INotifyPropertyChanged, IProcessingNode<Stream, IPlaybackController>
{
    Format CurrentStreamFormat { get; }
    bool IsSeekable { get; }
    TimeSpan? TotalDuration { get; }
    void OpenStream(Stream audioStream);
    bool Seek(TimeSpan targetDuration);
}