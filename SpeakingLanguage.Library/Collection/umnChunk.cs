using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnChunk
    {
        public IntPtr ptr;
        public umnChunk* next;
        public umnChunk* prev;
        public int length;
        public IntPtr typeHandle;
        public bool dispose;
    }
}
