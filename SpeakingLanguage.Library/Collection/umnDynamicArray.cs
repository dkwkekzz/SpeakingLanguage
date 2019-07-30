using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
        public int CurrentTypeIdx => _headChk->typeIdx;
        //public void* this[int index]
        //{
        //    get
        //    {
        //        if (Capacity <= _szElement * index)
        //            ThrowHelper.ThrowCapacityOverflow($"wrong index in Indexer_get:{Capacity.ToString()}");
        //
        //        var ofs = index * _szElement;
        //        return (_chk->ptr + ofs).ToPointer();
        //    }
        //    set
        //    {
        //        if (Capacity <= _szElement * index)
        //            ThrowHelper.ThrowCapacityOverflow($"wrong index in Indexer_set:{Capacity.ToString()}");
        //
        //        var ofs = index * _szElement;
        //        var ptr = _chk->ptr + ofs;
        //        Buffer.MemoryCopy(value, ptr.ToPointer(), _szElement, _szElement);
        //    }
        //}

        public static umnDynamicArray AllocateNew<TAllocator>(TAllocator* allocator, int capacity)
            where TAllocator : unmanaged, IumnAllocator
        {
            var chk = allocator->Calloc(capacity);
            return new umnDynamicArray(chk);
        }

        public umnDynamicArray(umnChunk* chk)
        {
            _rootChk = chk;
            _head = _rootChk->ptr;
            _headChk = null;
        }

        public void PushChunk(int typeIdx)
        {
            if (null != _headChk)
                _headChk->length = (int)(_head.ToInt64() - _headChk->ptr.ToInt64());

            var chkSize = StructSize.umnChunk;
            var chk = (umnChunk*)_head;
            chk->ptr = _head + chkSize;
            chk->length = 0;
            chk->typeIdx = typeIdx;
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
