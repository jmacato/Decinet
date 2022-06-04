// See https://aka.ms/new-console-template for more information


using Decinet;
using Decinet.Backend.macOS;
using Decinet.Decoders.Vorbis;
using Decinet.Decoders.Wave;


Console.WriteLine("Hello, World!");


const string y = "/Users/jumarmacato/Downloads/file_example_OOG_1MG.ogg";
using var fs = File.OpenRead(y);


var wavDecoder = new VorbisDecoder();
var playerController = new AudioPlaybackController();
var resampler = new ShortToFloatResampler();
var wdlResampler = new NWaveResampler();
var dspStack = new PassthroughDSPStack();
var backend = new CoreAudioBackend();

wavDecoder.Connect(backend, playerController);
wavDecoder.Receive(fs);

playerController.Connect(wavDecoder, resampler);
resampler.Connect(playerController, dspStack);
resampler.ConnectBackend(backend);

wdlResampler.Connect(playerController, dspStack);
wdlResampler.ConnectBackend(backend);

resampler.ConnectOutToResampler(wdlResampler);

dspStack.Connect(resampler, backend);
backend.Connect(dspStack, wavDecoder);

playerController.Play();


Console.ReadLine();