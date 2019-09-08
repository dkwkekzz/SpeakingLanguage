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
            private int capacity;
            private int offset;

            public Library.umnChunk* Current => cur;

            int IEnumerator<int>.Current => cur->typeHandle;
            object IEnumerator.Current => cur->typeHandle;

            public Enumerator(ref slObject obj)
            {
                fixed (slObject* pObj = &obj)
                {
                    begin = (Library.umnChunk*)((IntPtr)pObj + sizeof(slObject));
                    cur = null;
                    capacity = pObj->Capacity;
                    offset = 0;
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
                {
                    int interval = Library.umnSize.umnChunk + cur->length;
                    offset += interval;
                    if (offset >= capacity)
                        return false;

                    cur = Library.umnChunk.GetNext(cur);
                }
                
                return cur != null && cur->length > 0;
            }

            public void Reset()
            {
                cur = null;
            }
        }
        
        public slObjectHandle handle;
        public int Capacity { get; private set; }

        public static slObject* CreateDefault<TAllocator>(ref TAllocator allocator, int handle)
            where TAllocator : unmanaged, Library.IumnAllocator
        {
            var szObj = sizeof(slObject);
            var objChk = allocator.Alloc(szObj);
            if (null == objChk) return null;

            var objPtr = Library.umnChunk.GetPtr<slObject>(objChk);
            objPtr->handle = handle;
            
            var szDefaultState = TypeManager.Instance.SHDefaultState.size;
            var chkDefaultState = allocator.Calloc(szDefaultState);
            chkDefaultState->typeHandle = TypeManager.Instance.SHDefaultState.key;
            chkDefaultState->length = szDefaultState;
            
            objPtr->Capacity = sizeof(Library.umnChunk) + szDefaultState;

            return objPtr;
        }

        public static slObject* CreateControl<TAllocator>(ref TAllocator allocator, int handle)
            where TAllocator : unmanaged, Library.IumnAllocator
        {
            var szObj = sizeof(slObject);
            var objChk = allocator.Alloc(szObj);
            if (null == objChk) return null;

            var objPtr = Library.umnChunk.GetPtr<slObject>(objChk);
            objPtr->handle = handle;

            var szDefaultState = TypeManager.Instance.SHDefaultState.size;
            var chkDefaultState = allocator.Calloc(szDefaultState);
            chkDefaultState->typeHandle = TypeManager.Instance.SHDefaultState.key;
            chkDefaultState->length = szDefaultState;

            var szControlState = TypeManager.Instance.SHControlState.size;
            var chkControlState = allocator.Calloc(szControlState);
            chkControlState->typeHandle = TypeManager.Instance.SHControlState.key;
            chkControlState->length = szControlState;

            objPtr->Capacity = sizeof(Library.umnChunk) + szDefaultState + sizeof(Library.umnChunk) + szControlState;

            return objPtr;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(ref this);
        }

        public void Dispose()
        {
            handle = -1;
        }

        public void Append(int offset)
        {
            Capacity += offset;
        }

        public void Append(Library.umnChunk* chk)
        {
            Capacity += sizeof(Library.umnChunk) + chk->length;
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
