using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal class FrameTicker
    {
        private readonly int _startFrame;
        private int _elapsedFrame;

        public int FrameCount => _startFrame + _elapsedFrame;
        public long CurrentTick => Library.Ticker.GlobalTicks;
        public long FrameTick { get; private set; }

        public FrameTicker(int startFrame)
        {
            _startFrame = startFrame;
            _elapsedFrame = 0;
        }

        public void Begin()
        {
            _elapsedFrame++;
            FrameTick = CurrentTick;
        }
    }
}
