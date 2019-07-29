using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.World
{
    internal unsafe struct sCell
    {
        public struct Enumerator
        {
            private sCell* root;
            private sCellNode* cur;

            public sCellNode* Current => cur;

            public Enumerator(sCell* cell)
            {
                root = cell;
                cur = null;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (null == cur)
                    cur = root->Tail;
                else
                    cur = cur->Next;
                return cur != null;
            }

            public void Reset()
            {
                cur = null;
            }
        }

        public static readonly int BLOCK_BIT = 6;
        public static readonly int MAX_BIT = 16;

        private sCell* _rightCell;
        private sCell* _bottomCell;
        private sCell* _rightBottomCell;
        private sCellNode* _tail;

        public int Handle { get; private set; }
        public sCell* RightCell => _rightCell;
        public sCell* BottomCell => _bottomCell;
        public sCell* RightBottomCell => _rightBottomCell;
        public sCellNode* Tail => _tail;

        public sCell(int handle)
        {
            Handle = handle;
            _rightCell = null;
            _bottomCell = null;
            _rightBottomCell = null;
            _tail = null;
        }

        public void Take(Library.umnSplayBT<Library.umnHeap, sCell> sbtCell, int handle)
        {
            Handle = handle;

            var rh = handle + (1 << Cell.MAX_BIT);
            var bh = handle + 1;
            var rbh = handle + (1 << Cell.MAX_BIT) + 1;

            var key = new sCell();
            key.Handle = rh;
            sbtCell.TryGetValue(&key, out _rightCell);

            key.Handle = bh;
            sbtCell.TryGetValue(&key, out _bottomCell);

            key.Handle = rbh;
            sbtCell.TryGetValue(&key, out _rightBottomCell);
        }

        public void Release()
        {
            if (null != Tail)
                throw new InvalidOperationException("You must release all observers before removing this cell.");

            _rightCell = null;
            _bottomCell = null;
            _rightBottomCell = null;
        }

        public void Add(sCellNode* node)
        {
            if (_tail == null)
                _tail = node;
            else
            {
                fixed (sCell* pth = &this)
                {
                    node->Link(pth);
                }
            }
        }

        public void Remove(sCellNode* node)
        {
            if (node == _tail)
                _tail = _tail->Next;

            node->Unlink();
        }

        public Enumerator GetEnumerator()
        {
            fixed (sCell* pth = &this)
            {
                return new Enumerator(pth);
            }
        }
    }

    internal class Cell : IEnumerable<CellNode>
    {
        public struct Enumerator : IEnumerator<CellNode>
        {
            private Cell root;
            private CellNode cur;

            public CellNode Current => cur;
            object IEnumerator.Current => this.Current;

            public Enumerator(Cell cell)
            {
                root = cell;
                cur = null;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (null == cur)
                    cur = root.Tail;
                else
                    cur = cur.Next;
                return cur != null;
            }

            public void Reset()
            {
                cur = null;
            }
        }

        public static readonly int BLOCK_BIT = 6;
        public static readonly int MAX_BIT = 16;
        
        public Cell RightCell { get; private set; }
        public Cell BottomCell { get; private set; }
        public Cell RightBottomCell { get; private set; }
        public CellNode Tail { get; private set; }

        public void Take(Dictionary<int, Cell> dicCell, int handle)
        {
            var rh = handle + (1 << Cell.MAX_BIT);
            var bh = handle + 1;
            var rbh = handle + (1 << Cell.MAX_BIT) + 1;
            dicCell.TryGetValue(rh, out Cell RightCell);
            dicCell.TryGetValue(bh, out Cell BottomCell);
            dicCell.TryGetValue(rbh, out Cell RightBottomCell);
        }

        public void Release()
        {
            if (null != Tail)
                throw new InvalidOperationException("You must release all observers before removing this cell.");

            RightCell = null;
            BottomCell = null;
            RightBottomCell = null;
        }

        public void Add(CellNode node)
        {
            if (Tail == null)
                Tail = node;
            else
                node.Link(this);
        }

        public void Remove(CellNode node)
        {
            if (node == Tail)
                Tail = Tail.Next;

            node.Unlink();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<CellNode> IEnumerable<CellNode>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
