﻿using System;
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

            var curPtr = _rootPtr + _current;
            var endChk = (umnChunk*)(curPtr + totalSize);
            endChk->typeHandle = 0;
            endChk->length = 0;
            
            var chk = (umnChunk*)(curPtr);
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
        
        public void* Push(umnChunk* chk, int size)
        {
            var remained = _rootChk->length - _current;
            if (remained < size) return null;

            var curPtr = _rootPtr + _current;
            var endChk = (umnChunk*)(curPtr + size);
            endChk->typeHandle = 0;
            endChk->length = 0;

            var dest = (umnChunk*)curPtr;
            Buffer.MemoryCopy(chk, dest, size, size);

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
