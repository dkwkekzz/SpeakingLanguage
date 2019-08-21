using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct slObject2 : IDisposable
    {
        public struct Enumerator : IEnumerator<int>
        {
            private Library.umnChunk* root;
            private Library.umnChunk* cur;

            public Library.umnChunk* Current => cur;

            int IEnumerator<int>.Current => cur->typeHandle;
            object IEnumerator.Current => cur->typeHandle;

            public Enumerator(Library.umnChunk* chk)
            {
                root = chk;
                cur = null;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (null == cur)
                    cur = root;
                else
                    cur = cur->next;
                return cur != null;
            }

            public void Reset()
            {
                cur = null;
            }
        }

        private Library.umnChunk* _root;
        private Library.umnChunk* _head;

        public slObjectHandle handle;
        
        public static slObject2* CreateNew<TAllocator>(ref TAllocator allocator, int handle)
            where TAllocator : unmanaged, Library.IumnAllocator
        {
            var szObj = sizeof(slObject2);
            var objChk = allocator.Alloc(szObj);
            var objPtr = Library.umnChunk.GetPtr<slObject2>(objChk);
            objPtr->_root = objChk;
            objPtr->_head = objChk;
            objPtr->handle = handle;

            return objPtr;
        }
        
        public void Reset(int handle)
        {
            this.handle = handle;
        }

        public void Release()
        {
            handle = 0;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_head);
        }
        
        public void Link(Library.umnChunk* chk)
        {
            _head->next = chk;
            _head = chk;
        }

        public TState* AddState<TState>() where TState : unmanaged
        {
            int sz = sizeof(TState);
            var type = typeof(TState);
            var ptr = _addImpl(sz, type);
            return (TState*)ptr;
        }

        public TState* AddState<TState>(TState e) where TState : unmanaged
        {
            int sz = sizeof(TState);
            var type = typeof(TState);
            var ptr = _addImpl(sz, type);
            Buffer.MemoryCopy(&e, ptr.ToPointer(), sz, sz);
            return (TState*)ptr;
        }

        private IntPtr _addImpl(int sz, Type type)
        {
            var chk = _block.Calloc(sz);
            if (null == chk)
                Library.ThrowHelper.ThrowOutOfMemory($"in object inner heap\ncapacity:{_block.Capacity.ToString()}");

            var typeHandle = StateCollection.GetStateHandle(type).key;
            chk->typeHandle = typeHandle;

            if (_root == null)
            {
                _root = chk;
                return Library.umnChunk.GetPtr(chk);
            }

            Library.umnChunk* prev = null;
            Library.umnChunk* cur = _root;
            while (null != cur)
            {
                if (cur->typeHandle > typeHandle)
                    break;

                prev = cur;
                cur = cur->next;
            }

            if (null != prev)
                prev->next = chk;
            chk->next = cur;

            return Library.umnChunk.GetPtr(chk);
        }

        public bool RemoveEntity<TState>()
        {
            var typeHandle = StateCollection.GetStateHandle(typeof(TState)).key;
            var chk = _getStateChunk(typeHandle, _root);
            if (chk == null)
                return false;

            chk->Disposed = true;
            return true;
        }

        public void SetState<TState>(TState* src, TState value) where TState : unmanaged
        {
            var sz = sizeof(TState);
            Buffer.MemoryCopy(&value, src, sz, sz);
        }

        public TState* GetState<TState>() where TState : unmanaged
        {
            var typeHandle = StateCollection.GetStateHandle(typeof(TState)).key;
            var chk = _getStateChunk(typeHandle, _root);
            if (chk == null)
                return null;

            return (TState*)chk->Ptr;
        }

        public TState* GetState<TState>(Library.umnChunk* startChk) where TState : unmanaged
        {
            var typeHandle = StateCollection.GetStateHandle(typeof(TState)).key;
            var chk = _getStateChunk(typeHandle, startChk);
            if (chk == null)
                return null;

            return (TState*)chk->Ptr;
        }

        private Library.umnChunk* _getStateChunk(int typeHandle, Library.umnChunk* head)
        {
            while (null != head)
            {
                if (head->typeHandle == typeHandle)
                    return head;

                head = head->next;
            }

            return null;
        }

        public void Dispose()
        {
            _block.Dispose();
        }
    }

    public unsafe struct slObject : IDisposable
    {
        public struct Enumerator : IEnumerator<int>
        {
            private Library.umnChunk* root;
            private Library.umnChunk* cur;

            public Library.umnChunk* Current => cur;

            int IEnumerator<int>.Current => cur->typeHandle;
            object IEnumerator.Current => cur->typeHandle;

            public Enumerator(Library.umnChunk* chk)
            {
                root = chk;
                cur = null;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (null == cur)
                    cur = root;
                else
                    cur = cur->next;
                return cur != null;
            }

            public void Reset()
            {
                cur = null;
            }
        }

        private Library.umnHeap _block;
        private Library.umnChunk* _root;
        
        public slObjectHandle handle;
        public int BlockSize => _block.Capacity;

        public static unsafe slObject* Create<TAllocator>(TAllocator* allocator, int handle, int szBlock) 
            where TAllocator : unmanaged, Library.IumnAllocator
        {
            var objChk = allocator->Alloc(sizeof(slObject));
            var objPtr = (slObject*)Library.umnChunk.GetPtr(objChk);
            objPtr->_block = new Library.umnHeap(allocator->Alloc(szBlock));
            objPtr->_root = null;
            objPtr->handle = handle;
            return objPtr;
        }
        
        public void Allocate(int handle, Library.umnChunk* chk)
        {
            _block = new Library.umnHeap(chk);
            _root = null;
            this.handle = handle;
        }

        public void Take(int handle)
        {
            _block.Reset();
            _root = null;
            this.handle = handle;
        }

        public void Release()
        {
            handle = 0;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_root);
        }
        
        public TState* AddState<TState>() where TState : unmanaged
        {
            int sz = sizeof(TState);
            var type = typeof(TState);
            var ptr = _addImpl(sz, type);
            return (TState*)ptr;
        }

        public TState* AddState<TState>(TState e) where TState : unmanaged
        {
            int sz = sizeof(TState);
            var type = typeof(TState);
            var ptr = _addImpl(sz, type);
            Buffer.MemoryCopy(&e, ptr.ToPointer(), sz, sz);
            return (TState*)ptr;
        }

        private IntPtr _addImpl(int sz, Type type)
        {
            var chk = _block.Calloc(sz);
            if (null == chk)
                Library.ThrowHelper.ThrowOutOfMemory($"in object inner heap\ncapacity:{_block.Capacity.ToString()}");

            var typeHandle = StateCollection.GetStateHandle(type).key;
            chk->typeHandle = typeHandle;
            
            if (_root == null)
            {
                _root = chk;
                return Library.umnChunk.GetPtr(chk);
            }
            
            Library.umnChunk* prev = null;
            Library.umnChunk* cur = _root;
            while (null != cur)
            {
                if (cur->typeHandle > typeHandle)
                    break;

                prev = cur;
                cur = cur->next;
            }

            if (null != prev)
                prev->next = chk;
            chk->next = cur;

            return Library.umnChunk.GetPtr(chk);
        }

        public bool RemoveEntity<TState>()
        {
            var typeHandle = StateCollection.GetStateHandle(typeof(TState)).key;
            var chk = _getStateChunk(typeHandle, _root);
            if (chk == null)
                return false;
            
            chk->Disposed = true;
            return true;
        }

        public void SetState<TState>(TState* src, TState value) where TState : unmanaged
        {
            var sz = sizeof(TState);
            Buffer.MemoryCopy(&value, src, sz, sz);
        }

        public TState* GetState<TState>() where TState : unmanaged
        {
            var typeHandle = StateCollection.GetStateHandle(typeof(TState)).key;
            var chk = _getStateChunk(typeHandle, _root);
            if (chk == null)
                return null;

            return (TState*)chk->Ptr;
        }

        public TState* GetState<TState>(Library.umnChunk* startChk) where TState : unmanaged
        {
            var typeHandle = StateCollection.GetStateHandle(typeof(TState)).key;
            var chk = _getStateChunk(typeHandle, startChk);
            if (chk == null)
                return null;

            return (TState*)chk->Ptr;
        }

        private Library.umnChunk* _getStateChunk(int typeHandle, Library.umnChunk* head) 
        {
            while (null != head)
            {
                if (head->typeHandle == typeHandle)
                    return head;

                head = head->next;
            }
            
            return null;
        }

        public void Dispose()
        {
            _block.Dispose();
        }
    }
}
