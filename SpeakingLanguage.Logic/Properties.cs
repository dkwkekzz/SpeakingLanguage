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
    public struct Viewier : IProperty
    {
        public int handle;
        public int w;
        public int x;
        public int y;
        public int z;
    }

    public struct Position : IProperty
    {
        public int w;
        public int x;
        public int y;
        public int z;
    }

    public struct Controller : IProperty
    {
        public int handle;
        public int left;
        public int right;
        public int up;
        public int down;
        public int fire_a;
        public int fire_b;
        public int space;
    }

}