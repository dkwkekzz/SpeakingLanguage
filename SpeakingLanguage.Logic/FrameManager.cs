using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct FrameManager
    {
        private readonly int _startFrame;
        private int _elapsedFrame;

        public int FrameCount => _startFrame + _elapsedFrame;
        // tick = ms / 10000
        public long CurrentTick => Library.Ticker.GlobalTicks;

        public int Delta { get; private set; }
        // tick = ms / 10000
        public long FrameTick { get; private set; }

        public FrameManager(int startFrame)
        {
            _startFrame = startFrame;
            _elapsedFrame = 0;

            Delta = 0;
            FrameTick = 0L;
        }

        public void Begin()
        {
            _elapsedFrame++;

            Delta = (int)(CurrentTick - FrameTick);
            FrameTick = CurrentTick;
        }

        public void End()
        {
            var leg = (int)(CurrentTick - FrameTick) - 166666;
        }
    }
}
