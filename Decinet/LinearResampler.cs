using Decinet.Architecture;
using Decinet.Samples; 
namespace Decinet;

public class LinearResampler : IResampler
{
     private IDspStack _dsp;
    private IBackend _backend;
 
    private IResampler? _outResampler;

    public void ConnectOutToResampler(IResampler resampler)
    {
        _outResampler = resampler;
    }
     
    /// <inheritdoc />
    public void Receive(ISampleFrame data)
    {
        if (data is not FloatSampleFrame frame)
        {
            return;
        }

        if (_backend.DesiredAudioFormat.SampleRate == frame.AudioFormat.SampleRate)
        {
            if (_outResampler is null) _dsp.Receive(data); else _outResampler?.Receive(data);
            return;
        }
        
        var tracks = new float[frame.ChannelCount][];

        for (var i = 0; i < tracks.Length; i++)
        {
            tracks[i] = new float[frame.SampleCount];
        }

        var interleaveIndex = 0;

        for (var i = 0; i < frame.SampleCount; i++)
        {
            for (var ch = 0; ch < frame.ChannelCount; ch++)
            {
                tracks[ch][i] = frame.InterleavedSampleData[interleaveIndex++];
            }
        }

        var output = new float[frame.ChannelCount][];
 
        for (var ch = 0; ch < frame.ChannelCount; ch++)
        { 
            output[ch] = Resample(tracks[ch], frame.AudioFormat.SampleRate, _backend.DesiredAudioFormat.SampleRate);
 
        }
        
        interleaveIndex = 0;
        
        var nF = FloatSampleFrame.Create(output[0].Length, _backend.DesiredAudioFormat.ChannelCount,
            _backend.DesiredAudioFormat);
        
        for (var i = 0; i < nF.SampleCount; i++)
        {
            for (var j = 0; j < nF.ChannelCount; j++)
            {
                nF.InterleavedSampleData[interleaveIndex++] =
                    output[j][i];
            }
        }
        
        frame.Dispose();

        if (_outResampler is null) _dsp.Receive(nF); else _outResampler?.Receive(nF);
        
     }

    /// <inheritdoc />
    public void Connect(IPlaybackController priorNode, IDspStack targetNode)
    {
         _dsp = targetNode;
    }

    /// <inheritdoc />
    public void ConnectBackend(IBackend backend)
    {
        _backend = backend;
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
    
    
    public static float[] Resample(float[] audioData, int sourceSampleRate, int targetSampleRate)
    {
        var sourceLength = audioData.Length;
        var sourceDuration = (double)sourceLength / sourceSampleRate;

        var targetLength = (int)Math.Round(sourceDuration * targetSampleRate);
        var resampledData = new float[targetLength];

        var sourceToTargetRatio = (double)targetSampleRate / sourceSampleRate;
        var targetToSourceRatio = (double)sourceSampleRate / targetSampleRate;

        for (var i = 0; i < targetLength; i++)
        {
            var sourceIndex = i * targetToSourceRatio;
            var indexBefore = (int)Math.Floor(sourceIndex);
            var indexAfter = (int)Math.Ceiling(sourceIndex);

            if (indexBefore < 0 || indexAfter >= sourceLength)
            {
                // Use zero padding for out-of-bounds indices
                resampledData[i] = 0f;
            }
            else if (indexBefore == indexAfter)
            {
                // Use exact source sample for integer indices
                resampledData[i] = audioData[indexBefore];
            }
            else
            {
                // Use linear interpolation for non-integer indices
                var fractionAfter = sourceIndex - indexBefore;
                var fractionBefore = 1.0 - fractionAfter;

                var sampleBefore = audioData[indexBefore];
                var sampleAfter = audioData[indexAfter];

                resampledData[i] = (float)(fractionBefore * sampleBefore + fractionAfter * sampleAfter);
            }
        }

        return resampledData;
    }

}