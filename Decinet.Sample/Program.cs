// See https://aka.ms/new-console-template for more information


using Decinet.Decoders.Wave;


Console.WriteLine("Hello, World!");


var k = new WaveDecoder();
var y = "/Users/jumarmacato/jenny-s16.wav";

using var fs = File.OpenRead(y);
k.Receive(fs);

do
{
} while (k.TryRequestNewFrame());