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
                chk->next = (umnChunk*)(ptr + umnSize.umnChunk + size);
            }
            else
            {
                var ptr = Marshal.AllocHGlobal(size + umnSize.umnChunk);
                chk = (umnChunk*)ptr;
                chk->next = _headChk;
            }
            
            return _headChk = chk;
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
            var chk = _headChk;
            while (null != chk)
            {
                var ptr = (IntPtr)chk;
                Marshal.FreeHGlobal(ptr);

                chk = chk->next;
            }
        }
    }
}
