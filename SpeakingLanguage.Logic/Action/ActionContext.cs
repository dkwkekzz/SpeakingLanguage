using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct ActionContext
    {
        public StateContext subject;
        public StateContext target;
        public int delta;
    }
}
