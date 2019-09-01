using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnArray<T> : IEnumerable, IDisposable
        where T : unmanaged
    {
        public struct Enumerator : IEnumerator
        {
            private readonly umnChunk* _chk;
            private readonly int _capacity;
            private int _ofs;
            private int _sz;

            public Enumerator(umnChunk* chk, int len)
            {
                _sz = sizeof(T);
                _capacity = len * _sz;
                _chk = chk;
                _ofs = -_sz;
            }

            public T* Current => (T*)(umnChunk.GetPtr(_chk) + _ofs);
            object IEnumerator.Current => *Current;

            public bool MoveNext()
            {
                _ofs += _sz;
                if (_capacity <= _ofs)
                    return false;
                return true;
            }

            public void Reset()
            {
                _ofs = -_sz;
            }
        }

        private readonly umnChunk* _chk;
        private readonly int _szElement;
        
        public int Capacity { get; }
        public int Length { get; private set; }

        public T* this[int index]
        {
            get
            {
                var ofs = index * _szElement;
                var ptr = umnChunk.GetPtr(_chk);
                return (T*)(ptr + ofs);
            }
            set
            {
                var ofs = index * _szElement;
                var ptr = umnChunk.GetPtr(_chk) + ofs;
                Buffer.MemoryCopy(value, ptr.ToPointer(), _szElement, _szElement);
            }
        }
        
        public static umnArray<T> CreateNew<TAllocator>(ref TAllocator allocator, int maxLength)
            where TAllocator : unmanaged, IumnAllocator
        {
            var sz = sizeof(T);
            var chk = allocator.Calloc(maxLength * sz);
            return new umnArray<T>(chk);
        }

        public umnArray(umnChunk* chk)
        {
            _chk = chk;
            _szElement = Marshal.SizeOf(typeof(T));
            Length = 0;

            Capacity = chk->length;
        }

        public void Clear()
        {
            Length = 0;
        }

        public void CClear()
        {
            var ptr = umnChunk.GetPtr(_chk).ToPointer();
            UnmanagedHelper.Memset(ptr, 0, _chk->length);
            Length = 0;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_chk, Length);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T* PushBack(T* e)
        {
            if (Capacity <= _szElement * Length)
                ThrowHelper.ThrowCapacityOverflow($"Capacity:{Capacity.ToString()}");

            var ofs = Length++ * _szElement;
            var ptr = umnChunk.GetPtr(_chk) + ofs;
            Buffer.MemoryCopy(e, ptr.ToPointer(), _szElement, _szElement);
            return (T*)ptr;
        }
        
        public T* PopBack()
        {
            if (Length <= 0)
                ThrowHelper.ThrowCapacityOverflow($"Length:{Length.ToString()}");

            var ofs = --Length * _szElement;
            return (T*)(umnChunk.GetPtr(_chk) + ofs);
        }

        public void Dispose()
        {
            _chk->Disposed = true;
        }
    }
}
