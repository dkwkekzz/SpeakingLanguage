using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnFactory<TAllocator, T> : IumnFactory<T>
        where TAllocator : unmanaged, IumnAllocator
        where T : unmanaged
    {
        private readonly TAllocator* _allocator;
        private readonly int _szNode;
        private readonly umnArray<TAllocator, T> _temp;

        public umnFactory(TAllocator* allocator, int capacity)
        {
            _allocator = allocator;
            _szNode = Marshal.SizeOf(typeof(T));
            _temp = umnArray<TAllocator, T>.Allocate_umnArray(_allocator, capacity);
        }

        public T* GetObject()
        {
            if (_temp.Length > 0)
                return _temp.PopBack();

            var chk = _allocator->Alloc(_szNode);
            return (T*)chk->ptr;
        }

        public void PutObject(T* x)
        {
            _temp.PushBack(x);
        }
    }
}
