using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal unsafe class CellDictionary
    {
        private readonly Library.umnHeap _cellHeap;

        private readonly Library.umnFactory<Library.umnHeap, sCell> _sfCell;
        private readonly Library.umnSplayBT<Library.umnHeap, sCellComparer, sCell.Key, sCell> _sbtCell;
        private readonly Library.umnFactory<Library.umnHeap, sCellNode> _sfCellNode;
        private readonly Library.umnSplayBT<Library.umnHeap, sCellNodeComparer, sCellNode.Key, sCellNode> _sbtCellNode;
        
        public CellDictionary(Library.umnHeap* allocator, int capacity, int defaultCellCount)
        {
            _cellHeap = new Library.umnHeap(allocator->Alloc(capacity));
            fixed (Library.umnHeap* pHeap = &_cellHeap)
            {
                _sfCell = new Library.umnFactory<Library.umnHeap, sCell>(pHeap, defaultCellCount);
                _sbtCell = new Library.umnSplayBT<Library.umnHeap, sCellComparer, sCell.Key, sCell>(pHeap, defaultCellCount);
                _sfCellNode = new Library.umnFactory<Library.umnHeap, sCellNode>(pHeap, defaultCellCount);
                _sbtCellNode = new Library.umnSplayBT<Library.umnHeap, sCellNodeComparer, sCellNode.Key, sCellNode>(pHeap, defaultCellCount);
            }
        }

        public Library.umnSplayBT<Library.umnHeap, sCellComparer, sCell.Key, sCell>.Enumerator GetCellEnumerator()
        {
            return _sbtCell.GetEnumerator();
        }

        public Library.umnSplayBT<Library.umnHeap, sCellNodeComparer, sCellNode.Key, sCellNode>.Enumerator GetCellNodeEnumerator()
        {
            return _sbtCellNode.GetEnumerator();
        }

        public sCellNode* CreateCellNode()
        {
            return _sfCellNode.GetObject();
        }

        public void Add(sCellNode* node)
        {
            _sbtCellNode.Add(node->ObHandle, node);
        }

        public bool TryGetValue(sCellNode.Key key, out sCellNode* value)
        {
            return _sbtCellNode.TryGetValue(key, out value);
        }

        public bool Remove(sCellNode* node)
        {
            node->Release();
            _sfCellNode.PutObject(node);
            return _sbtCellNode.Remove(node->ObHandle);
        }

        public sCell* CreateCell()
        {
            return _sfCell.GetObject();
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
