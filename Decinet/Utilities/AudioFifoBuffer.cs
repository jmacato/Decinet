namespace Decinet.Utilities;

public class AudioFifoBuffer
{
    private readonly int _bufferSize;
    private readonly byte[] _mainBuffer;
    private int _availableDataIndex;
    private int _pullDataIndex;
    private readonly object _bufferLock;

    /// <summary>
    /// Handles First-in/First-out buffering for audio backends that needs critical timing.
    /// </summary>
    /// <param name="size"></param>
    /// <param name="sampleRate"></param>
    /// <param name="bytesPerSample"></param>
    /// <param name="channelCount"></param>
    public AudioFifoBuffer(TimeSpan size, int sampleRate, int bytesPerSample, int channelCount)
    {
        _bufferSize = (int) Math.Round(size.TotalSeconds * channelCount * sampleRate * bytesPerSample);
        _mainBuffer = new byte[_bufferSize];
        _availableDataIndex = 0;
        _pullDataIndex = 0;
        _bufferLock = new object();
    }

    /// <summary>
    /// Stores the incoming raw audio data to be buffered.
    /// </summary>
    /// <param name="incomingBytes"></param>
    /// <returns></returns>
    public bool TryPushData(byte[] incomingBytes)
    {
        lock (_bufferLock)
        {
            var inLen = incomingBytes.Length;

            // the buffer is already full, tell the caller that we can't push
            // the incoming data.
            if (_availableDataIndex + inLen > _bufferSize)
            {
                return false;
            }

            Array.Copy(incomingBytes, 0, _mainBuffer, _availableDataIndex, inLen);
            _availableDataIndex += incomingBytes.Length;

            return true;
        }
    }

    /// <summary>
    /// Gets the data needed for the backend to play with.
    /// It returns silence in case there's no audio data
    /// available in the internal buffer.
    /// </summary>
    /// <param name="expectedBytes"></param>
    /// <param name="outData"></param>
    public void GetData(int expectedBytes, out byte[] outData)
    {
        lock (_bufferLock)
        {
            outData = new byte[expectedBytes];

            // if the remaining bytes are less than the available,
            // just get everything out.
            var remainingLength = Math.Min(expectedBytes, _bufferSize - _pullDataIndex);

            // we dont have any more data to send...
            // so let's send the remaining stuff + padding empty bytes.
            if (remainingLength == 0)
            {
                _availableDataIndex = 0;
                _pullDataIndex = 0;
                return;
            }
            
            _mainBuffer.AsSpan(_pullDataIndex, remainingLength).CopyTo(outData);

            _pullDataIndex += remainingLength;
        }
    }
}