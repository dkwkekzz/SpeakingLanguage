using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct Service : IDisposable
    {
        private readonly Library.umnMarshal _umnAllocator;

        private readonly ActionCollection _colAct;
        private readonly ObjectCollection _colObj;
        private readonly StreamingContext _ctx;

        public Service(StartInfo info) : this(ref info)
        {
        }

        public Service(ref StartInfo info)
        {
            _umnAllocator = new Library.umnMarshal();
            _colAct = new ActionCollection();
            _colObj = new ObjectCollection(_umnAllocator.Alloc(info.max_byte_objectcollection));
            _ctx = new StreamingContext
            {
                frameManager = new FrameManager(info.startFrame),
                streamer = new Streamer(_umnAllocator.Calloc(info.max_byte_streamer)),
            };
        }

        public void Dispose()
        {
            _umnAllocator.Dispose();
        }

    }
}
