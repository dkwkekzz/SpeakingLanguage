using System;
using System.Collections.Generic;
using System.Threading;

namespace SpeakingLanguage.Library
{
    public struct SimpleSpinLock
    {
        private int _resourceInUse;

        public void Enter()
        {
            while (true)
            {
                if (Interlocked.Exchange(ref _resourceInUse, 1) == 0)
                    return;
            }
        }

        public void Leave()
        {
            Volatile.Write(ref _resourceInUse, 0);
        }
    }
}
