using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal class FrameManager
    {
        private readonly int _startFrame;
        private int _elapsedFrame;

        public int FrameCount => _startFrame + _elapsedFrame;
        public long CurrentTick => Library.Ticker.GlobalTicks;

        public int Delta { get; private set; }
        public long FrameTick { get; private set; }

        public FrameManager(int startFrame)
        {
            _startFrame = startFrame;
            _elapsedFrame = 0;
        }

        public void Begin()
        {
            _elapsedFrame++;

            Delta = (int)(CurrentTick - FrameTick);
            FrameTick = CurrentTick;
        }
    }
}
