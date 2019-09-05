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
        public int FrameCount { get; private set; }

        public Service(StartInfo info) : this(ref info)
        {
        }

        public Service(ref StartInfo info)
        {
            colAct = new slActionCollection();
            colObj = new slObjectCollection(info.default_objectcount);
            itrGraph = new Container.InteractionGraph(info.default_objectcount, info.default_interactcount);

            BeginTick = Library.Ticker.GlobalTicks;
            Delta = 0;
            FrameRate = info.default_frameRate;
            FrameTick = 1000 * 10000 / FrameRate;
            FrameCount = 0;
        }

        public void Dispose()
        {
            colObj.Dispose();
            itrGraph.Dispose();
        }

        public unsafe void DeserializeObject(ref Library.Reader reader)
        {
            colObj.InsertFront(ref reader);
        }
        
        public unsafe void SerializeObject(slObjectHandle handle, ref Library.Writer writer)
        {
            var obj = colObj.Find(handle);
            if (obj == null)
            {
                Library.Tracer.Error($"No has object: {handle.ToString()}");
                return;
            }

            var size = obj->capacity + sizeof(slObject);
            writer.WriteInt(size);
            writer.WriteMemory(obj, size);
        }

        public void Begin()
        {
            Delta = (int)(CurrentTick - BeginTick);
            BeginTick = CurrentTick;
        }

        public FrameResult End()
        {
            //colObj.SwapBuffer();
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
