using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnStack : IumnAllocator, IDisposable
    {
        private umnChunk* _rootChk;
        private IntPtr _rootPtr;
        private int _current;

        public umnChunk* Root => (umnChunk*)_rootPtr;
        public int Capacity => _rootChk->length;
        public bool IsCreated => _rootChk != null;
        public int Current => _current;

        public umnStack(umnChunk* chk)
        {
            _rootChk = chk;
            _rootPtr = umnChunk.GetPtr(chk);
            ((umnChunk*)_rootPtr)->typeHandle = 0;
            ((umnChunk*)_rootPtr)->length = 0;
            _current = 0;
        }

        public void Reset()
        {
            _current = 0;
        }

        public umnChunk* Alloc(int size)
        {
            var szChk = umnSize.umnChunk;
            var totalSize = size + szChk;
            var remained = _rootChk->length - _current;
            if (remained < totalSize)
                return null;

            var endChk = (umnChunk*)(_rootPtr + _current + totalSize);
            endChk->length = 0;
            
            var chk = (umnChunk*)(_rootPtr + _current);
            chk->typeHandle = 0;
            chk->length = size;

            _current += totalSize;

            return chk;
        }

        public umnChunk* Calloc(int size)
        {
            var chk = Alloc(size);
            if (null == chk)
                return null;

            var ptr = umnChunk.GetPtr(chk);
            UnmanagedHelper.Memset(ptr.ToPointer(), 0, size);
            return chk;
        }
        
        public void* Push(void* src, int size)
        {
            var szChk = umnSize.umnChunk;
            var remained = _rootChk->length - _current;
            if (remained < size)
                ThrowHelper.ThrowOutOfMemory("at umnStack::PushBack");

            var dest = (_rootPtr + _current).ToPointer();
            Buffer.MemoryCopy(src, dest, size, size);
            _current += size;
            return dest;
        }

        public void* Push(ref Reader reader)
        {
            var ret = reader.ReadInt(out int size);
            if (!ret)
                return null;

            if (size == 0)
                return null;
            
            var szChk = umnSize.umnChunk;
            var remained = _rootChk->length - _current;
            if (remained < size)
                ThrowHelper.ThrowOutOfMemory("at umnStack::PushBack");

            var dest = (_rootPtr + _current).ToPointer();
            reader.ReadMemory(dest, size);
            _current += size;
            return dest;
        }

        public void CopyTo(void* dest)
        {
            var ofs = _current;
            var ptr = umnChunk.GetPtr(_rootChk);
            Buffer.MemoryCopy(ptr.ToPointer(), dest, ofs, ofs);
        }

        public void CopyTo(IntPtr dest)
        {
            var ofs = _current;
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
            Tracer.Write($"per: {_current.ToString()} / {Capacity.ToString()}");
            Tracer.Write($"======================");
        }

        public static void Swap(ref umnStack lhs, ref umnStack rhs)
        {
            var tempRootChk = lhs._rootChk;
            var tempRootPtr = lhs._rootPtr;
            var tempHead = lhs._current;
            lhs._rootChk = rhs._rootChk;
            lhs._rootPtr = rhs._rootPtr;
            lhs._current = rhs._current;
            rhs._rootChk = tempRootChk;
            rhs._rootPtr = tempRootPtr;
            rhs._current = tempHead;
        }
    }
}
