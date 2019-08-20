using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnMarshal : IumnAllocator, IDisposable
    {
        private IntPtr _root;

        public umnChunk* Alloc(int size)
        {
            _root = Marshal.AllocHGlobal(size + umnSize.umnChunk * 2);

            var chk = (umnChunk*)_root;
            chk->next = (umnChunk*)(_root + umnSize.umnChunk + size);

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
            Marshal.FreeHGlobal(_root);
        }
    }
}
