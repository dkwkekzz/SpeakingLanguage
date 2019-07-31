using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal struct sObserverComparer : IComparer<sObserver.Key>
    {
        public int Compare(sObserver.Key x, sObserver.Key y)
        {
            return x.Handle.CompareTo(y.Handle);
        }
    }
}
