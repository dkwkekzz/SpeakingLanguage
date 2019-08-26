using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct slObjectContext : IDisposable
    {
        private readonly slObject* owner;
        private readonly Library.umnChunk** stateLookup;
        private readonly IntPtr tempStackPtr;
        private int tempOffset;

        internal IntPtr ObjectPtr => (IntPtr)owner;
        internal int ObjectLength => owner->capacity;
        internal IntPtr StackPtr => tempStackPtr;
        internal int StackOffset => tempOffset; 

        internal slObjectHandle Handle => owner->handle;

        internal slObjectContext(slObject* obj, Library.umnChunk** chks, byte* stackPtr)
        {
            owner = obj;
            stateLookup = chks;
            tempStackPtr = (IntPtr)stackPtr;
            tempOffset = 0;
        }

        public TState* Get<TState>() where TState : unmanaged
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
            var handle = TypeManager.GetStateHandle(typeof(TState)).key;
            var objChk = new Library.umnChunk();
            objChk.typeHandle = handle;
            objChk.length = szObj;

            *((Library.umnChunk*)(tempStackPtr + tempOffset)) = objChk;
            tempOffset += sizeof(Library.umnChunk);

            var stPtr = (TState*)(tempStackPtr + tempOffset);
            *stPtr = st;
            tempOffset += szObj;
            
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
