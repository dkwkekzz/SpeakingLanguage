using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal unsafe struct slObjectEqualityComparer : Library.IumnEqualityComparer<slObjectHandle>
    {
        public bool Equals(slObjectHandle* x, slObjectHandle* y)
        {
            return x->value == y->value;
        }

        public int GetHashCode(slObjectHandle* obj)
        {
            return obj->value;
        }
    }
}
