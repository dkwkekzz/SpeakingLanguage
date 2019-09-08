using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct slActionContext
    {
        public slSubject subject;
        public slTarget target;
        public int delta;
        public long beginTick;
        public long currentTick;
    }
}
