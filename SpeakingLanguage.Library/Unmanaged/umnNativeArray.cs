using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnNativeArray
    {
        private readonly IntPtr _root;
        private IntPtr _head;

        public int Capacity { get; }
        public int Offset => (int)((long)_head - (long)_root);
        
        public static umnNativeArray AllocateNew<TAllocator>(TAllocator* allocator, int capacity)
            where TAllocator : unmanaged, IumnAllocator
        {
            var chk = allocator->Calloc(capacity);
            return new umnNativeArray(chk);
        }

        public umnNativeArray(umnChunk* chk)
        {
            _root = umnChunk.GetPtr(chk);
            _head = _root;
            Capacity = chk->length;
        }

        public void Reset()
        {
            UnmanagedHelper.Memset(_root.ToPointer(), 0, Capacity);
        }
        
        public void PushBack<T>(T* e) where T : unmanaged
        {
            var sz = sizeof(T);
            if (Offset + sz > Capacity)
                ThrowHelper.ThrowCapacityOverflow($"Offset:{Offset.ToString()} / Capacity:{Capacity.ToString()}");
            
            Buffer.MemoryCopy(e, _head.ToPointer(), sz, sz);
            _head += sz;
        }

        public T* PopBack<T>() where T : unmanaged
        {
            if (Offset <= 0)
                ThrowHelper.ThrowCapacityOverflow($"Offset:{Offset.ToString()}");
            
            var sz = sizeof(T);
            _head -= sz;
            return (T*)_head;
        }
    }
}
