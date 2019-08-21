using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct Service : IDisposable
    {
        public struct Poster<TEvent>
            where TEvent : unmanaged
        {
            private readonly Library.umnDynamicArray* pStreamer;

            public Poster(Library.umnDynamicArray* stream)
            {
                pStreamer = stream;
                pStreamer->Entry<TEvent>();
            }
            
            public void Push(TEvent st)
            {
                pStreamer->PushBack(&st);
            }

            public void Push(TEvent* st)
            {
                pStreamer->PushBack(st);
            }

            public void Push(void* ptr, int len)
            {
                pStreamer->PushBack(ptr, len);
            }
        }

        internal struct Requester<TEvent> : IDisposable 
            where TEvent : unmanaged
        {
            private readonly Library.umnDynamicArray* pStreamer;

            public Requester(Library.umnDynamicArray* stream)
            {
                pStreamer = stream;
            }

            public void Dispose()
            {
                pStreamer->Exit();
            }

            public bool TryPop(out TEvent stInter)
            {
                var pInter = pStreamer->PopBack<TEvent>();
                if (null == pInter)
                {
                    stInter = default;
                    return false;
                }

                stInter = *pInter;
                return true;
            }
        }
        
        private readonly Library.umnMarshal umnAllocator;
        private readonly Library.umnDynamicArray stEventStream;

        internal readonly ActionCollection colAct;
        internal readonly ObjectCollection2 colObj;
        internal readonly FrameManager frameManager;
        
        public Service(StartInfo info) : this(ref info)
        {
        }

        public Service(ref StartInfo info)
        {
            umnAllocator = new Library.umnMarshal();
            stEventStream = new Library.umnDynamicArray(umnAllocator.Calloc(info.max_byte_streamer));

            colAct = new ActionCollection();
            colObj = new ObjectCollection2(info.default_objectcount);
            frameManager = new FrameManager(info.startFrame);
        }

        public void Dispose()
        {
            umnAllocator.Dispose();
        }

        public Poster<TEvent> GetPoster<TEvent>()
            where TEvent : unmanaged
        {
            fixed (Library.umnDynamicArray* pStrm = &stEventStream)
                return new Poster<TEvent>(pStrm);
        }

        internal Requester<TEvent> GetRequester<TEvent>()
            where TEvent : unmanaged
        {
            fixed (Library.umnDynamicArray* pStrm = &stEventStream)
                return new Requester<TEvent>(pStrm);
        }
    }
}
