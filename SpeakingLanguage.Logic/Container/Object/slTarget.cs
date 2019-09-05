using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct slTarget : IDisposable
    {
        private readonly slObject* owner;
        private readonly Library.umnChunk** stateLookup;

        internal IntPtr ObjectPtr => (IntPtr)owner;
        internal int ObjectLength => owner->Capacity;

        public slObjectHandle Handle => owner->handle;

        internal slTarget(slObject* obj, Library.umnChunk** chks)
        {
            owner = obj;
            stateLookup = chks;
        }

        public TState Get<TState>() where TState : unmanaged
        {
            var handle = TypeManager.GetStateHandle(typeof(TState)).key;
            var chk = stateLookup[handle];
            if (null == chk)
                Library.ThrowHelper.ThrowKeyNotFound($"this state not found: {typeof(TState).Name}");

            var ptr = Library.umnChunk.GetPtr<TState>(chk);
            return *ptr;
        }

        public bool TryGet<TState>(out TState state) where TState : unmanaged
        {
            var handle = TypeManager.GetStateHandle(typeof(TState)).key;
            var chk = stateLookup[handle];
            if (null == chk)
            {
                state = default;
                return false;
            }

            var ptr = Library.umnChunk.GetPtr<TState>(chk);
            state = *ptr;
            return true;
        }

        public ref TState GetRef<TState>() where TState : unmanaged
        {
            var handle = TypeManager.GetStateHandle(typeof(TState)).key;
            var chk = stateLookup[handle];
            if (null == chk)
                Library.ThrowHelper.ThrowKeyNotFound($"this state not found: {typeof(TState).Name}");

            var ptr = Library.umnChunk.GetPtr<TState>(chk);
            return ref *ptr;
        }

        public TState* GetPtr<TState>() where TState : unmanaged
        {
            var handle = TypeManager.GetStateHandle(typeof(TState)).key;
            var chk = stateLookup[handle];
            if (null == chk) return null;

            var ptr = Library.umnChunk.GetPtr<TState>(chk);
            return ptr;
        }

        public void Set<TState>(TState st) where TState : unmanaged
        {
            var handle = TypeManager.GetStateHandle(typeof(TState)).key;
            var chk = stateLookup[handle];
            if (null == chk) Library.ThrowHelper.ThrowWrongArgument($"no has this state: {typeof(TState).Name}");

            var ptr = Library.umnChunk.GetPtr<TState>(chk);
            *ptr = st;
        }

        public void Dispose()
        {
        }
    }
}
