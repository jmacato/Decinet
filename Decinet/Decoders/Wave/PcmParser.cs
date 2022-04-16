using System.Buffers;
using System.Xml.XPath;
using Decinet.Architecture;
using Decinet.Samples;

namespace Decinet.Decoders.Wave;

internal class PcmParser : WaveParser
{
    private readonly Format _format;

    public PcmParser(BinaryReader currentStream, WaveFormat format, long rawDataStartOffset) :
        base(currentStream, format, rawDataStartOffset)
    {
        _format = Format.BitsPerSample switch
        {
            16 => new Format(typeof(short), sizeof(short), (int) format.SampleRate, format.NumChannels),
            32 => new Format(typeof(float), sizeof(float), (int) format.SampleRate, format.NumChannels),
            64 => new Format(typeof(long), sizeof(long), (int) format.SampleRate, format.NumChannels),
            _ => throw new Exception("Invalid bit size.")
        };
    }


    public override bool TryGetBytes(int numSamples, out ISampleFrame? sampleFrame)
    {
        sampleFrame = null;
        var bytesPerSample = Format.BitsPerSample / 8;
        var sampleFrameBytes = numSamples * Format.NumChannels * (bytesPerSample);

        var normalizedSeekScalar = CurrentPosition.TotalSeconds / Duration.TotalSeconds;
        var byteOffset = (long) (normalizedSeekScalar * TotalRawDataBytes) + RawDataStartOffset;
        var streamPos = CurrentStream.BaseStream.Position;
        var streamLength = CurrentStream.BaseStream.Length;

        if (streamPos + sampleFrameBytes > streamLength)
        {
            var remainingSamplesBytes = (int) (streamPos + sampleFrameBytes) - (int) streamLength;
            if (remainingSamplesBytes == 0)
            {
                return false;
            }
            else
            {
                numSamples = remainingSamplesBytes / Format.NumChannels / bytesPerSample;
                sampleFrameBytes = remainingSamplesBytes;
            }
        }

        CurrentStream.BaseStream.Position = byteOffset;
        
        switch (bytesPerSample)
        {
            case sizeof(short):
                var rawShort = ArrayPool<short>.Shared.Rent(Format.NumChannels * numSamples);

                for (var i = 0; i < sampleFrameBytes / sizeof(short); i++)
                {
                    rawShort[i] = CurrentStream.ReadInt16();
                }

                sampleFrame = new ShortSampleFrame(rawShort, numSamples, Format.NumChannels, _format);
                break;
            case sizeof(float):
                var rawFloat = ArrayPool<float>.Shared.Rent(Format.NumChannels * numSamples);

                for (var i = 0; i < sampleFrameBytes / sizeof(float); i++)
                {
                    rawFloat[i] = CurrentStream.ReadSingle();
                }

                sampleFrame = new FloatSampleFrame(rawFloat, numSamples, Format.NumChannels, _format);
                break;
            case sizeof(double):

                var rawDouble = ArrayPool<double>.Shared.Rent(Format.NumChannels * numSamples);

                for (var i = 0; i < sampleFrameBytes / sizeof(double); i++)
                {
                    rawDouble[i] = CurrentStream.ReadSingle();
                }

                sampleFrame = new DoubleSampleFrame(rawDouble, numSamples, Format.NumChannels, _format);
                break;

            default:
                return false;
        }

        CurrentPosition += TimeSpan.FromSeconds(sampleFrameBytes / (double) Format.ByteRate);

        return true;
    }
}