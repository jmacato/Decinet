using System;

namespace LibResample.Sharp
{
    internal class FloatBuffer
    {
        private readonly float[] _buffer;
        private int _position;
        private readonly int _start;
        private readonly int _end;

        private FloatBuffer(float[] buffer, int offset, int len)
        {
            if (buffer == null || buffer.Length == 0)
            {
                throw new ArgumentException("buffer could not be null or empty");
            }
            if (offset < 0)
            {
                throw new ArgumentException("offset must be greater than 0");
            }
            if (offset > buffer.Length - 1)
            {
                throw new ArgumentException(string.Format("offset {0} is greater than buffer size {1}", offset, buffer.Length));
            }
            if (len < 0)
            {
                throw new ArgumentException("length can not be less than 1");
            }
            if (offset + len > buffer.Length)
            {
                throw new ArgumentException("Exceed bound!");
            }

            _buffer = buffer;
            _position = 0;
            _start = offset;
            _end = _start + len;
            Length = len;
        }

        public static FloatBuffer Wrap(float[] buffer, int offset, int len)
        {
            return new FloatBuffer(buffer, offset, len);
        }

        public int Get(float[] floatArr, int offset, int len)
        {
            var accLen = RemainLength < len ? RemainLength : len;
            Array.Copy(_buffer, _start + _position, floatArr, offset, accLen);
            _position += accLen;
            return accLen;
        }

        public int Put(float[] floatArr, int offset, int len)
        {
            var accLen = RemainLength < len ? RemainLength : len;
            Array.Copy(floatArr, offset, _buffer, _start + _position, accLen);
            _position += accLen;
            return accLen;
        }

        public int RemainLength
        {
            get { return Length - _position; }
        }

        public int Length { get; private set; }

        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (value < 0 || value > Length)
                {
                    throw new ArgumentException("out of bound");
                }
                _position = value;
            }
        }
    }
}
