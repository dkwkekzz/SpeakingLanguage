using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal unsafe struct slObject
    {
        public struct Enumerator : IEnumerator<int>
        {
            private Library.umnChunk* begin;
            private Library.umnChunk* cur;

            public Library.umnChunk* Current => cur;

            int IEnumerator<int>.Current => cur->typeHandle;
            object IEnumerator.Current => cur->typeHandle;

            public Enumerator(ref slObject obj)
            {
                fixed (slObject* pObj = &obj)
                {
                    begin = (Library.umnChunk*)((IntPtr)pObj + sizeof(slObject));
                    cur = null;
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (null == cur)
                    cur = begin;
                else
                    cur = Library.umnChunk.GetNext(cur);
                return cur != null;
            }

            public void Reset()
            {
                cur = null;
            }
        }
        
        public slObjectHandle handle;
        public int capacity;
        public int frame;
        
        public static slObject* GetNext(slObject* curObj)
        {
            var nextChkPtr = (IntPtr)curObj + sizeof(slObject) + curObj->capacity;
            if (((Library.umnChunk*)nextChkPtr)->length == 0)
                return null;

            var nextObjPtr = nextChkPtr + sizeof(Library.umnChunk);
            return (slObject*)nextObjPtr;
        }

        public static slObject* CreateNew<TAllocator>(ref TAllocator allocator, int handle)
            where TAllocator : unmanaged, Library.IumnAllocator
        {
            var szObj = sizeof(slObject);
            var objChk = allocator.Alloc(szObj);
            var objPtr = Library.umnChunk.GetPtr<slObject>(objChk);
            objPtr->handle = handle;
            
            var szLifeCycle = TypeManager.SHLifeCycle.size;
            var chkLifeCycle = allocator.Alloc(szLifeCycle);
            chkLifeCycle->typeHandle = TypeManager.SHLifeCycle.key;
            chkLifeCycle->length = szLifeCycle;

            var szSpawner = TypeManager.SHSpawner.size;
            var chkSpawner = allocator.Alloc(szSpawner);
            chkSpawner->typeHandle = TypeManager.SHSpawner.key;
            chkSpawner->length = szSpawner;

            var szPosition = TypeManager.SHPosition.size;
            var chkPosition = allocator.Alloc(szPosition);
            chkPosition->typeHandle = TypeManager.SHPosition.key;
            chkPosition->length = szPosition;

            objPtr->capacity = sizeof(Library.umnChunk) * 3 + szLifeCycle + szSpawner + szPosition;

            return objPtr;
        }

        public static LifeCycle* GetLifeCycle(slObject* obj)
        {
            var ptr = (IntPtr)obj + sizeof(slObject) + sizeof(Library.umnChunk);
            return (LifeCycle*)ptr.ToPointer();
        }

        public static Spawner* GetSpawner(slObject* obj)
        {
            var ptr = (IntPtr)obj + sizeof(slObject) + sizeof(Library.umnChunk) * 2 + TypeManager.SHLifeCycle.size;
            return (Spawner*)ptr.ToPointer();
        }

        public static Position* GetPosition(slObject* obj)
        {
            var ptr = (IntPtr)obj + sizeof(slObject) + sizeof(Library.umnChunk) * 3 
                + TypeManager.SHLifeCycle.size + TypeManager.SHSpawner.size;
            return (Position*)ptr.ToPointer();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(ref this);
        }
        
        public void Append(Library.umnChunk* chk)
        {
            capacity += sizeof(Library.umnChunk) + chk->length;
        }
    }

    #region OLD
    //public unsafe struct slObject : IDisposable
    //{
    //    public struct Enumerator : IEnumerator<int>
    //    {
    //        private Library.umnChunk* root;
    //        private Library.umnChunk* cur;
    //
    //        public Library.umnChunk* Current => cur;
    //
    //        int IEnumerator<int>.Current => cur->typeHandle;
    //        object IEnumerator.Current => cur->typeHandle;
    //
    //        public Enumerator(Library.umnChunk* chk)
    //        {
    //            root = chk;
    //            cur = null;
    //        }
    //
    //        public void Dispose()
    //        {
    //        }
    //
    //        public bool MoveNext()
    //        {
    //            if (null == cur)
    //                cur = root;
    //            else
    //                cur = cur->next;
    //            return cur != null;
    //        }
    //
    //        public void Reset()
    //        {
    //            cur = null;
    //        }
    //    }
    //
    //    private Library.umnHeap _block;
    //    private Library.umnChunk* _root;
    //    
    //    public slObjectHandle handle;
    //    public int BlockSize => _block.Capacity;
    //
    //    public static unsafe slObject* Create<TAllocator>(TAllocator* allocator, int handle, int szBlock) 
    //        where TAllocator : unmanaged, Library.IumnAllocator
    //    {
    //        var objChk = allocator->Alloc(sizeof(slObject));
    //        var objPtr = (slObject*)Library.umnChunk.GetPtr(objChk);
    //        objPtr->_block = new Library.umnHeap(allocator->Alloc(szBlock));
    //        objPtr->_root = null;
    //        objPtr->handle = handle;
    //        return objPtr;
    //    }
    //    
    //    public void Allocate(int handle, Library.umnChunk* chk)
    //    {
    //        _block = new Library.umnHeap(chk);
    //        _root = null;
    //        this.handle = handle;
    //    }
    //
    //    public void Take(int handle)
    //    {
    //        _block.Reset();
    //        _root = null;
    //        this.handle = handle;
    //    }
    //
    //    public void Release()
    //    {
    //        handle = 0;
    //    }
    //
    //    public Enumerator GetEnumerator()
    //    {
    //        return new Enumerator(_root);
    //    }
    //    
    //    public TState* AddState<TState>() where TState : unmanaged
    //    {
    //        int sz = sizeof(TState);
    //        var type = typeof(TState);
    //        var ptr = _addImpl(sz, type);
    //        return (TState*)ptr;
    //    }
    //
    //    public TState* AddState<TState>(TState e) where TState : unmanaged
    //    {
    //        int sz = sizeof(TState);
    //        var type = typeof(TState);
    //        var ptr = _addImpl(sz, type);
    //        Buffer.MemoryCopy(&e, ptr.ToPointer(), sz, sz);
    //        return (TState*)ptr;
    //    }
    //
    //    private IntPtr _addImpl(int sz, Type type)
    //    {
    //        var chk = _block.Calloc(sz);
    //        if (null == chk)
    //            Library.ThrowHelper.ThrowOutOfMemory($"in object inner heap\ncapacity:{_block.Capacity.ToString()}");
    //
    //        var typeHandle = StateCollection.GetStateHandle(type).key;
    //        chk->typeHandle = typeHandle;
    //        
    //        if (_root == null)
    //        {
    //            _root = chk;
    //            return Library.umnChunk.GetPtr(chk);
    //        }
    //        
    //        Library.umnChunk* prev = null;
    //        Library.umnChunk* cur = _root;
    //        while (null != cur)
    //        {
    //            if (cur->typeHandle > typeHandle)
    //                break;
    //
    //            prev = cur;
    //            cur = cur->next;
    //        }
    //
    //        if (null != prev)
    //            prev->next = chk;
    //        chk->next = cur;
    //
    //        return Library.umnChunk.GetPtr(chk);
    //    }
    //
    //    public bool RemoveEntity<TState>()
    //    {
    //        var typeHandle = StateCollection.GetStateHandle(typeof(TState)).key;
    //        var chk = _getStateChunk(typeHandle, _root);
    //        if (chk == null)
    //            return false;
    //        
    //        chk->Disposed = true;
    //        return true;
    //    }
    //
    //    public void SetState<TState>(TState* src, TState value) where TState : unmanaged
    //    {
    //        var sz = sizeof(TState);
    //        Buffer.MemoryCopy(&value, src, sz, sz);
    //    }
    //
    //    public TState* GetState<TState>() where TState : unmanaged
    //    {
    //        var typeHandle = StateCollection.GetStateHandle(typeof(TState)).key;
    //        var chk = _getStateChunk(typeHandle, _root);
    //        if (chk == null)
    //            return null;
    //
    //        return (TState*)chk->Ptr;
    //    }
    //
    //    public TState* GetState<TState>(Library.umnChunk* startChk) where TState : unmanaged
    //    {
    //        var typeHandle = StateCollection.GetStateHandle(typeof(TState)).key;
    //        var chk = _getStateChunk(typeHandle, startChk);
    //        if (chk == null)
    //            return null;
    //
    //        return (TState*)chk->Ptr;
    //    }
    //
    //    private Library.umnChunk* _getStateChunk(int typeHandle, Library.umnChunk* head) 
    //    {
    //        while (null != head)
    //        {
    //            if (head->typeHandle == typeHandle)
    //                return head;
    //
    //            head = head->next;
    //        }
    //        
    //        return null;
    //    }
    //
    //    public void Dispose()
    //    {
    //        _block.Dispose();
    //    }
    //}
    #endregion
}
