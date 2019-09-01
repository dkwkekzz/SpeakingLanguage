using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnChunk
    {
        public int typeHandle;
        public int length;

        public static umnChunk* GetChunk(void* ptr) => (umnChunk*)((IntPtr)ptr - umnSize.umnChunk);
        public static umnChunk* GetNext(umnChunk* chk) => (umnChunk*)((IntPtr)chk + umnSize.umnChunk + chk->length);
        public static IntPtr GetPtr(umnChunk* chk) => (IntPtr)chk + umnSize.umnChunk;
        public static T* GetPtr<T>(umnChunk* chk) where T : unmanaged => chk->length == 0 ? null : (T*)((IntPtr)chk + umnSize.umnChunk);
        
        public bool Disposed
        {
            get
            {
                return typeHandle == -1;
            }
            set
            {
                if (value)
                    typeHandle = -1;
                else if (typeHandle < 0)
                    typeHandle = 0;
            }
        }
    }
}
