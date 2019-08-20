using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public class FrameAttribute : Attribute
    {
        public int Frame { get; }
        public float Per { get; }

        public FrameAttribute(int frame)
        {
            Frame = frame;
            Per = 1000f;
        }

        public FrameAttribute(int frame, float per)
        {
            Frame = frame;
            Per = per;
        }
    }
}
