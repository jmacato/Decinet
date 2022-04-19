using Decinet.Backend.macOS;
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
        var backend = new CoreAudioBackend();
        
        wavDecoder.Connect(backend, playerController);
        playerController.Connect(wavDecoder, resampler);
        resampler.Connect(playerController, dspStack);
        resampler.ConnectBackend(backend);
        dspStack.Connect(resampler, backend);
        backend.Connect(dspStack, wavDecoder);

    }
}