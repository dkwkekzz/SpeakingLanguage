using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnNativeArray : IDisposable
    {
        private readonly umnChunk* _chk;
        private readonly int _szElement;
        private int _index;

        public int Capacity => _chk->length;
        public int Length => _index;

        public void* this[int index]
        {
            get
            {
                if (Capacity <= _szElement * index)
                    ThrowHelper.ThrowCapacityOverflow($"wrong index in Indexer_get:{Capacity.ToString()}");

                var ofs = index * _szElement;
                return (_chk->ptr + ofs).ToPointer();
            }
            set
            {
                if (Capacity <= _szElement * index)
                    ThrowHelper.ThrowCapacityOverflow($"wrong index in Indexer_set:{Capacity.ToString()}");

                var ofs = index * _szElement;
                var ptr = _chk->ptr + ofs;
                Buffer.MemoryCopy(value, ptr.ToPointer(), _szElement, _szElement);
            }
        }
        
        public static umnNativeArray AllocateNew<TAllocator, T>(TAllocator* allocator, int maxLength)
            where TAllocator : unmanaged, IumnAllocator
            where T : unmanaged
        {
            var sz = sizeof(T);
            var chk = allocator->Calloc(maxLength * sz);
            return new umnNativeArray(chk, sz);
        }

        public umnNativeArray(umnChunk* chk, int size)
        {
            _chk = chk;
            _szElement = size;
            _index = 0;
        }

        public void Reset()
        {
            _index = 0;
            UnmanagedHelper.Memset(_chk->ptr.ToPointer(), 0, Capacity);
        }
        
        public void PushBack(void* e)
        {
            if (Capacity <= _szElement * _index)
                ThrowHelper.ThrowCapacityOverflow($"Capacity:{Capacity.ToString()}");

            this[_index++] = e;
        }

        public void PushBack(void* e, int sz)
        {
            if (Capacity <= _szElement * (_index - 1) + sz)
                ThrowHelper.ThrowCapacityOverflow($"Capacity:{Capacity.ToString()}");
            
            var ptr = _chk->ptr + sz;
            Buffer.MemoryCopy(e, ptr.ToPointer(), sz, sz);
        }

        public void* PopBack()
        {
            if (_index <= 0)
                ThrowHelper.ThrowCapacityOverflow($"_length:{_index.ToString()}");

            return this[--_index];
        }

        public void Dispose()
        {
            _chk->dispose = true;
        }
    }
}
