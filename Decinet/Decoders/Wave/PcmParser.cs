using System.Buffers;
using System.Xml.XPath;
using Decinet.Architecture;
using Decinet.Samples;

namespace Decinet.Decoders.Wave;

internal class PcmParser : WaveParser
{
    public PcmParser(BinaryReader currentStream, WaveFormat format, long rawDataStartOffset) :
        base(currentStream, format, rawDataStartOffset)
    {
        AudioFormat = Format.BitsPerSample switch
        {
            16 => new AudioFormat(typeof(short), sizeof(short), (int) format.SampleRate, format.NumChannels),
            32 => new AudioFormat(typeof(float), sizeof(float), (int) format.SampleRate, format.NumChannels),
            64 => new AudioFormat(typeof(long), sizeof(long), (int) format.SampleRate, format.NumChannels),
            _ => throw new Exception("Invalid bit size.")
        };
    }

    public override AudioFormat AudioFormat { get; }

    public override bool TryGetBytes(int numSamples, out ISampleFrame sampleFrame)
    {
        var samplesToGet = numSamples;
        sampleFrame = null!;
        var bytesPerSample = Format.BitsPerSample / 8;
        var sampleFrameBytes = samplesToGet * Format.NumChannels * (bytesPerSample);

        var normalizedSeekScalar = CurrentPosition.TotalSeconds / Duration.TotalSeconds;
        var byteOffset = (long) (normalizedSeekScalar * TotalRawDataBytes) + RawDataStartOffset;
        var streamPosBytes = CurrentStream.BaseStream.Position;
        var streamLengthBytes = CurrentStream.BaseStream.Length;

        if (byteOffset + sampleFrameBytes >= streamLengthBytes)
        {
            var remainingSamplesBytes = (int) (streamLengthBytes - byteOffset);
            if (remainingSamplesBytes == 0)
            {
                return false;
            }
            else
            {
                samplesToGet = remainingSamplesBytes / Format.NumChannels / bytesPerSample;
                sampleFrameBytes = remainingSamplesBytes;
            }
        }

        CurrentStream.BaseStream.Position = byteOffset;

        switch (bytesPerSample)
        {
            case sizeof(short):
                var rawShort = ArrayPool<short>.Shared.Rent(Format.NumChannels * samplesToGet);

                for (var i = 0; i < samplesToGet; i++)
                {
                    rawShort[i] = CurrentStream.ReadInt16();
                }

                sampleFrame = new ShortSampleFrame(rawShort, samplesToGet, Format.NumChannels, AudioFormat);
                break;
            case sizeof(float):
                var rawFloat = ArrayPool<float>.Shared.Rent(Format.NumChannels * samplesToGet);

                for (var i = 0; i < samplesToGet; i++)
                {
                    rawFloat[i] = CurrentStream.ReadSingle();
                }

                sampleFrame = new FloatSampleFrame(rawFloat, samplesToGet, Format.NumChannels, AudioFormat);
                break;
            case sizeof(double):

                var rawDouble = ArrayPool<double>.Shared.Rent(Format.NumChannels * samplesToGet);

                for (var i = 0; i < samplesToGet; i++)
                {
                    rawDouble[i] = CurrentStream.ReadSingle();
                }

                sampleFrame = new DoubleSampleFrame(rawDouble, samplesToGet, Format.NumChannels, AudioFormat);
                break;

            default:
                return false;
        }

        CurrentPosition += TimeSpan.FromSeconds(sampleFrameBytes / (double) Format.ByteRate);

        return true;
    }
}