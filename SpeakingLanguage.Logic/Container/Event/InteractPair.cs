using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Container
{
    internal struct InteractPair : IEquatable<InteractPair>, IEquatable<slObjectHandle>
    {
        public slObjectHandle handle;
        public int count;
        
        public InteractPair(slObjectHandle h, int v)
        {
            handle = h;
            count = v;
        }

        public InteractPair(int h, int v)
        {
            handle = h;
            count = v;
        }

        public bool Equals(InteractPair other)
        {
            return handle == other.handle;
        }

        public bool Equals(slObjectHandle other)
        {
            return handle == other.value;
        }
    }
}
