using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct Streamer
    {
        public Library.umnDynamicArray _stack;

        public Streamer(Library.umnChunk* chk)
        {
            _stack = new Library.umnDynamicArray(chk);
        }
    }
}
