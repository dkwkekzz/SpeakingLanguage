using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnChunk
    {
        public umnChunk* next;
        public int typeHandle;
        public int length;

        public static umnChunk* GetChunk(void* ptr) => (umnChunk*)((IntPtr)ptr - umnSize.umnChunk);
        public static umnChunk* GetNext(umnChunk* chk) => (umnChunk*)((IntPtr)chk + umnSize.umnChunk + chk->length);
        public static IntPtr GetPtr(umnChunk* chk) => (IntPtr)chk + umnSize.umnChunk;
        public static T* GetPtr<T>(umnChunk* chk) where T : unmanaged => (T*)((IntPtr)chk + umnSize.umnChunk);
        public static int GetLength(umnChunk* chk) => (int)((long)chk->next - (long)chk->next) - umnSize.umnChunk;

        public IntPtr Ptr
        {
            get
            {
                fixed (umnChunk* pth = &this)
                    return (IntPtr)pth + umnSize.umnChunk;
            }
        }

        public int Length
        {
            get
            {
                fixed (umnChunk* pth = &this)
                    return (int)(((IntPtr)next).ToInt64() - ((IntPtr)pth).ToInt64()) - umnSize.umnChunk;

            }
        }

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
            }
        }
    }
}
