// See https://aka.ms/new-console-template for more information


using Decinet.Decoders.Wave;


Console.WriteLine("Hello, World!");


var decoder = new WaveDecoder();
var y = "/Users/jumarmacato/jenny-s16.wav";
using var fs = File.OpenRead(y);


decoder.Receive(fs);

do
{
} while (decoder.TryRequestNewFrame(1078));