using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal unsafe struct sCellNode
    {
        public struct Key
        {
            public int obHandle;

            public static implicit operator Key(int h)
            {
                return new Key { obHandle = h };
            }
        }

        public int ObHandle { get; private set; }
        public sCell* Parent { get; private set; }
        public sCellNode* Prev { get; private set; }
        public sCellNode* Next { get; private set; }
        
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
}
