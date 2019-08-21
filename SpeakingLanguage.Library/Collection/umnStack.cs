using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnStack : IumnAllocator, IDisposable
    {
        private umnChunk* _rootChk;
        private umnChunk* _headChk;

        public int Capacity => (int)((long)_rootChk->next - (long)_rootChk) - umnSize.umnChunk;

        public bool IsCreated => _rootChk != null;
        public umnChunk* Root => (umnChunk*)((IntPtr)_rootChk + umnSize.umnChunk);
        public long Offset => (long)_headChk - (long)_rootChk + umnSize.umnChunk;

        public umnStack(umnChunk* chk)
        {
            _rootChk = chk;
            _headChk = _rootChk + umnSize.umnChunk;
            _headChk->next = null;
        }

        public void Reset()
        {
            _headChk = _rootChk + umnSize.umnChunk;
        }

        public umnChunk* Alloc(int size)
        {
            var szChk = umnSize.umnChunk;
            var head = (long)_headChk;
            var tail = (long)_rootChk->next;

            var remained = tail - head;
            if (remained < size + szChk)
                return null;

            var endChk = (umnChunk*)(head + size);
            endChk->next = null;
            
            var chk = _headChk;
            chk->next = endChk;
            
            _headChk = endChk;

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

        public umnChunk* DupAlloc(umnChunk* chk)
        {

            var retChk = Alloc(umnChunk.GetLength(chk));
            retChk->typeHandle = chk->typeHandle;

            var originPtr = umnChunk.GetPtr()
        }

        public void CopyTo(void* dest)
        {
            var ofs = Offset;
            var ptr = umnChunk.GetPtr(Root);
            Buffer.MemoryCopy(ptr.ToPointer(), dest, ofs, ofs);
        }

        public void CopyTo(IntPtr dest)
        {
            var ofs = Offset;
            var ptr = umnChunk.GetPtr(Root);
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
            Tracer.Write($"per: {Offset.ToString()} / {Capacity.ToString()}");
            Tracer.Write($"======================");
        }

        public static void Swap(ref umnStack lhs, ref umnStack rhs)
        {
            var tempChk1 = lhs._rootChk;
            var tempChk2 = lhs._headChk;
            lhs._rootChk = rhs._rootChk;
            lhs._headChk = rhs._headChk;
            rhs._rootChk = tempChk1;
            rhs._headChk = tempChk2;
        }
    }
}
