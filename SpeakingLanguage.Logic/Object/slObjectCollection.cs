using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal unsafe struct slObjectCollection : IEnumerable<slObject>, IDisposable
    {
        public struct Enumerator : IEnumerator<slObject>
        {
            private Library.umnChunk* root;
            private slObject* cur;

            public slObject* Current => cur;

            slObject IEnumerator<slObject>.Current => *Current;
            object IEnumerator.Current => *Current;

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
                {
                    cur = Library.umnChunk.GetPtr<slObject>(root);
                }
                else
                {
                    cur = slObject.GetNext(cur);
                }
                
                return null != cur;
            }

            public void Reset()
            {
                cur = null;
            }
        }
        
        private readonly Library.umnMarshal _umnAllocator;
        private Library.umnStack _frontStack;
        private Library.umnStack _backStack;
        private Library.umnHashMap<slObjectEqualityComparer, slObjectHandle, slObject> _lookup;

        public slObjectCollection(int defaultObjectCount)
        {
            _umnAllocator = new Library.umnMarshal();
            _frontStack = new Library.umnStack(_umnAllocator.Alloc(defaultObjectCount * 64));
            _backStack = new Library.umnStack(_umnAllocator.Alloc(defaultObjectCount * 64));
            _lookup = Library.umnHashMap<slObjectEqualityComparer, slObjectHandle, slObject>.CreateNew(ref _umnAllocator, defaultObjectCount);
        }

        private static int generator;
        public static int GenerateHandle
        {
            get { return generator++; }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_frontStack.Root);
        }

        IEnumerator<slObject> IEnumerable<slObject>.GetEnumerator()
        {
            return new Enumerator(_frontStack.Root);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(_frontStack.Root);
        }
        
        public void Dispose()
        {
            _umnAllocator.Dispose();
        }
        
        public void SwapBuffer()
        {
            Library.umnStack.Swap(ref _frontStack, ref _backStack);
        }

        public slObject* Find(slObjectHandle handle)
        {
            if (handle == 0)
                return null;

            return _lookup[handle];
        }

        public void InsertFront(ref Library.Reader reader, int size)
        {
            var objPtr = (slObject*)_frontStack.Push(ref reader, size);
            _lookup[&objPtr->handle] = objPtr;
        }

        public void InsertBack(ref slObjectContext ctx)
        {   // 삭제된것은 지워야함...
            var objPtr = (slObject*)ctx.ObjectPtr.ToPointer();
            objPtr->capacity = ctx.ObjectLength + ctx.StackOffset;

            _backStack.Push(objPtr, ctx.ObjectLength);
            _backStack.Push(ctx.StackPtr.ToPointer(), ctx.StackOffset);
            _lookup[&objPtr->handle] = objPtr;
        }

        public slObject* Create(int dataIndex)
        {
            var pObj = slObject.CreateNew(ref _backStack, GenerateHandle);
            _lookup.Add(&pObj->handle, pObj);
            return pObj;
        }
        
        public void Destroy(slObject* obj)
        {
            var objChk = Library.umnChunk.GetChunk(obj);
            objChk->Disposed = true;

            _lookup.Remove(&obj->handle);
        }

    }

    #region OLD
    //internal unsafe struct ObjectCollection
    //{
    //    public struct Enumerator
    //    {
    //        private Library.umnChunk* root;
    //        private Library.umnChunk* cur;
    //
    //        public slObject* Current => Library.umnChunk.GetPtr<slObject>( cur );
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
    //            cur = cur->next;
    //
    //            return cur != null;
    //        }
    //
    //        public void Reset()
    //        {
    //            cur = null;
    //        }
    //    }
    //    
    //    private readonly Library.umnHeap _heap;
    //    private readonly Library.umnSplayBT<Library.umnHeap, slObjectComparer, slObjectHandle, slObject> _stPool;
    //
    //    public int Count => _stPool.Count;
    //    public int GenerateHandle => _stPool.Count + 1;
    //
    //    public ObjectCollection(Library.umnChunk* chk)
    //    {
    //        _heap = new Library.umnHeap(chk);
    //        _stPool = new Library.umnSplayBT<Library.umnHeap, slObjectComparer, slObjectHandle, slObject>(null);
    //    }
    //
    //    public Enumerator GetEnumerator()
    //    {
    //        return new Enumerator(_heap.Root);
    //    }
    //    
    //    public slObject* Create(int szBlock)
    //    {
    //        fixed (Library.umnHeap* ph = &_heap)
    //        {
    //            var szObj = sizeof(slObject);
    //            var szNode = sizeof(Library.sbtPairNode);
    //            var objChk = _heap.Alloc(szObj + szNode);
    //            var objIntPtr = Library.umnChunk.GetPtr(objChk);
    //
    //            var objPtr = (slObject*)objIntPtr;
    //            objPtr->Allocate(GenerateHandle, _heap.Alloc(szBlock));
    //
    //            var nodePtr = (Library.sbtPairNode*)(objPtr + szObj);
    //            nodePtr->key = & objPtr->handle;
    //            nodePtr->value = (void*)objPtr;
    //
    //            _stPool.Add(nodePtr);
    //            return objPtr;
    //        }
    //    }
    //
    //    public void Destroy(slObject* obj)
    //    {
    //        var objChk = Library.umnChunk.GetChunk(obj);
    //        objChk->Disposed = true;
    //
    //        _stPool.Remove(obj->handle);
    //    }
    //
    //    public slObject* Find(slObjectHandle handle)
    //    {
    //        if (handle == 0)
    //            return null;
    //
    //        return _stPool[handle];
    //    }
    //
    //    public void Compact()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    #endregion
}
