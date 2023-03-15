using Decinet.Architecture;
using Decinet.Samples; 
namespace Decinet;

public class LinearInterpResampler : IResampler
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

        var ratio =   frame.AudioFormat.SampleRate /  (float)_backend.DesiredAudioFormat.SampleRate;

        if (_backend.DesiredAudioFormat.SampleRate == frame.AudioFormat.SampleRate)
        {
            if (_outResampler is null) _dsp?.Receive(data); else _outResampler?.Receive(data);
            return;
        }
        
        var signals = new float[frame.ChannelCount][];

        for (var i = 0; i < signals.Length; i++)
        {
            signals[i] = new float[frame.SampleCount];
        }

        var interleaveIndex = 0;

        for (var i = 0; i < frame.SampleCount; i++)
        {
            for (var j = 0; j < frame.ChannelCount; j++)
            {
                signals[j][i] = frame.InterleavedSampleData[interleaveIndex++];
            }
        }

        var ratioSampleCount = (int) Math.Round(ratio * frame.SampleCount);
        
        var output = new float[frame.ChannelCount][];

        for (var i = 0; i < signals.Length; i++)
        {
            output[i] = new float[ratioSampleCount];
        }
 
        for (var j = 0; j < frame.ChannelCount; j++)
        { 
            output[j] = Resample(signals[j], frame.AudioFormat.SampleRate, _backend.DesiredAudioFormat.SampleRate);
 
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

        if (_outResampler is null) _dsp?.Receive(nF); else _outResampler?.Receive(nF);
        
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
        int sourceLength = audioData.Length;
        double sourceDuration = (double)sourceLength / sourceSampleRate;

        int targetLength = (int)Math.Round(sourceDuration * targetSampleRate);
        float[] resampledData = new float[targetLength];

        double sourceToTargetRatio = (double)targetSampleRate / sourceSampleRate;
        double targetToSourceRatio = (double)sourceSampleRate / targetSampleRate;

        for (int i = 0; i < targetLength; i++)
        {
            double sourceIndex = i * targetToSourceRatio;
            int indexBefore = (int)Math.Floor(sourceIndex);
            int indexAfter = (int)Math.Ceiling(sourceIndex);

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
                double fractionAfter = sourceIndex - indexBefore;
                double fractionBefore = 1.0 - fractionAfter;

                float sampleBefore = audioData[indexBefore];
                float sampleAfter = audioData[indexAfter];

                resampledData[i] = (float)(fractionBefore * sampleBefore + fractionAfter * sampleAfter);
            }
        }

        return resampledData;
    }

}