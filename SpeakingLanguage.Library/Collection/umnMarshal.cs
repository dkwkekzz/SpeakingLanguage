using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnMarshal : IumnAllocator, IDisposable
    {
        private umnChunk* _headChk;

        public umnChunk* Alloc(int size)
        {
            var ptr = Marshal.AllocHGlobal(size + StructSize.umnChunk);

            var chk = (umnChunk*)ptr;
            chk->length = size;
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

        public void Dispose()
        {
            while (_headChk != null)
            {
                Marshal.FreeHGlobal(_headChk->Ptr);
                _headChk = _headChk->prev;
            }
        }
    }
}
