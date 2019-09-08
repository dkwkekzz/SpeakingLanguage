using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct FrameResult
    {
        private static long _lastMS;

        public int frame;
        public int objectCount;
        public int frameTick;
        public int elapsed;
        public int delta;
        public int Leg => (elapsed - frameTick) / 10000;

        public override string ToString()
        {
            return $"[FrameResult] frameCount: {frame.ToString()}, objectCount: {objectCount.ToString()}, elapsed: {elapsed.ToString()}, leg: {Leg.ToString()}";
        }

        public void Display(int perMS = 1000)
        {
            var ms = Library.Ticker.ElapsedMS;
            if (ms - _lastMS > perMS)
            {
                Library.Tracer.Write(ToString());
                _lastMS = ms;
            } 
        }
    }
}
