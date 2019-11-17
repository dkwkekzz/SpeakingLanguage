using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SpeakingLanguage.Library
{
    public struct Writer
    {
        private byte[] _buffer;
        private int _offset;

        public byte[] Buffer => _buffer;
        public int Offset => _offset;
        public int Remained => _buffer.Length - _offset;

        public Writer(int capacity, bool pooling = false)
        {
            if (pooling)
                _buffer = Library.Locator.BufferPool.GetBuffer(capacity);
            else
                _buffer = new byte[capacity];
            _offset = 0;
        }

        public Writer(byte[] buffer, int startIndex)
        {
            _buffer = buffer;
            _offset = startIndex;
        }

        public void Reset()
        {
            _offset = 0;
        }

        public void Expand()
        {
            var newBuf = new byte[_buffer.Length << 1];
            System.Buffer.BlockCopy(_buffer, 0, newBuf, 0, _offset);
            _buffer = newBuf;
        }

        public void WriteSuccess()
        {
            WriteBoolean(true);
        }

        public void WriteFailure(int error)
        {
            WriteBoolean(false);
            WriteInt(error);
        }

        public void WriteBoolean(bool v)
        {
            Library.BitConverter.GetBytes(_buffer, ref _offset, v);
        }

        public void WriteInt(int v)
        {
            Library.BitConverter.GetBytes(_buffer, ref _offset, v);
        }

        public void WriteLong(long v)
        {
            Library.BitConverter.GetBytes(_buffer, ref _offset, v);
        }

        public void WriteFloat(float v)
        {
            Library.BitConverter.GetBytes(_buffer, ref _offset, v);
        }

        public void WriteString(string v)
        {
            Library.BitConverter.GetBytes(_buffer, ref _offset, v);
        }

        public unsafe void WriteBytes(byte[] v)
        {
            fixed (void* vp = &v[0])
            fixed (void* bp = &_buffer[_offset])
            {
                System.Buffer.MemoryCopy(vp, bp, v.Length, v.Length);
                _offset += v.Length;
            }
        }
        
        public unsafe void WriteMemory(void* p, int sz)
        {
            fixed (void* bp = &_buffer[_offset])
            {
                System.Buffer.MemoryCopy(p, bp, sz, sz);
                _offset += sz;
            }
        }

        public void WriteStream(Stream stream)
        {
            stream.Position = 0;
            stream.Read(_buffer, _offset, (int)stream.Length);
            _offset += (int)stream.Length;
        }

        public void Flush(Stream stream)
        {
            stream.Write(_buffer, 0, _offset);
        }

        public async Task FlushAsync(Stream stream)
        {
            await stream.WriteAsync(_buffer, 0, _offset);
        }

        public byte[] GetResizedBuffer()
        {
            var newBuf = new byte[_offset];
            System.Buffer.BlockCopy(_buffer, 0, newBuf, 0, _offset);
            return newBuf;
        }

        public MemoryStream GetStream()
        {
            return new MemoryStream(_buffer, 0, _offset);
        }
    }
}
