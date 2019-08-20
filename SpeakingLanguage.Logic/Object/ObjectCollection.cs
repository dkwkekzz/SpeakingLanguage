using System;
using System.Collections.Generic;
using System.Threading;

namespace SpeakingLanguage.Logic
{
    internal unsafe struct ObjectCollection
    {
        public struct Enumerator
        {
            private Library.umnChunk* root;
            private Library.umnChunk* cur;

            public slObject* Current => Library.umnChunk.GetPtr<slObject>( cur );

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
                cur = cur->next;

                return cur != null;
            }

            public void Reset()
            {
                cur = null;
            }
        }
        
        private readonly Library.umnHeap _heap;
        private readonly Library.umnSplayBT<Library.umnHeap, slObjectComparer, slObjectHandle, slObject> _stPool;

        public int Count => _stPool.Count;
        public int GenerateHandle => _stPool.Count + 1;

        public ObjectCollection(Library.umnChunk* chk)
        {
            _heap = new Library.umnHeap(chk);
            _stPool = new Library.umnSplayBT<Library.umnHeap, slObjectComparer, slObjectHandle, slObject>(null);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_heap.Root);
        }
        
        public slObject* Create(int szBlock)
        {
            fixed (Library.umnHeap* ph = &_heap)
            {
                var szObj = sizeof(slObject);
                var szNode = sizeof(Library.sbtPairNode);
                var objChk = _heap.Alloc(szObj + szNode);
                var objIntPtr = Library.umnChunk.GetPtr(objChk);

                var objPtr = (slObject*)objIntPtr;
                objPtr->Allocate(GenerateHandle, _heap.Alloc(szBlock));

                var nodePtr = (Library.sbtPairNode*)(objPtr + szObj);
                nodePtr->key = & objPtr->handle;
                nodePtr->value = (void*)objPtr;

                _stPool.Add(nodePtr);
                return objPtr;
            }
        }

        public void Destroy(slObject* obj)
        {
            var objChk = Library.umnChunk.GetChunk(obj);
            objChk->Disposed = true;

            _stPool.Remove(obj->handle);
        }

        public slObject* Find(slObjectHandle handle)
        {
            if (handle == 0)
                return null;

            return _stPool[handle];
        }

        public void Compact()
        {
            throw new NotImplementedException();
        }
    }
}
