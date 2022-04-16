// using System.ComponentModel;
// using System.Reflection.Metadata.Ecma335;
// using Decinet.Architecture;
//
// namespace Decinet;
//
// public class AudioPlayer
// {
//     public AudioPlayer(Stream audioStream)
//     {
//         var playerController = new AudioPlaybackController();
//     }
// }
//
// public class AudioPlaybackController : IPlaybackController
// {
//     private IPlaybackController.PlaybackStatus _status;
//     private IDecoder _decoder;
//     private IResampler _resampler;
//     public event PropertyChangedEventHandler? PropertyChanged;
//     
//     public void Receive(ISampleFrame data)
//     {
//         if (_status == IPlaybackController.PlaybackStatus.Playing)
//         {
//             _resampler.Receive(data);
//         }
//     }
//
//     public void Connect(IDecoder priorNode, IResampler targetNode)
//     {
//         _decoder = priorNode;
//         _resampler = targetNode;
//     }
//
//     public IPlaybackController.PlaybackStatus Status
//     {
//         get => _status;
//         set
//         {
//             _status = value;
//             RaisePropertyChanged(nameof(Status));
//         }
//     }
//
//     private void RaisePropertyChanged(string propertyName)
//     {
//         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//     }
//
//     public void Play()
//     {
//         
//     }
//
//     public void Pause()
//     {
//         
//     }
//
//     public void Stop()
//     {
//         
//     }
// }