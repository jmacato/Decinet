using System.ComponentModel;

namespace Decinet.Architecture;

public interface IBackend : INotifyPropertyChanged, IProcessingNode<ISampleFrame, IDspStack, IDecoder>
{
    AudioFormat DesiredAudioFormat { get; }
}