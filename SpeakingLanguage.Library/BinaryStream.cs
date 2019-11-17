using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public class BinaryStream
    {
        private byte[] _buffer;
        private int _wofs;
        private int _rofs;

        public int LengthToRead => _wofs - _rofs;

        public BinaryStream(int capacity)
        {
            _buffer = new byte[capacity];
        }
        
        public void Write(bool v)
        {
            Library.BitConverter.GetBytes(_buffer, ref _wofs, v);
        }

        public void Write(int v)
        {
            Library.BitConverter.GetBytes(_buffer, ref _wofs, v);
        }

        public void Write(long v)
        {
            Library.BitConverter.GetBytes(_buffer, ref _wofs, v);
        }

        public void Write(float v)
        {
            Library.BitConverter.GetBytes(_buffer, ref _wofs, v);
        }

        public void Write(string v)
        {
            Library.BitConverter.GetBytes(_buffer, ref _wofs, v);
        }

        public bool Read(out bool ret)
        {
            if (LengthToRead < sizeof(byte))
            {
                ret = false;
                return false;
            }

            ret = Library.BitConverter.ToBoolean(_buffer, ref _rofs);
            return true;
        }

        public bool Read(out int ret)
        {
            if (LengthToRead < sizeof(int))
            {
                ret = 0;
                return false;
            }

            ret = Library.BitConverter.ToInt(_buffer, ref _rofs);
            return true;
        }

        public bool Read(out long ret)
        {
            if (LengthToRead < sizeof(long))
            {
                ret = 0L;
                return false;
            }

            ret = Library.BitConverter.ToLong(_buffer, ref _rofs);
            return true;
        }

        public bool Read(out float ret)
        {
            if (LengthToRead < sizeof(float))
            {
                ret = 0;
                return false;
            }

            ret = Library.BitConverter.ToSingle(_buffer, ref _rofs);
            return true;
        }

        public bool Read(out string ret)
        {
            ret = Library.BitConverter.ToString(_buffer, ref _rofs);
            return true;
        }
    }
}
