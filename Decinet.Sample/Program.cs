// See https://aka.ms/new-console-template for more information


using Decinet.Decoders.Wave;


Console.WriteLine("Hello, World!");


var k = new WaveDecoder();

k.Receive(File.Open("/Users/jumarmacato/jenny-s16.wav", FileMode.Open));