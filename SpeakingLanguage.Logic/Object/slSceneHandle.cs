using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct slSceneHandle : IEquatable<slSceneHandle>
    {
        public int w;
        public int x;
        public int y;
        public int z;

        public override string ToString() => $"({w.ToString()}, {x.ToString()}, {y.ToString()}, {z.ToString()})";

        public bool Equals(slSceneHandle other)
        {
            return w == other.w
                && x == other.x
                && y == other.y
                && z == other.z;
        }

        public static bool operator ==(slSceneHandle lhs, slSceneHandle rhs) => lhs.Equals(rhs);
        public static bool operator !=(slSceneHandle lhs, slSceneHandle rhs) => !lhs.Equals(rhs);
    }
}
