using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnMarshal : IumnAllocator, IDisposable
    {
        private static readonly int SIZE_CHK = sizeof(umnChunk);

        private umnChunk* _rootChk;
        private umnChunk* _headChk;

        public umnChunk* Alloc(int size)
        {
            var ptr = Marshal.AllocHGlobal(size + SIZE_CHK);

            var chk = (umnChunk*)ptr;
            chk->ptr = ptr + SIZE_CHK;
            chk->next = null;
            chk->prev = null;
            chk->length = size;
            chk->dispose = false;

            if (_rootChk == null)
                _rootChk = chk;
            _headChk = chk;

            return chk;
        }

        public umnChunk* Calloc(int size)
        {
            var chk = Alloc(size);
            if (null == chk)
                return null;

            UnmanagedHelper.Memset(chk->ptr.ToPointer(), 0, size);
            return chk;
        }

        public void Dispose()
        {
            while (_rootChk != null)
            {
                Marshal.FreeHGlobal(_rootChk->ptr);
                _rootChk = _rootChk->prev;
            }
        }
    }
}
