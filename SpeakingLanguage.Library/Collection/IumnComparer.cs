using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe interface IumnComparer<T> where T : unmanaged
    {
        int Compare(T* x, T* y);
    }
}
