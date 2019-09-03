using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Container
{
    internal unsafe struct InteractGroup :IEnumerator<Interaction>
    {
        private readonly Library.umnArray<Interaction>.Indexer _interactions;
        private int _begin, _current, _end;
        
        public int Length => _end - _begin + 1;

        public Interaction Current => *_interactions[_current];
        object IEnumerator.Current => Current;

        public InteractGroup(Library.umnArray<Interaction>.Indexer indexer, int begin, int end)
        {
            _interactions = indexer;
            _begin = begin;
            _end = end;
            _current = -1;
        }
        
        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_current == -1)
                _current = _begin;
            else
                _current++;

            return _current < _end;
        }

        public void Reset()
        {
            _current = -1;
        }
    }
}
