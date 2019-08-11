﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnArray<T> : IDisposable
        where T : unmanaged
    {
        public struct Enumerator
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

            public T* Current => (T*)(_chk->Ptr + _ofs);

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
        private int _length;

        public int Capacity => _chk->length;
        public int Length => _length;

        public T* this[int index]
        {
            get
            {
                if (Capacity <= _szElement * index)
                    ThrowHelper.ThrowCapacityOverflow($"wrong index in Indexer_get:{Capacity.ToString()}");

                var ofs = index * _szElement;
                return (T*)(_chk->Ptr + ofs);
            }
            set
            {
                if (Capacity <= _szElement * index)
                    ThrowHelper.ThrowCapacityOverflow($"wrong index in Indexer_set:{Capacity.ToString()}");

                var ofs = index * _szElement;
                var ptr = _chk->Ptr + ofs;
                Buffer.MemoryCopy(value, ptr.ToPointer(), _szElement, _szElement);
            }
        }
        
        public static umnArray<T> AllocateNew<TAllocator>(TAllocator* allocator, int maxLength)
            where TAllocator : unmanaged, IumnAllocator
        {
            var sz = sizeof(T);
            var chk = allocator->Calloc(maxLength * sz);
            return new umnArray<T>(chk);
        }

        public umnArray(umnChunk* chk)
        {
            _chk = chk;
            _szElement = Marshal.SizeOf(typeof(T));
            _length = 0;
        }
        
        public Enumerator GetEnumerator()
        {
            return new Enumerator(_chk, _length);
        }

        public void PushBack(T* e)
        {
            if (Capacity <= _szElement * _length)
                ThrowHelper.ThrowCapacityOverflow($"Capacity:{Capacity.ToString()}");

            this[_length++] = e;
        }

        public T* PopBack()
        {
            if (_length <= 0)
                ThrowHelper.ThrowCapacityOverflow($"_length:{_length.ToString()}");

            return this[--_length];
        }

        public void Dispose()
        {
            _chk->Disposed = true;
        }
    }
}
