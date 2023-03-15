using System.Buffers;
using System.Runtime.CompilerServices;

namespace Decinet.Utilities;

// Copyright The OpenTelemetry Authors under the Apache License.

/// <summary>
/// Lock-free implementation of single-reader multi-writer circular buffer.
/// </summary>
/// <typeparam name="T">The type of the underlying value.</typeparam>
public class CircularBuffer
{
    private readonly byte[] _buffer;
    private long _head;
    private long _tail;

    public CircularBuffer(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "capacity should be greater than zero.");
        }

        Capacity = capacity;
        _buffer = new byte[capacity];
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
            var tailSnapshot = _tail;
            return (int) (_head - tailSnapshot);
        }
    }

    /// <summary>
    /// Gets the number of items added to the <see cref="CircularBuffer{T}"/>.
    /// </summary>
    public long AddedCount => _head;

    /// <summary>
    /// Gets the number of items removed from the <see cref="CircularBuffer{T}"/>.
    /// </summary>
    public long RemovedCount => _tail;

    public bool IsFull => _tail - _head >= Capacity;

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
            var tailSnapshot = _tail;
            var headSnapshot = this._head;

            if (headSnapshot - tailSnapshot >= Capacity)
            {
                return false; // buffer is full
            }

            var head = Interlocked.CompareExchange(ref this._head, headSnapshot + 1, headSnapshot);
            if (head != headSnapshot)
            {
                continue;
            }

            var index = (int) (head % Capacity);
            _buffer[index] = value;
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
            var tailSnapshot = _tail;
            var headSnapshot = this._head;

            if (headSnapshot - tailSnapshot >= Capacity)
            {
                return false; // buffer is full
            }

            var head = Interlocked.CompareExchange(ref this._head, headSnapshot + 1, headSnapshot);
            if (head != headSnapshot)
            {
                if (spinCountDown-- == 0)
                {
                    return false; // exceeded maximum spin count
                }

                continue;
            }

            var index = (int) (head % Capacity);
            _buffer[index] = value;
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
        var index = (int) (_tail % Capacity);
        var value = _buffer[index];
        _buffer[index] = 0;
        _tail++;
        return value;
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
    public byte[] ReadBytes(int length)
    {

        var ret = new byte[length];


        for (var i = 0; i < length; i++)
        {
            
            var index = (int) (_tail % Capacity);
           ret[i] = _buffer[index];
            _buffer[index] = 0;
            _tail++;
        }
        
        
        
        return ret;
    }
}