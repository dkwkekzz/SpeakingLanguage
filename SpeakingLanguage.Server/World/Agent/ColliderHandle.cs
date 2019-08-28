using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal struct ColliderHandle
    {
        public int objHandleValue;

        public ColliderHandle(Logic.slObjectHandle objectHandle)
        {
            objHandleValue = objectHandle.value;
        }
    }
}
