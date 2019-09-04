using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Container
{
    internal enum InteractSort
    {
        Subject,
        Target,
    }
    
    internal struct InteractPair : IEquatable<InteractPair>, IEquatable<slObjectHandle>
    {
        public slObjectHandle handle;
        public int value;
        
        public InteractSort Sort => value < 1000 ? InteractSort.Subject : InteractSort.Target;
        public InteractDirection Direction => value < 1000 ? InteractDirection.None : (InteractDirection)value;

        public InteractPair(slObjectHandle h, int v)
        {
            handle = h;
            value = v;
        }

        public InteractPair(int h, int v)
        {
            handle = h;
            value = v;
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
