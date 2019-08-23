using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnStack : IumnAllocator, IDisposable
    {
        private umnChunk* _rootChk;
        private IntPtr _rootPtr;
        private int _head;

        public umnChunk* Root => _rootChk;
        public int Capacity => _rootChk->length;
        public bool IsCreated => _rootChk != null;

        public umnStack(umnChunk* chk)
        {
            _rootChk = chk;
            _rootPtr = umnChunk.GetPtr(chk);
            _head = 0;
        }

        public void Reset()
        {
            _head = 0;
        }

        public umnChunk* Alloc(int size)
        {
            var szChk = umnSize.umnChunk;
            var totalSize = size + szChk;
            var remained = _rootChk->length - _head;
            if (remained < totalSize)
                return null;

            var endChk = (umnChunk*)(_rootPtr + _head + totalSize);
            endChk->length = 0;
            
            var chk = (umnChunk*)(_rootPtr + _head);
            chk->length = size;

            _head += totalSize;

            return chk;
        }

        public umnChunk* Calloc(int size)
        {
            var chk = Alloc(size);
            if (null == chk)
                return null;

            UnmanagedHelper.Memset(chk->Ptr.ToPointer(), 0, size);
            return chk;
        }
        
        public void Push(void* src, int size)
        {
            var szChk = umnSize.umnChunk;
            var remained = _rootChk->length - _head;
            if (remained < size)
                ThrowHelper.ThrowOutOfMemory("at umnStack::PushBack");

            var dest = (_rootPtr + _head).ToPointer();
            Buffer.MemoryCopy(src, dest, size, size);
            _head += size;
        }

        public void CopyTo(void* dest)
        {
            var ofs = _head;
            var ptr = umnChunk.GetPtr(_rootChk);
            Buffer.MemoryCopy(ptr.ToPointer(), dest, ofs, ofs);
        }

        public void CopyTo(IntPtr dest)
        {
            var ofs = _head;
            var ptr = umnChunk.GetPtr(_rootChk);
            Buffer.MemoryCopy(ptr.ToPointer(), dest.ToPointer(), ofs, ofs);
        }

        public void Dispose()
        {
            _rootChk->Disposed = true;
        }

        public void Display()
        {
            Tracer.Write($"=====[umnStack]=====");
            Tracer.Write($"capacity: {Capacity.ToString()}");
            Tracer.Write($"per: {_head.ToString()} / {Capacity.ToString()}");
            Tracer.Write($"======================");
        }

        public static void Swap(ref umnStack lhs, ref umnStack rhs)
        {
            var tempRootChk = lhs._rootChk;
            var tempRootPtr = lhs._rootPtr;
            var tempHead = lhs._head;
            lhs._rootChk = rhs._rootChk;
            lhs._rootPtr = rhs._rootPtr;
            lhs._head = rhs._head;
            rhs._rootChk = tempRootChk;
            rhs._rootPtr = tempRootPtr;
            rhs._head = tempHead;
        }
    }
}
