namespace Decinet.Backend.macOS;

using System;
using System.Collections.Generic;

/// <summary>
/// Buffers up encoded audio packets and provides a constant stream of sound (silence if there is no more audio to decode)
/// </summary>
public class AudioDecodingBuffer
{
    private readonly int _sampleRate;

    // private readonly ushort _frameSize;
    private int _decodedOffset;
    private int _decodedCount;
    private readonly byte[] _decodedBuffer;


    /// <summary>
    /// Initializes a new instance of the <see cref="AudioDecodingBuffer"/> class which Buffers up encoded audio packets and provides a constant stream of sound (silence if there is no more audio to decode).
    /// </summary>
    /// <param name="sampleRate">The sample rate in Hertz (samples per second).</param>
    /// <param name="sampleChannels">The sample channels (1 for mono, 2 for stereo).</param>
    public AudioDecodingBuffer(int sampleRate, byte sampleBytes, byte sampleChannels)
    {
        _sampleRate = sampleRate;
        _decodedBuffer = new byte[sampleRate * (sampleBytes) * sampleChannels];
    }

    private long _nextSequenceToDecode;
    private readonly List<BufferPacket> _encodedBuffer = new List<BufferPacket>();


    /// <summary>
    /// The time, in milliseconds, for the jitter buffer to delay when network data is exhausted. Only updates internally when jitter is detected.
    /// </summary>
    public TimeSpan JitterDelay { get; set; } = TimeSpan.FromMilliseconds(350f);

    private bool isJitterDetected;
    private bool isJitterTimerRunning;
    private DateTime jitterTimer = DateTime.UtcNow;
    private double jitterMillis = 350f;

    public int Read(byte[] buffer, int offset, int count)
    {
        int readCount = 0;

        if (isJitterTimerRunning && ((DateTime.UtcNow - jitterTimer).TotalMilliseconds > jitterMillis))
        {
            isJitterDetected = false;
            isJitterTimerRunning = false;
        }

        if (!isJitterDetected)
        {
            while (readCount < count)
            {
                readCount += ReadFromBuffer(buffer, offset + readCount, count - readCount);

                if (readCount == 0)
                {
                    isJitterDetected = true;
                }

                //Try to decode some more data into the buffer
                if (!FillBuffer())
                    break;
            }
        }

        if (readCount == 0)
        {
            //Return silence
            Array.Clear(buffer, 0, count);
            return count;
        }

        return readCount;
    }

    /// <summary>
    /// Add a new packet of encoded data
    /// </summary>
    /// <param name="sequence">Sequence number of this packet</param>
    /// <param name="data">The encoded audio packet</param>
    /// <param name="codec">The codec to use to decode this packet</param>
    public void AddEncodedPacket(long sequence, byte[] data)
    {
        if (isJitterDetected && !isJitterTimerRunning)
        {
            jitterTimer = DateTime.UtcNow;
            jitterMillis = JitterDelay.TotalMilliseconds;
            isJitterTimerRunning = true;
        }

        if (sequence == 0)
            _nextSequenceToDecode = 0;
        
        //If the next seq we expect to decode comes after this packet we've already missed our opportunity!
        if (_nextSequenceToDecode > sequence)
            return;

        _encodedBuffer.Add(new BufferPacket
        {
            Data = data,
            Sequence = sequence
        });
    }

    private BufferPacket? GetNextEncodedData()
    {
        if (_encodedBuffer.Count == 0)
            return null;

        int minIndex = 0;
        for (int i = 1; i < _encodedBuffer.Count; i++)
            minIndex = _encodedBuffer[minIndex].Sequence < _encodedBuffer[i].Sequence ? minIndex : i;

        var packet = _encodedBuffer[minIndex];
        _encodedBuffer.RemoveAt(minIndex);

        return packet;
    }

    /// <summary>
    /// Read data that has already been decoded
    /// </summary>
    /// <param name="dst"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private int ReadFromBuffer(byte[] dst, int offset, int count)
    {
        //Copy as much data as we can from the buffer up to the limit
        int readCount = Math.Min(count, _decodedCount);
        Array.Copy(_decodedBuffer, _decodedOffset, dst, offset, readCount);
        _decodedCount -= readCount;
        _decodedOffset += readCount;

        //When the buffer is emptied, put the start offset back to index 0
        if (_decodedCount == 0)
            _decodedOffset = 0;

        //If the offset is nearing the end of the buffer then copy the data back to offset 0
        if ((_decodedOffset > _decodedCount) && (_decodedOffset + _decodedCount) > _decodedBuffer.Length * 0.9)
            Buffer.BlockCopy(_decodedBuffer, _decodedOffset, _decodedBuffer, 0, _decodedCount);

        return readCount;
    }

    /// <summary>
    /// Decoded data into the buffer
    /// </summary>
    /// <returns></returns>
    private bool FillBuffer()
    {
        var packet = GetNextEncodedData();
        if (!packet.HasValue)
            return false;

        ////todo: _nextSequenceToDecode calculation is wrong, which causes this to happen for almost every packet!
        ////Decode a null to indicate a dropped packet
        //if (packet.Value.Sequence != _nextSequenceToDecode)
        //    _codec.Decode(null);

        var d = packet.Value.Data;
        _nextSequenceToDecode = packet.Value.Sequence + d.Length / (_sampleRate );

        Array.Copy(d, 0, _decodedBuffer, _decodedOffset, d.Length);
        _decodedCount += d.Length;
        return true;
    }

    private struct BufferPacket
    {
        public byte[] Data;
        public long Sequence;
    }
}