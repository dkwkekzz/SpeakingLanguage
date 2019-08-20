using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct Streamer
    {
        private readonly Library.umnDynamicArray _stack;

        public Streamer(Library.umnChunk* chk)
        {
            _stack = new Library.umnDynamicArray(chk);
            _stack.Entry<Interaction>();
        }

        public void Insert(Interaction inter)
        {
            _stack.PushBack(&inter);
        }

        public bool TryGet(out Interaction inter)
        {
            var pe = _stack.PopBack<Interaction>();
            if (null == pe)
            {
                inter = default;
                return false;
            }

            inter = *pe;
            return true;
        }

        public void Reset()
        {
            _stack.Reset();
        }
    }
}
