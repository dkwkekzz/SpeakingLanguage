using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct ActionContext
    {
        public ObjectCollection2.StateManager subject;
        public ObjectCollection2.StateManager target;
        public int delta;
    }
}
