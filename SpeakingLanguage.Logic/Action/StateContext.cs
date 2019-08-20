using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct StateContext
    {
        private slObjectHandle handle;
        private Library.umnChunk** stateChks;

        public slObjectHandle Handle => handle;

        public StateContext(slObjectHandle hd, Library.umnChunk** chks)
        {
            handle = hd;
            stateChks = chks;
        }

        public TState* Get<TState>() where TState : unmanaged
        {
            var handle = StateCollection.GetStateHandle(typeof(TState)).key;
            var ptr = Library.umnChunk.GetPtr<TState>(stateChks[handle]);
            return ptr;
        }
    }
}
