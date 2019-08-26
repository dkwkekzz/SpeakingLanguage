using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal unsafe struct slSceneEqualityComparer : Library.IumnEqualityComparer<slSceneHandle>
    {
        public bool Equals(slSceneHandle* x, slSceneHandle* y)
        {
            return x->w == y->w
                && x->x == y->x
                && x->y == y->y
                && x->z == y->z;
        }

        public int GetHashCode(slSceneHandle* obj)
        {
            return obj->x ^ (obj->y << 16);
        }
    }
}
