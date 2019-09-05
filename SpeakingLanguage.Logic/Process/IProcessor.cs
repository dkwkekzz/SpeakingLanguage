using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public interface IProcessor : IDisposable
    {
        void Awake();
        void Signal(ref Service service);
    }
}
