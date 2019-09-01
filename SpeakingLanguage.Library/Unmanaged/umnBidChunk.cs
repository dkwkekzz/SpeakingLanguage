using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnBidChunk
    {
        private static readonly IntPtr _disposedPtr = typeof(umnBidChunk).TypeHandle.Value;

        public umnBidChunk* next;
        public umnBidChunk* prev;
        public IntPtr typeHandle;
        public int length;

        public IntPtr Ptr
        {
            get
            {
                fixed (umnBidChunk* pth = &this)
                    return (IntPtr)pth + sizeof(umnBidChunk);
            }
        }
        
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
