using System.ComponentModel;

namespace Decinet.Architecture;

public interface IBackend : INotifyPropertyChanged, IProcessingNode<ISampleFrame, IDSPStack, IDecoder>
{
    AudioFormat DesiredAudioFormat { get; }
}