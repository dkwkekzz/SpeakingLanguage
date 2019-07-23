using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SpeakingLanguage.Library
{
    public sealed class BufferPool : KeyObjectPool<int, byte[]>
    {
        public BufferPool() : base(capacity => { return new byte[capacity]; }, buffer => { })
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBuffer(int capacity)
        {
            return base.GetObject(1 << (Math.Log2ge(capacity)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PutBuffer(byte[] buffer)
        {
            base.PutObject(buffer.Length, buffer);
        }
    }
}
