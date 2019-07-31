using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal unsafe struct sCell
    {
        public struct Key
        {
            public int handle;

            public static implicit operator Key(int h)
            {
                return new Key { handle = h };
            }
        }

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
        
        public void Take(int handle, sCell* rc, sCell* bc, sCell* rbc)
        {
            Handle = handle;
            _rightCell = rc;
            _bottomCell = bc;
            _rightBottomCell = rbc;
            _tail = null;
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
}
