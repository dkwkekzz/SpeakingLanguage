using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct FrameResult
    {
        public int objectCount;
        public int frameTick;
        public int elapsed;
        public int delta;
        public int Leg => (elapsed - frameTick) / 10000;

        public override string ToString()
        {
            return $"[FrameResult] objectCount: {objectCount.ToString()}, elapsed: {elapsed.ToString()}, leg: {Leg.ToString()}";
        }

        public void Display()
        {
            Library.Tracer.Write(ToString());
        }
    }
}
