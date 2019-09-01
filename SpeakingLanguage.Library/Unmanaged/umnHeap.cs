using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnHeap : IumnAllocator, IDisposable
    {
        private umnChunk* _rootChk;
        private umnChunk* _headChk;

        public int Capacity => _rootChk->length;
        public bool IsCreated => _rootChk != null;
        public umnChunk* Root => _rootChk;
        public long Offset => (long)_headChk - (long)_rootChk + umnSize.umnChunk;

        public umnHeap(umnChunk* chk)
        {
            _rootChk = chk;
            _headChk = _rootChk + umnSize.umnChunk;
            _headChk->length = 0;
        }

        public void Reset()
        {
            _headChk = _rootChk + umnSize.umnChunk;
        }

        public umnChunk* Alloc(int size)
        {
            var szChk = umnSize.umnChunk;
            var totalSize = size + szChk;
            var remained = Offset;
            if (remained < totalSize)
                return null;

            var headPtr = (IntPtr)_headChk;
            var endChk = (umnChunk*)(headPtr + totalSize);
            endChk->length = 0;

            _headChk->length = size;

            var curChk = _headChk;
            _headChk = endChk;
            return curChk;
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
            Tracer.Write($"=====[umnHeap]=====");
            Tracer.Write($"capacity: {Capacity.ToString()}");
            Tracer.Write($"per: {Offset.ToString()} / {Capacity.ToString()}");
            Tracer.Write($"======================");
        }
        
        private int _compactClean()
        {
            throw new NotImplementedException("implementing...");
            //if (null == _garbage || _garbage.Count == 0)
            //    return 0;
            //
            //_garbage.Sort((ck1, ck2) => { return ck1.ptr.ToInt64().CompareTo(ck2.ptr.ToInt64()); });
            //
            //IntPtr retHead = IntPtr.Zero;
            //IntPtr currentHead = _garbage[0].ptr;
            //IntPtr currentTail = _garbage[0].ptr + _garbage[0].size;
            //for (int i = 0; i != _garbage.Count - 1; i++)
            //{
            //    var current = _garbage[i];
            //    if (currentHead == IntPtr.Zero)
            //    {
            //        currentHead = current.ptr;
            //        currentTail = currentHead + current.size;
            //    }
            //
            //    if (currentTail == current.ptr)
            //    {
            //        currentTail = current.ptr + current.size;
            //        continue;
            //    }
            //
            //    var next = _garbage[i + 1];
            //    var interval = next.ptr.ToInt64() - currentTail.ToInt64();
            //
            //    Buffer.MemoryCopy(currentTail.ToPointer(), currentHead.ToPointer(), interval, interval);
            //
            //    retHead = currentHead + (int)interval;
            //    currentHead = IntPtr.Zero;
            //    currentTail = IntPtr.Zero;
            //}
            //
            //{
            //    var endPtr = _head;
            //    var interval = endPtr.ToInt64() - currentTail.ToInt64();
            //
            //    Buffer.MemoryCopy(currentTail.ToPointer(), currentHead.ToPointer(), interval, interval);
            //
            //    retHead = currentHead + (int)interval;
            //}
            //
            //_head = retHead;
            //_garbage.Clear();
            //
            //var remained = _tail.ToInt64() - _head.ToInt64();
            //return (int)remained;
        }
    }
}
