using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal unsafe class CellDictionary
    {
        private readonly Library.umnHeap _cellHeap;

        private readonly Library.umnFactory<Library.umnHeap, sCell> _sfCell;
        private readonly Library.umnSplayBT<Library.umnHeap, sCellComparer, sCell.Key, sCell> _sbtCell;

        public int Count => _sbtCell.Count;
        public int GenerateHandle => (int)(Library.Ticker.GlobalTicks << 4) | (_sbtCell.Count & 0xF);

        public CellDictionary(Library.umnChunk* chk, int defaultCellCount)
        {
            _cellHeap = new Library.umnHeap(chk);
            fixed (Library.umnHeap* pHeap = &_cellHeap)
            {
                _sfCell = new Library.umnFactory<Library.umnHeap, sCell>(pHeap, defaultCellCount);
                _sbtCell = new Library.umnSplayBT<Library.umnHeap, sCellComparer, sCell.Key, sCell>(pHeap, defaultCellCount);
            }
        }

        public Library.umnFactory<Library.umnHeap, sCell>.Enumerator GetEnumerator()
        {
            return _sfCell.GetEnumerator();
        }
        
        public sCell* CreateCell()
        {
            var pCell = _sfCell.GetObject();
            pCell->Take(GenerateHandle);
            return pCell;
        }

        public void Add(sCell* node)
        {
            _sbtCell.Add(node->Handle, node);
        }

        public bool TryGetValue(sCell.Key key, out sCell* value)
        {
            return _sbtCell.TryGetValue(key, out value);
        }

        public bool Remove(sCell* node)
        {
            node->Release();
            _sfCell.PutObject(node);
            return _sbtCell.Remove(node->Handle);
        }
    }
}
