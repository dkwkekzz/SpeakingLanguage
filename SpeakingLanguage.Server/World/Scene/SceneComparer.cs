using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    public class SceneComparer : IEqualityComparer<SceneHandle>
    {
        public bool Equals(SceneHandle x, SceneHandle y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(SceneHandle obj)
        {
            return obj.xValue ^ (obj.yValue << 16);
        }
    }
}
