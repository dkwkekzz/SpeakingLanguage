using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct slActionContext
    {
        public slObjectContext subject;
        public slObjectContext target;
        public int delta;
        public long beginTick;
        public long currentTick;
    }
}
