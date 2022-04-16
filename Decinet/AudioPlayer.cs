using Decinet.Decoders.Wave;

namespace Decinet;

public class AudioPlayer
{
    public AudioPlayer(Stream audioStream)
    {
        var wavDecoder = new WaveDecoder();
        var playerController = new AudioPlaybackController();
        var resampler = new ShortToFloatResampler();
        var dspStack = new PassthroughDSPStack();
        
        playerController.Connect(wavDecoder, resampler);
        resampler.Connect(playerController, dspStack);
        
        

    }
}