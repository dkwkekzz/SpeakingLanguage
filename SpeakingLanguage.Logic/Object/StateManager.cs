using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct StateManager : IDisposable
    {
        private Library.umnStack* allocator;
        private slObject2* owner;
        private Library.umnChunk** stateLookup;

        public slObjectHandle Handle => owner->handle;

        public StateManager(Library.umnStack* alloc, slObject2* obj, Library.umnChunk** chks)
        {
            allocator = alloc;
            owner = obj;
            stateLookup = chks;
        }

        public TState* Get<TState>() where TState : unmanaged
        {
            var handle = StateCollection.GetStateHandle(typeof(TState)).key;
            var chk = stateLookup[handle];
            if (null == chk) return null;

            var ptr = Library.umnChunk.GetPtr<TState>(chk);
            return ptr;
        }

        public void Set<TState>(TState st) where TState : unmanaged
        {
            var handle = StateCollection.GetStateHandle(typeof(TState)).key;
            var chk = stateLookup[handle];
            if (null == chk) Library.ThrowHelper.ThrowWrongArgument($"no has this state: {typeof(TState).Name}");

            var ptr = Library.umnChunk.GetPtr<TState>(chk);
            *ptr = st;
        }

        public TState* Add<TState>() where TState : unmanaged
        {
            var szObj = sizeof(TState);
            var objChk = allocator->Calloc(szObj);
            var objPtr = Library.umnChunk.GetPtr<TState>(objChk);
            return objPtr;
        }

        public TState* Add<TState>(TState st) where TState : unmanaged
        {
            var szObj = sizeof(TState);
            var objChk = allocator->Calloc(szObj);
            var objPtr = Library.umnChunk.GetPtr<TState>(objChk);
            *objPtr = st;
            return objPtr;
        }

        public bool Remove<TState>() where TState : unmanaged
        {
            var handle = StateCollection.GetStateHandle(typeof(TState)).key;
            var chk = stateLookup[handle];
            if (null == chk) return false;

            chk->Disposed = true;
            return true;
        }

        public void Dispose()
        {
            var iter = owner->GetEnumerator();
            while (iter.MoveNext())
            {
                var chk = iter.Current;
                if (chk->Disposed) continue;

                var backChk = allocator->Alloc(Library.umnChunk.GetLength(chk));
                backChk->typeHandle = chk->typeHandle;
            }
        }
    }
}
