using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe interface IumnEqualityComparer<T> where T : unmanaged
    {
        bool Equals(T* x, T* y);
        int GetHashCode(T* obj);
    }
}
