using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct InteractContext
    {
        public int delta;
        public long frameTick;
        public void* pSrc;
        public slObject* pThis;
        public slObject* pOther;
    }
}
