using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct ActionContext
    {
        public StateManager subject;
        public StateManager target;
        public int delta;
    }
}
