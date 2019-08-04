using System;
using System.Collections.Generic;
using System.Threading;

namespace SpeakingLanguage.Logic
{
    internal unsafe class ObjectDictionary
    {
        struct _sloComparer : Library.IumnComparer<slObject.Key>
        {
            public int Compare(slObject.Key* x, slObject.Key* y)
            {
                return x->handle.CompareTo(y->handle);
            }
        }

        private readonly Library.umnHeap _obHeap;

        private readonly Library.umnFactory<Library.umnHeap, slObject> _sfOb;
        private readonly Library.umnSplayBT<Library.umnHeap, _sloComparer, slObject.Key, slObject> _sbtOb;

        public int Count => _sbtOb.Count;
        public int GenerateHandle => (int)(Library.Ticker.GlobalTicks << 4) | (_sbtOb.Count & 0xF);

        public ObjectDictionary(Library.umnChunk* chk,int defaultObCount)
        {
            _obHeap = new Library.umnHeap(chk);
            fixed (Library.umnHeap* pHeap = &_obHeap)
            {
                _sfOb = new Library.umnFactory<Library.umnHeap, slObject>(pHeap, defaultObCount);
                _sbtOb = new Library.umnSplayBT<Library.umnHeap, _sloComparer, slObject.Key, slObject>(pHeap, defaultObCount);
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
            return ob;
        }

        public void Add(slObject* node)
        {
            _sbtOb.Add(node->Handle, node);
        }

        public bool TryGetValue(slObject.Key key, out slObject* value)
        {
            return _sbtOb.TryGetValue(key, out value);
        }

        public bool Remove(slObject* node)
        {
            node->Release();
            _sfOb.PutObject(node);
            return _sbtOb.Remove(node->Handle);
        }
    }
}
