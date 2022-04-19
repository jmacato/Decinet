// This class based on the Resampler that is part of Cockos WDL
// originally written in C++ and ported to C# for NAudio by Mark Heath
// Used in NAudio with permission from Justin Frankel
// Original WDL License:
//     Copyright (C) 2005 and later Cockos Incorporated
//     
//     Portions copyright other contributors, see each source file for more information
// 
//     This software is provided 'as-is', without any express or implied
//     warranty.  In no event will the authors be held liable for any damages
//     arising from the use of this software.
// 
//     Permission is granted to anyone to use this software for any purpose,
//     including commercial applications, and to alter it and redistribute it
//     freely, subject to the following restrictions:
// 
//     1. The origin of this software must not be misrepresented; you must not
//        claim that you wrote the original software. If you use this software
//        in a product, an acknowledgment in the product documentation would be
//        appreciated but is not required.
//     2. Altered source versions must be plainly marked as such, and must not be
//        misrepresented as being the original software.
//     3. This notice may not be removed or altered from any source distribution.


// default to floats for audio samples

using System.Buffers;
using Decinet.Architecture;
using Decinet.Samples;
using NWaves.Filters.Base;
using NWaves.Operations;
using NWaves.Signals;
using WDL_ResampleSample = System.Single; // n.b. default in WDL is double

// default to floats for sinc filter ceofficients
using WDL_SincFilterSample = System.Single; // can also be set to double

namespace Decinet;

public class NWaveResampler : IResampler
{
    private IPlaybackController _playback;
    private IDSPStack _dsp;
    private IBackend _backend;
    private readonly Resampler _resamplerC;

    public NWaveResampler()
    {
        _resamplerC = new Resampler();
    }

    /// <inheritdoc />
    public void Receive(ISampleFrame data)
    {
        // _dsp?.Receive(data);
        // return;
        //
        if (data is not FloatSampleFrame frame)
        {
            return;
        }

        var signals = new DiscreteSignal[frame.ChannelCount];

        for (var i = 0; i < signals.Length; i++)
        {
            signals[i] = new DiscreteSignal(frame.AudioFormat.SampleRate, frame.SampleCount);
        }

        var interleaveIndex = 0;

        for (var i = 0; i < frame.SampleCount; i++)
        {
            for (var j = 0; j < frame.ChannelCount; j++)
            {
                signals[j].Samples[i] = frame.InterleavedSampleData[interleaveIndex++];
            }
        }
        
        for (var j = 0; j < frame.ChannelCount; j++)
        {
            var n = _resamplerC.Resample(signals[j], _backend.DesiredAudioFormat.SampleRate, new FirFilter());
            
            if (n is null)
                return;
        
            signals[j] = n;
        }

        interleaveIndex = 0;

        var nF = FloatSampleFrame.Create(signals[0].Length, _backend.DesiredAudioFormat.ChannelCount,
            _backend.DesiredAudioFormat);

        for (var i = 0; i < nF.SampleCount; i++)
        {
            for (var j = 0; j < nF.ChannelCount; j++)
            {
                nF.InterleavedSampleData[interleaveIndex++] =
                    signals[j].Samples[i];
            }
        }
        
        frame.Dispose();
        _dsp?.Receive(nF);
    }

    /// <inheritdoc />
    public void Connect(IPlaybackController priorNode, IDSPStack targetNode)
    {
        _playback = priorNode;
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
}