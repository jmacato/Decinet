﻿// See https://aka.ms/new-console-template for more information


using Decinet;
using Decinet.Backend.macOS;
using Decinet.Decoders.Vorbis; 


Console.WriteLine("Hello, World!");


const string y = "/Users/jmacato/Downloads/HoliznaCC0-No-One-Is-Perfect.ogg";
using var fs = File.OpenRead(y);

var wavDecoder = new VorbisDecoder();
var playerController = new AudioPlaybackController();
var sampler1 = new LinearInterpResampler();
var sampler2 = new FloatToShortResampler();
var dspStack = new PassthroughDspStack();
var backend = new MacOsAudioToolkitBackend();

wavDecoder.Connect(backend, playerController);
wavDecoder.Receive(fs);

playerController.Connect(wavDecoder, sampler1);

sampler1.Connect(playerController, dspStack);
sampler1.ConnectOutToResampler(sampler2);
sampler1.ConnectBackend(backend);

sampler2.Connect(playerController, dspStack);
sampler2.ConnectBackend(backend);

dspStack.Connect(sampler2, backend);
backend.Connect(dspStack, wavDecoder);

playerController.Play();

Console.ReadLine();