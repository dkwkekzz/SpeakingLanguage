using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal unsafe class ObserverDictionary
    {
        private readonly Library.umnHeap _obHeap;

        private readonly Library.umnFactory<Library.umnHeap, sObserver> _sfOb;
        private readonly Library.umnSplayBT<Library.umnHeap, sObserverComparer, sObserver.Key, sObserver> _sbtOb;

        public ObserverDictionary(Library.umnHeap* allocator, int capacity, int defaultObCount)
        {
            _obHeap = new Library.umnHeap(allocator->Alloc(capacity));
            fixed (Library.umnHeap* pHeap = &_obHeap)
            {
                _sfOb = new Library.umnFactory<Library.umnHeap, sObserver>(pHeap, defaultObCount);
                _sbtOb = new Library.umnSplayBT<Library.umnHeap, sObserverComparer, sObserver.Key, sObserver>(pHeap, defaultObCount);
            }
        }

        public Library.umnSplayBT<Library.umnHeap, sObserverComparer, sObserver.Key, sObserver>.Enumerator GetEnumerator()
        {
            return _sbtOb.GetEnumerator();
        }

        public sObserver* CreateObserver(int szBlock)
        {
            var ob = _sfOb.GetObject();
            fixed (Library.umnHeap* pHeap = &_obHeap)
                ob->Take((int)Library.Ticker.GlobalTicks, pHeap, szBlock);
            return ob;
        }

        public void Add(sObserver* node)
        {
            _sbtOb.Add(node->Handle, node);
        }

        public bool TryGetValue(sObserver.Key key, out sObserver* value)
        {
            return _sbtOb.TryGetValue(key, out value);
        }

        public bool Remove(sObserver* node)
        {
            node->Release();
            _sfOb.PutObject(node);
            return _sbtOb.Remove(node->Handle);
        }
    }
}
