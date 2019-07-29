using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Logic
{
    public class BinaryCommand
    {
        private readonly UnmanagedHeap _heap = new UnmanagedHeap(Config.MAX_BYTE_COMMAND_HEAP);
        private int _ofs;
        private int _sz;

        public Type Type { get; private set; }
        public bool IsEmtpy => _heap.IsEmpty;

        public void Take(Type type)
        {
            Type = type;
            _sz = Marshal.SizeOf(Type);
            _heap.Reset();
        }

        public void BeginRead()
        {
            _ofs = 0;
        }

        public unsafe void Write(void* p)
        {
            var ptr = _heap.Alloc(_sz);
            Buffer.MemoryCopy(&p, ptr.ToPointer(), _sz, _sz);
        }

        public unsafe bool Read(void* p)
        {
            if ((int)_heap.Offset == _ofs)
                return false;

            var ptr = _heap.Root;
            _ofs += _sz;
            ptr += _ofs;
            Buffer.MemoryCopy(ptr.ToPointer(), p, _sz, _sz);
            return true;
        }

        public unsafe void ReadAll(void* p)
        {
            _heap.CopyTo(p);
        }

        public unsafe void* GetBuffer(out long sz)
        {
            sz = _heap.Offset;
            return _heap.Root.ToPointer();
        }
    }
}
