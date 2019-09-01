using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe interface IumnAllocator
    {
        umnChunk* Alloc(int size);
        umnChunk* Calloc(int size);
    }
}
