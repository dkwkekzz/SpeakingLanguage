using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct StateContext
    {
        private slObject2* owner;
        private Library.umnChunk** stateChks;

        public slObjectHandle Handle => owner->handle;

        public StateContext(slObject2* obj, Library.umnChunk** chks)
        {
            owner = obj;
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
