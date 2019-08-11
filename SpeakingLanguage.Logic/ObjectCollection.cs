using System;
using System.Collections.Generic;
using System.Threading;

namespace SpeakingLanguage.Logic
{
    internal unsafe struct ObjectCollection
    {
        public struct Enumerator
        {
            private IntPtr th_slObject;
            private Library.umnChunk* root;
            private Library.umnChunk* cur;

            public slObject* Current => Library.umnChunk.GetPtr<slObject>( cur );

            public Enumerator(Library.umnChunk* chk)
            {
                th_slObject = typeof(slObject).TypeHandle.Value;
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

                while (cur != null)
                {
                    if (cur->typeHandle == th_slObject)
                        break;
                    cur = cur->next;
                }

                return cur != null;
            }

            public void Reset()
            {
                cur = null;
            }
        }
        
        private struct _objComparer : Library.IumnComparer<slObject.Key>
        {
            public int Compare(slObject.Key* x, slObject.Key* y)
            {
                var xh = x->handle;
                var yh = y->handle;
                if (xh == yh)
                    return 0;
                return xh < yh ? -1 :  1;
            }
        }

        private readonly Library.umnHeap _heap;
        private readonly Library.umnSplayBT<Library.umnHeap, _objComparer, slObject.Key, slObject> _stPool;

        public int Count => _stPool.Count;
        public int GenerateHandle => _stPool.Count + 1;

        public ObjectCollection(Library.umnChunk* chk)
        {
            _heap = new Library.umnHeap(chk);
            fixed (Library.umnHeap* ph = &_heap)
                _stPool = new Library.umnSplayBT<Library.umnHeap, _objComparer, slObject.Key, slObject>(ph);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_heap.Root);
        }
        
        public slObject* Create(int szBlock)
        {
            slObject* obj;
            fixed (Library.umnHeap* ph = &_heap)
                obj = slObject.Create(ph, GenerateHandle, szBlock);

            _stPool.Add(obj->Handle, obj);
            return obj;
        }

        public void Destroy(slObject* obj)
        {
            var objChk = Library.umnChunk.GetChunk(obj);
            objChk->Disposed = true;

            _stPool.Remove(obj->Handle);
        }

        public slObject* Find(slObject.Key handle)
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
