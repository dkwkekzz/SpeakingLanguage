using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnChunk
    {
        private static readonly IntPtr _disposedPtr = typeof(umnChunk).TypeHandle.Value;

        public umnChunk* next;
        public umnChunk* prev;
        public IntPtr typeHandle;
        public int length;

        public IntPtr Ptr
        {
            get
            {
                fixed (umnChunk* pth = &this)
                    return (IntPtr)pth + sizeof(umnChunk);
            }
        }

        //public umnChunk* next => (umnChunk*)(ptr + length).ToPointer();

        public IntPtr NextPtr => Ptr + length + sizeof(umnChunk);

        public bool Disposed
        {
            get
            {
                if (typeHandle == _disposedPtr)
                    return true;
                return false;
            }
            set
            {
                if (value)
                    typeHandle = _disposedPtr;
            }
        }
    }
}
