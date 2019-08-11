using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnDynamicArray
    {
        private struct _token
        {
            public long prevEP;
            public IntPtr typeHandle;
        }

        private readonly IntPtr _root;
        private IntPtr _head;
        private long _EP;

        public int Capacity { get; }
        public int Offset => (int)((long)_head - (long)_root);

        public static umnDynamicArray AllocateNew<TAllocator>(TAllocator* allocator, int capacity)
            where TAllocator : unmanaged, IumnAllocator
        {
            var chk = allocator->Calloc(capacity);
            return new umnDynamicArray(chk);
        }

        public umnDynamicArray(umnChunk* chk)
        {
            _root = umnChunk.GetPtr(chk);
            _head = _root;
            _EP = (long)_head;
            Capacity = umnChunk.GetLength(chk);
        }

        public void Reset()
        {
            UnmanagedHelper.Memset(_root.ToPointer(), 0, Capacity);
        }

        public void Entry<T>() where T : unmanaged
        {
            var ptkn = (_token*)_head;
            ptkn->prevEP = _EP;
            ptkn->typeHandle = typeof(T).TypeHandle.Value;

            var tsz = sizeof(_token);
            _head += tsz;
            _EP = (long)_head;
        }

        public void Exit()
        {
            var tsz = sizeof(_token);
            _head -= tsz;

            var ptkn = (_token*)_head;
            _EP = ptkn->prevEP;
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

            if (_EP == (long)_head)
                return null;

            var sz = sizeof(T);
            _head -= sz;
            return (T*)_head;
        }
    }
}
