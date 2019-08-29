using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct Service : IDisposable
    {
        internal readonly slActionCollection colAct;
        internal readonly slObjectCollection colObj;

        // tick = ms / 10000
        internal long CurrentTick => Library.Ticker.GlobalTicks;
        internal long BeginTick { get; private set; }
        internal int Delta { get; private set; }
        internal int FrameRate { get; set; }

        public Service(StartInfo info) : this(ref info)
        {
        }

        public Service(ref StartInfo info)
        {
            colAct = new slActionCollection();
            colObj = new slObjectCollection(info.default_objectcount);

            BeginTick = Library.Ticker.GlobalTicks;
            Delta = 0;
            FrameRate = info.default_frameRate;
        }

        public void Dispose()
        {
            colObj.Dispose();
        }

        public unsafe void DeserializeObject(ref Library.Reader reader)
        {
            var ret = reader.ReadInt(out int size);
            if (!ret)
            {
                Library.Tracer.Error("fail to read.");
                return;
            }

            colObj.InsertFront(ref reader, size);
        }

        public unsafe void SerializeObject(slObjectHandle handle, ref Library.Writer writer)
        {
            var obj = colObj.Find(handle);
            if (obj == null)
            {
                Library.Tracer.Error($"no has object: {handle.ToString()}");
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
            colObj.SwapBuffer();

            var frameTick = 1000 * 10000 / FrameRate;
            var elapsed = (int)(CurrentTick - BeginTick);
            Library.Tracer.Write($"[Service Report] elapsed: {elapsed.ToString()}");

            return new FrameResult
            {
                leg = elapsed - frameTick,
            };
        }
    }
}
