using System;
using System.Collections.Generic;
using System.IO;

namespace SpeakingLanguage.Library
{
    public struct Reader
    {
        private byte[] _buffer;
        private int _offset;
        private int _length;

        public bool EOB => _buffer.Length <= _offset + 1;
        public int Position => _offset;
        public int LengthToRead => _length - _offset;
        public byte[] Buffer => _buffer;

        public Reader(byte[] buf)
        {
            _buffer = buf;
            _offset = 0;
            _length = buf.Length;
        }

        public Reader(byte[] buf, int length)
        {
            _buffer = buf;
            _offset = 0;
            _length = length;
        }

        public Reader(byte[] buf, int offset, int length)
        {
            _buffer = buf;
            _offset = offset;
            _length = length;
        }

        public Reader(Stream stream)
        {
            _buffer = new byte[stream.Length];
            stream.Read(_buffer, 0, (int)stream.Length);
            _offset = 0;
            _length = _buffer.Length;
        }

        public bool ReadBoolean(out bool ret)
        {
            if (LengthToRead < sizeof(byte))
            {
                ret = false;
                return false;
            }

            ret = Library.BitConverter.ToBoolean(_buffer, ref _offset);
            return true;
        }

        public bool ReadInt(out int ret)
        {
            if (LengthToRead < sizeof(int))
            {
                ret = 0;
                return false;
            }

            ret = Library.BitConverter.ToInt(_buffer, ref _offset);
            return true;
        }

        public bool ReadLong(out long ret)
        {
            if (LengthToRead < sizeof(long))
            {
                ret = 0L;
                return false;
            }

            ret = Library.BitConverter.ToLong(_buffer, ref _offset);
            return true;
        }

        public bool ReadFloat(out float ret)
        {
            if (LengthToRead < sizeof(float))
            {
                ret = 0;
                return false;
            }

            ret = Library.BitConverter.ToSingle(_buffer, ref _offset);
            return true;
        }

        public bool ReadString(out string ret)
        {
            ret = Library.BitConverter.ToString(_buffer, ref _offset);
            return true;
        }

        public unsafe bool ReadMemory(void* destPtr, int sz)
        {
            if (LengthToRead < sz)
                return false;

            fixed (void* bp = &_buffer[_offset])
            {
                System.Buffer.MemoryCopy(bp, destPtr, sz, sz);
                _offset += sz;
            }

            return true;
        }

        public Stream GetStream()
        {
            var stream = new MemoryStream(_buffer, _offset, _length);
            stream.Position = 0;
            return stream;
        }

        public bool ReadSkip(int len)
        {
            if (LengthToRead < len)
                return false;

            _offset += len;
            return true;
        }
    }
}
