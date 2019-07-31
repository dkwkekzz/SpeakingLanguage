using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnDynamicArray : IDisposable
    {
        private readonly umnChunk* _rootChk;
        private umnChunk* _headChk;
        private IntPtr _head;

        public int Capacity => _rootChk->length;
        public long Offset => _head.ToInt64() - _rootChk->ptr.ToInt64();
        public bool IsHead => _head == _headChk->ptr;
        public IntPtr CurrentTypeHandle => _headChk->typeHandle;

        public static umnDynamicArray AllocateNew<TAllocator>(TAllocator* allocator, int capacity)
            where TAllocator : unmanaged, IumnAllocator
        {
            var chk = allocator->Calloc(capacity);
            return new umnDynamicArray(chk);
        }

        //public static umnDynamicArray* AllocateNew<TAllocator>(TAllocator* allocator, int capacity)
        //    where TAllocator : unmanaged, IumnAllocator
        //{
        //    var szArr = sizeof(umnDynamicArray);
        //    var arrChk = allocator->Alloc(szArr + sizeof(umnChunk));
        //    var chk = allocator->Calloc(capacity);
        //
        //    var arr = new umnDynamicArray(chk);
        //    var destPtr = arrChk->ptr.ToPointer();
        //    Buffer.MemoryCopy(&arr, destPtr, szArr, szArr);
        //
        //    return (umnDynamicArray*)destPtr;
        //}

        public umnDynamicArray(umnChunk* chk)
        {
            _rootChk = chk;
            _head = _rootChk->ptr;
            _headChk = null;
        }

        public void PushChunk(IntPtr typeHandle)
        {
            if (null != _headChk)
                _headChk->length = (int)(_head.ToInt64() - _headChk->ptr.ToInt64());

            var chkSize = StructSize.umnChunk;
            var chk = (umnChunk*)_head;
            chk->ptr = _head + chkSize;
            chk->length = 0;
            chk->typeHandle = typeHandle;
            chk->dispose = false;
            _head += chkSize;

            if (null != _headChk)
                _headChk->next = chk;
            chk->prev = _headChk;
            _headChk = chk;
        }

        public void PopChunk()
        {
            if (null == _headChk)
                ThrowHelper.ThrowWrongState("Please call Begin first.");

            var prevChk = _headChk;
            prevChk->dispose = true;

            _headChk = prevChk->prev;
            _headChk->next = null;
            _head = _headChk->ptr + _headChk->length;
        }

        public void PushBack(void* e, int sz)
        {
            if (null == _headChk)
                ThrowHelper.ThrowWrongState("Please call Begin first.");
            //if (Capacity <= Offset)
            //    ThrowHelper.ThrowCapacityOverflow($"Capacity/Offset:{Capacity.ToString()}/{Offset.ToString()}");
            
            Buffer.MemoryCopy(e, _head.ToPointer(), sz, sz);
            _head += sz;
        }

        public void* PopBack(int sz)
        {
            if (null == _headChk)
                ThrowHelper.ThrowWrongState("Please call Begin first.");

            if (_head == _headChk->ptr)
                return null;

            _head -= sz;
            return _head.ToPointer();
        }

        public void Dispose()
        {
            _rootChk->dispose = true;
        }
    }
}
