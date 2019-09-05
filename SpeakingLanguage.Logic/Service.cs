using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct Service : IDisposable
    {
        internal readonly slActionCollection colAct;
        internal readonly slObjectCollection colObj;
        internal readonly Container.InteractionGraph itrGraph;

        // tick = ms / 10000
        public long CurrentTick => Library.Ticker.GlobalTicks;
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
            colObj = new slObjectCollection(Config.default_objectcount);
            itrGraph = new Container.InteractionGraph(Config.default_objectcount, Config.default_interactcount);

            BeginTick = Library.Ticker.GlobalTicks;
            Delta = 0;
            FrameRate = Config.default_frameRate;
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

        public FrameResult End()
        {
            colObj.SwapBuffer();
            itrGraph.Reset();

            return new FrameResult
            {
                objectCount = colObj.Count,
                frameTick = FrameTick,
                elapsed = (int)(CurrentTick - BeginTick),
                delta = Delta,
            };
        }
    }
}
