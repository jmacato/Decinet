using System.Runtime.CompilerServices;

namespace Decinet.Utilities;

// Copyright The OpenTelemetry Authors under the Apache License.

/// <summary>
/// Lock-free implementation of single-reader multi-writer circular buffer.
/// </summary>
/// <typeparam name="T">The type of the underlying value.</typeparam>
public class CircularBuffer
{
    private readonly byte[] buffer;
    private long head;
    private long tail;

    public CircularBuffer(TimeSpan size, int sampleRate, int bytesPerSample, int channelCount)
    {
        var capacity = (int) Math.Round(size.TotalSeconds * channelCount * sampleRate * bytesPerSample);

        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "capacity should be greater than zero.");
        }

        Capacity = capacity;
        buffer = new byte[capacity];
    }

    /// <summary>
    /// Gets the capacity of the <see cref="CircularBuffer{T}"/>.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets the number of items contained in the <see cref="CircularBuffer{T}"/>.
    /// </summary>
    public int Count
    {
        get
        {
            var tailSnapshot = tail;
            return (int) (head - tailSnapshot);
        }
    }

    /// <summary>
    /// Gets the number of items added to the <see cref="CircularBuffer{T}"/>.
    /// </summary>
    public long AddedCount => head;

    /// <summary>
    /// Gets the number of items removed from the <see cref="CircularBuffer{T}"/>.
    /// </summary>
    public long RemovedCount => tail;

    public bool IsFull => tail - head >= Capacity;

    /// <summary>
    /// Adds the specified item to the buffer.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>
    /// Returns <c>true</c> if the item was added to the buffer successfully;
    /// <c>false</c> if the buffer is full.
    /// </returns>
    public bool Add(byte value)
    {
        while (true)
        {
            var tailSnapshot = tail;
            var headSnapshot = this.head;

            if (headSnapshot - tailSnapshot >= Capacity)
            {
                return false; // buffer is full
            }

            var head = Interlocked.CompareExchange(ref this.head, headSnapshot + 1, headSnapshot);
            if (head != headSnapshot)
            {
                continue;
            }

            var index = (int) (head % Capacity);
            buffer[index] = value;
            return true;
        }
    }

    /// <summary>
    /// Attempts to add the specified item to the buffer.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <param name="maxSpinCount">The maximum allowed spin count, when set to a negative number or zero, will spin indefinitely.</param>
    /// <returns>
    /// Returns <c>true</c> if the item was added to the buffer successfully;
    /// <c>false</c> if the buffer is full or the spin count exceeded <paramref name="maxSpinCount"/>.
    /// </returns>
    public bool TryAdd(byte value, int maxSpinCount)
    {
        if (maxSpinCount <= 0)
        {
            return Add(value);
        }

        var spinCountDown = maxSpinCount;

        while (true)
        {
            var tailSnapshot = tail;
            var headSnapshot = this.head;

            if (headSnapshot - tailSnapshot >= Capacity)
            {
                return false; // buffer is full
            }

            var head = Interlocked.CompareExchange(ref this.head, headSnapshot + 1, headSnapshot);
            if (head != headSnapshot)
            {
                if (spinCountDown-- == 0)
                {
                    return false; // exceeded maximum spin count
                }

                continue;
            }

            var index = (int) (head % Capacity);
            buffer[index] = value;
            return true;
        }
    }

    /// <summary>
    /// Reads an item from the <see cref="CircularBuffer{T}"/>.
    /// </summary>
    /// <remarks>
    /// This function is not reentrant-safe, only one reader is allowed at any given time.
    /// Warning: There is no bounds check in this method. Do not call unless you have verified Count > 0.
    /// </remarks>
    /// <returns>Item read.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte Read()
    {
        var index = (int) (tail % Capacity);
        var value = buffer[index];
        buffer[index] = 0;
        tail++;
        return value;
    }
}