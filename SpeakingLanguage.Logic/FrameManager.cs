using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct FrameManager
    {
        private readonly int _startFrame;
        private int _elapsedFrame;

        public int FrameRate => 60;
        public int FrameCount => _startFrame + _elapsedFrame;
        public int Delta { get; private set; }

        // tick = ms / 10000
        public long CurrentTick => Library.Ticker.GlobalTicks;
        public long BeginTick { get; private set; }

        public FrameManager(int startFrame)
        {
            _startFrame = startFrame;
            _elapsedFrame = 0;

            Delta = 0;
            BeginTick = 0;
            BeginTick = CurrentTick;
        }

        public void Begin()
        {
            _elapsedFrame++;

            Delta = (int)(CurrentTick - BeginTick);
            BeginTick = CurrentTick;
        }

        public void End()
        {
            var frameTick = 1000 * 10000 / FrameRate;
            var leg = (int)(CurrentTick - BeginTick) - frameTick;
        }
    }
}
