using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnHeap : IumnAllocator, IDisposable
    {
        private umnChunk* _rootChk;
        private umnChunk* _headChk;
        private IntPtr _head;

        public bool IsCreated => _rootChk != null;
        public int Capacity => _rootChk->length;
        public IntPtr Root => _rootChk->Ptr;
        public long Offset => _head.ToInt64() - _rootChk->Ptr.ToInt64();
        public IntPtr Tail => _rootChk->Ptr + _rootChk->length;
        public bool IsEmpty => _rootChk->Ptr == _head;

        public umnHeap(umnChunk* chk)
        {
            _rootChk = chk;
            _head = _rootChk->Ptr;
            _headChk = null;
        }

        public void Reset()
        {
            _head = _rootChk->Ptr;
            _headChk = null;
        }

        public umnChunk* Alloc(int size)
        {
            var remained = Capacity - Offset;
            if (remained < size)
            {
                remained = _compactClean();
                if (remained < size)
                    return null;
            }

            var chk = (umnChunk*)_head;
            _head += StructSize.umnChunk + size;

            chk->next = null;
            chk->prev = _headChk;
            _headChk = chk;

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

        public void CopyTo(void* dest)
        {
            var ofs = Offset;
            Buffer.MemoryCopy(Root.ToPointer(), dest, ofs, ofs);
        }

        public void CopyTo(IntPtr dest)
        {
            var ofs = Offset;
            Buffer.MemoryCopy(Root.ToPointer(), dest.ToPointer(), ofs, ofs);
        }
        
        public void Dispose()
        {
            while (_rootChk != null)
            {
                _rootChk->Disposed = true;
                _rootChk = _rootChk->prev;
            }
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
