using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal interface IProcessor : IDisposable
    {
        void Awake();
        void Signal(ref Service service);
    }
}
