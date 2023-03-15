// See https://aka.ms/new-console-template for more information


using Decinet;
using Decinet.Backend.macOS;
using Decinet.Decoders.Vorbis;
using Decinet.Decoders.Wave;


Console.WriteLine("Hello, World!");


const string y = "/Users/jmacato/Downloads/03-Spectrum-_feat-Matthew-Koma_.ogg";
using var fs = File.OpenRead(y);

var wavDecoder = new VorbisDecoder();
var playerController = new AudioPlaybackController();
var sampler = new FloatToShortResampler();
var dspStack = new PassthroughDSPStack();
var backend = new CoreAudioBackend();

wavDecoder.Connect(backend, playerController);
wavDecoder.Receive(fs);

playerController.Connect(wavDecoder, sampler);
sampler.Connect(playerController, dspStack);
sampler.ConnectBackend(backend);

dspStack.Connect(sampler, backend);
backend.Connect(dspStack, wavDecoder);

playerController.Play();

Console.ReadLine();