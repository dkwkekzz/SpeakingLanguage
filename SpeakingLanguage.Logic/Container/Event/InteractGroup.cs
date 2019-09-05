using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Container
{
    internal unsafe struct InteractGroup : IEnumerable<InteractPair>
    {
        public struct Enumerator : IEnumerator<InteractPair>
        {
            private readonly Library.umnArray<InteractPair>.Indexer _interactions;
            private int _begin, _end;
            private int _current;

            public InteractPair Current => *_interactions[_current];
            object IEnumerator.Current => Current;
            public int ChildLength { get; private set; }

            public Enumerator(ref InteractGroup group)
            {
                _interactions = group._interactions;
                _begin = group._begin;
                _end = group._end;
                _current = -1;
                
                ChildLength = 0;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_current == -1)
                    _current = _begin;

                if (_current >= _end)
                    return false;

                if (ChildLength != 0)
                    Library.ThrowHelper.ThrowWrongState($"Please call MoveNextChild to end. ChildLength: {ChildLength.ToString()}");

                ++_current;
                ChildLength = _interactions[_current]->count;
                return _current < _end;
            }

            public bool MoveNextChild()
            {
                if (_current == -1)
                    _current = _begin;

                if (_current >= _end)
                    return false;

                if (ChildLength == 0)
                    return false;

                ++_current;
                --ChildLength;
                return _current < _end;
            }

            public void Reset()
            {
                _current = -1;
                ChildLength = 0;
            }
        }

        private readonly Library.umnArray<InteractPair>.Indexer _interactions;
        private int _begin, _end;
        
        public int Length => _end - _begin + 1;
        
        public InteractGroup(Library.umnArray<InteractPair>.Indexer indexer, int begin, int end)
        {
            _interactions = indexer;
            _begin = begin;
            _end = end;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(ref this);
        }

        IEnumerator<InteractPair> IEnumerable<InteractPair>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
