using System;
using System.Collections.Generic;
using System.Threading;

namespace SpeakingLanguage.Logic
{
    internal unsafe class ObjectDictionary
    {
        struct _slPtr
        {   // index 접근을 보장해주는 slobject의 래퍼 포인터
            public slObject* ptr;
        }

        struct _sloComparer : Library.IumnComparer<slObject.Key>
        {
            public int Compare(slObject.Key* x, slObject.Key* y)
            {
                return x->handle.CompareTo(y->handle);
            }
        }

        private readonly Library.umnHeap _obHeap;

        private readonly Library.umnFactory<Library.umnHeap, slObject> _sfOb;
        private readonly Library.umnArray<_slPtr> _arrPtr;
        //private readonly Library.umnSplayBT<Library.umnHeap, _sloComparer, slObject.Key, slObject> _sbtOb;

        public int Count => _arrPtr.Length;
        //public int GenerateHandle => (int)(Library.Ticker.GlobalTicks << 4) | (_sbtOb.Count & 0xF);
        public int GenerateHandle => _arrPtr.Length;

        public ObjectDictionary(Library.umnChunk* chk,int defaultObCount)
        {
            _obHeap = new Library.umnHeap(chk);
            fixed (Library.umnHeap* pHeap = &_obHeap)
            {
                _sfOb = new Library.umnFactory<Library.umnHeap, slObject>(pHeap, defaultObCount);
                _arrPtr = new Library.umnArray<_slPtr>(_obHeap.Calloc(defaultObCount));
                //_sbtOb = new Library.umnSplayBT<Library.umnHeap, _sloComparer, slObject.Key, slObject>(pHeap, defaultObCount);
            }
        }

        public Library.umnFactory<Library.umnHeap, slObject>.Enumerator GetEnumerator()
        {
            return _sfOb.GetEnumerator();
        }
        
        public slObject* CreateObserver(int szBlock)
        {
            var ob = _sfOb.GetObject();
            ob->Take(GenerateHandle, _obHeap.Alloc(szBlock));
            // arr에 추가하는 구현 필요
            return ob;
        }

        public slObject* Find(int handle)
        {
            return _arrPtr[handle]->ptr;
        }
        //public void Add(slObject* node)
        //{
        //    _sbtOb.Add(node->Handle, node);
        //}
        //
        //public bool TryGetValue(slObject.Key key, out slObject* value)
        //{
        //    return _sbtOb.TryGetValue(key, out value);
        //}

        public bool Remove(slObject* node)
        {
            node->Release();
            _sfOb.PutObject(node);
            // arr에 제거하는 구현 필요. 표시만 해두었다가 한번에 제거해야 한다.
            //return _sbtOb.Remove(node->Handle);
            return true;
        }
    }
}
