using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Event
{
    internal unsafe struct Transform
    {
        public slObject* actor;
        public int handle;
        public int x;
        public int y;
        public int layer;
    }
}
