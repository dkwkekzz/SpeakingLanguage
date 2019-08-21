using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct slObjectEqualityComparer : Library.IumnEqualityComparer<slObjectHandle>
    {
        public bool Equals(slObjectHandle* x, slObjectHandle* y)
        {
            return x->handle == y->handle;
        }

        public int GetHashCode(slObjectHandle* obj)
        {
            return obj->handle;
        }
    }
}
