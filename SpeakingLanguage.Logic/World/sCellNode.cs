using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.World
{
    internal unsafe struct sCellNode
    {
        public int ObHandle { get; private set; }
        public sCell* Parent { get; private set; }
        public sCellNode* Prev { get; private set; }
        public sCellNode* Next { get; private set; }

        public sCellNode(int handle)
        {
            ObHandle = handle;
            Parent = null;
            Prev = null;
            Next = null;
        }

        public void Take(int handle)
        {
            ObHandle = handle;
        }

        public void Release()
        {
            ObHandle = 0;
            Unlink();
        }

        public void Link(sCell* cell)
        {
            fixed (sCellNode* pth = &this)
            {
                Parent = cell;
                cell->Tail->Next = pth;
                Prev = cell->Tail;
            }
        }

        public void Unlink()
        {
            Parent = null;

            if (null != Prev)
            {
                Prev->Next = Next;
                Prev = null;
            }

            if (null != Next)
            {
                Next->Prev = Prev;
                Next = null;
            }
        }
    }

    internal class CellNode
    {
        public int Handle { get; private set; }
        public Cell Parent { get; private set; }
        public CellNode Prev { get; private set; }
        public CellNode Next { get; private set; }

        public void Take(int handle)
        {
            Handle = handle;
        }

        public void Release()
        {
            Handle = 0;
        }

        public void Link(Cell cell)
        {
            Parent = cell;
            cell.Tail.Next = this;
            Prev = cell.Tail;
        }

        public void Unlink()
        {
            Parent = null;

            if (null != Prev)
            {
                Prev.Next = Next;
                Prev = null;
            }

            if (null != Next)
            {
                Next.Prev = Prev;
                Next = null;
            }
        }
    }
}
