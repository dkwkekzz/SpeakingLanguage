using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Interact
{
    internal unsafe struct CallContext
    {
        public IPublicContext itrCtx;
        public void* src;
    }
}
