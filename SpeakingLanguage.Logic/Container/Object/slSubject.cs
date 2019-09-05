using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct slSubject : IDisposable
    {
        public const int STACK_BUFFER_SIZE = 1024;

        private readonly slObject* owner;
        private readonly Library.umnChunk** stateLookup;
        private readonly IntPtr tempStackPtr;
        private int tempOffset;
        private bool isReadonly;

        internal IntPtr ObjectPtr => (IntPtr)owner;
        internal int ObjectLength => owner->Capacity;
        internal IntPtr StackPtr => tempStackPtr;
        internal int StackOffset => tempOffset;
        internal bool IsReadonly => isReadonly;

        public slObjectHandle Handle => owner->handle;
        
        internal slSubject(slObject* obj, Library.umnChunk** chks, byte* stackPtr)
        {
            owner = obj;
            stateLookup = chks;
            tempStackPtr = (IntPtr)stackPtr;
            tempOffset = 0;
            isReadonly = true;
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
        
        public TState* Add<TState>(TState st) where TState : unmanaged
        {
            var szObj = sizeof(TState);
            var szChk = sizeof(Library.umnChunk);
            if (tempOffset + szObj + szChk >= STACK_BUFFER_SIZE)
            {
                Library.Tracer.Error($"Overflow stackbuffer in subject add.");
                return null;
            }

            var handle = TypeManager.GetStateHandle(typeof(TState)).key;
            var objChk = new Library.umnChunk();
            objChk.typeHandle = handle;
            objChk.length = szObj;
        
            *((Library.umnChunk*)(tempStackPtr + tempOffset)) = objChk;
            tempOffset += sizeof(Library.umnChunk);
        
            var stPtr = (TState*)(tempStackPtr + tempOffset);
            *stPtr = st;
            tempOffset += szObj;

            isReadonly = false;

            return stPtr;
        }
        
        public bool Remove<TState>() where TState : unmanaged
        {
            var handle = TypeManager.GetStateHandle(typeof(TState)).key;
            var chk = stateLookup[handle];
            if (null == chk) return false;
        
            chk->Disposed = true;
            return true;
        }

        public void Dispose()
        {
        }
    }
}
