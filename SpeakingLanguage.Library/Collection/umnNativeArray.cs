using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnNativeArray : IDisposable
    {
        private readonly umnChunk* _chk;
        private readonly int _szElement;
        private int _length;

        public int Capacity => _chk->length;
        public int Length => _length;

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
        
        public static umnNativeArray Allocate_umnNativeArray<TAllocator, T>(TAllocator* allocator, int capacity)
            where TAllocator : unmanaged, IumnAllocator
            where T : unmanaged
        {
            var chk = allocator->Calloc(capacity);
            return new umnNativeArray(chk, sizeof(T));
        }

        public umnNativeArray(umnChunk* chk, int size)
        {
            _chk = chk;
            _szElement = size;
            _length = 0;
        }
        
        public void PushBack(void* e)
        {
            if (Capacity <= _szElement * _length)
                ThrowHelper.ThrowCapacityOverflow($"Capacity:{Capacity.ToString()}");

            this[_length++] = e;
        }

        public void* PopBack()
        {
            if (_length <= 0)
                ThrowHelper.ThrowCapacityOverflow($"_length:{_length.ToString()}");

            return this[--_length];
        }

        public void Dispose()
        {
            _chk->dispose = true;
        }
    }
}
