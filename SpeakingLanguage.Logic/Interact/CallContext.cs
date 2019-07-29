using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Interact
{
    internal unsafe struct CallContext
    {
        public float delta;
        public void* src;
        public object[] args;
    }
}
