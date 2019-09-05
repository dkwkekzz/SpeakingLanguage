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
                    cur = Library.umnChunk.GetPtr<slObject>(root);
                else
                    cur = slObjectHelper.GetNext(cur);

                if (null == cur)
                    return false;

                while (cur->handle.value < 0)
                {
                    cur = slObjectHelper.GetNext(cur);
                    if (cur == null)
                        break;
                }
                
                return null != cur;
            }

            public void Reset()
            {
                cur = null;
            }
        }
        
        private readonly Library.umnMarshal _umnAllocator;
        private Library.umnStack _readStack;
        private Library.umnStack _writeStack;
        private Library.umnHashMap<slObjectEqualityComparer, slObjectHandle, slObject> _lookup;

        public int Count => _lookup.Count;

        public slObjectCollection(int defaultObjectCount)
        {
            _umnAllocator = new Library.umnMarshal();
            _readStack = new Library.umnStack(_umnAllocator.Alloc(defaultObjectCount * 64));
            _writeStack = new Library.umnStack(_umnAllocator.Alloc(defaultObjectCount * 64));
            _lookup = Library.umnHashMap<slObjectEqualityComparer, slObjectHandle, slObject>.CreateNew(ref _umnAllocator, defaultObjectCount);
        }

        private static int generator;
        public static int GenerateHandle
        {
            get { return generator++; }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_readStack.Root);
        }

        IEnumerator<slObject> IEnumerable<slObject>.GetEnumerator()
        {
            return new Enumerator(_readStack.Root);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(_readStack.Root);
        }
        
        public void Dispose()
        {
            _umnAllocator.Dispose();
        }
        
        public void SwapBuffer()
        {
            // lookup reset at last step for multithreading
            var iter = new Enumerator(_writeStack.Root);
            while (iter.MoveNext())
            {
                var objPtr = iter.Current;
                _lookup[&objPtr->handle] = objPtr;
            }

            Library.umnStack.Swap(ref _readStack, ref _writeStack);
        }

        public slObject* Find(slObjectHandle handle)
        {
            if (handle == 0)
                return null;

            return _lookup[handle];
        }

        public void InsertFront(ref Library.Reader reader)
        {
            var objPtr = (slObject*)_readStack.Push(ref reader);
            if (null == objPtr)
            {
                bool read = reader.ReadInt(out int handleValue);
                if (!read || handleValue < 0)
                    Library.ThrowHelper.ThrowWrongArgument($"Fail to deserialize object");

                objPtr = slObject.CreateDefault(ref _readStack, handleValue);
            }

            _lookup[&objPtr->handle] = objPtr;
        }

        public void InsertBack(ref slSubject subject)
        {   
            var objPtr = (slObject*)subject.ObjectPtr.ToPointer();
            objPtr->Append(subject.StackOffset);
        
            _writeStack.Push(objPtr, subject.ObjectLength);
            _writeStack.Push(subject.StackPtr.ToPointer(), subject.StackOffset);
        }
        
        public slObject* Create(int handleValue)
        {
            var pObj = slObject.CreateDefault(ref _writeStack, handleValue);
            _lookup.Add(&pObj->handle, pObj);
            return pObj;
        }
        
        public void Destroy(slObject* obj)
        {
            var objChk = Library.umnChunk.GetChunk(obj);
            objChk->Disposed = true;

            _lookup.Remove(&obj->handle);
            obj->Dispose();
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
