using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Collection
{
    internal unsafe struct InteractGroup :IEnumerator<Interaction>
    {
        private Library.umnArray<Interaction> _arrInter;

        public int Size => _arrInter.Length;

        public Interaction Current => *(_arrInter.PopBack());
        object IEnumerator.Current => Current;

        public InteractGroup(Library.umnChunk* chk)
        {
            _arrInter = new Library.umnArray<Interaction>(chk);
        }

        public void Insert(Interaction st)
        {
            _arrInter.PushBack(&st);
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_arrInter.Length <= 0)
                return false;

            return true;
        }

        public void Reset()
        {
        }
    }
}
