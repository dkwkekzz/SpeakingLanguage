using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct slObjectHandle
    {
        public int handle;

        public override string ToString() => handle.ToString();

        public static implicit operator slObjectHandle(int h) => new slObjectHandle { handle = h };
        public static bool operator ==(slObjectHandle key, int h) => key.handle == h;
        public static bool operator !=(slObjectHandle key, int h) => key.handle != h;
    }
}
