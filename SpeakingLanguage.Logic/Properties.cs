using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public interface IProperty
    {
    }
}

namespace SpeakingLanguage.Logic.Property
{
    public struct Selector : IProperty
    {
        public int handle;
    }

    public struct Position : IProperty
    {
        public int w;
        public int x;
        public int y;
        public int z;
    }
}