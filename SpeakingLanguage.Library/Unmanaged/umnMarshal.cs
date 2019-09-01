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
            umnChunk* chk = null;
            if (null == _headChk)
            {
                var ptr = Marshal.AllocHGlobal(size + umnSize.umnChunk * 2);
                chk = (umnChunk*)ptr;
                chk->length = size;

                var endChk = umnChunk.GetNext(chk);
                endChk->length = 0;
            }
            else
            {
                var ptr = Marshal.AllocHGlobal(size + umnSize.umnChunk * 2);
                chk = (umnChunk*)ptr;
                chk->length = size;

                var offsetFromHead = (long)_headChk - (long)ptr;
                var endChk = umnChunk.GetNext(chk);
                endChk->length = (int)offsetFromHead;
            }

            return _headChk = chk;
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

        public void Dispose()
        {
            var chk = _headChk;
            while (null != chk)
            {
                var ptr = (IntPtr)chk;
                Marshal.FreeHGlobal(ptr);

                var endChk = umnChunk.GetNext(chk);
                if (endChk->length == 0)
                    break;

                chk = umnChunk.GetNext(endChk);
            }
        }
    }
}
