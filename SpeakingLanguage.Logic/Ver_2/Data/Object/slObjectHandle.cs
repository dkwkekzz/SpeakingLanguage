﻿using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct slObjectHandle : IEquatable<slObjectHandle>
    {
        public int value;

        public override string ToString() => value.ToString();

        public slObjectHandle(int v)
        {
            value = v;
        }

        public bool Equals(slObjectHandle other)
        {
            return value == other.value;
        }

        public static implicit operator slObjectHandle(int h) => new slObjectHandle { value = h };
        public static bool operator ==(slObjectHandle key, int h) => key.value == h;
        public static bool operator !=(slObjectHandle key, int h) => key.value != h;
        public static bool operator ==(slObjectHandle lhs, slObjectHandle rhs) => lhs.value == rhs.value;
        public static bool operator !=(slObjectHandle lhs, slObjectHandle rhs) => lhs.value != rhs.value;
    }
}
