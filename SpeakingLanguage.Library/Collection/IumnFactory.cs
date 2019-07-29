using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public unsafe interface IumnFactory<T>
        where T : unmanaged
    {
        T* GetObject();
        void PutObject(T* x);
    }
}
