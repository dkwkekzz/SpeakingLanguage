using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal struct Service : IDisposable
    {
        internal slActionCollection colAct;
        internal slObjectCollection colObj;
        internal Data.InteractionGraph itrGraph;
        
        // tick = ms / 10000
        public long CurrentTick => Library.Ticker.GlobalTicks;
        public long ElapsedTick => CurrentTick - BeginTick;
        public long BeginTick { get; private set; }
        public int Delta { get; private set; }
        public int FrameRate { get; private set; }
        public int FrameTick { get; private set; }

        public Service(StartInfo info) : this(ref info)
        {
        }

        public Service(ref StartInfo info)
        {
            colAct = new slActionCollection();
            colObj = new slObjectCollection(info.default_objectcount);
            itrGraph = new Data.InteractionGraph(info.default_objectcount, info.default_interactcount);

            BeginTick = Library.Ticker.GlobalTicks;
            Delta = 0;
            FrameRate = info.default_frameRate;
            FrameTick = 1000 * 10000 / FrameRate;
        }

        public void Dispose()
        {
            colObj.Dispose();
            itrGraph.Dispose();
        }
        
        public void Begin()
        {
            Delta = (int)(CurrentTick - BeginTick);
            BeginTick = CurrentTick;
        }

        public FrameResult End(int currentFrame)
        {
            colObj.SwapBuffer();
            itrGraph.Reset();

            return new FrameResult
            {
                frame = currentFrame,
                objectCount = colObj.Count,
                frameTick = FrameTick,
                elapsed = (int)(CurrentTick - BeginTick),
                delta = Delta,
            };
        }
    }
}
