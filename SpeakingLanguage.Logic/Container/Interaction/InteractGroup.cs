using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Container
{
    internal unsafe struct InteractGroup : IEnumerable<slObjectHandle>
    {
        public struct Enumerator : IEnumerator<slObjectHandle>
        {
            private static readonly slObjectHandle EmptyHandle = new slObjectHandle(-1);

            private readonly Library.umnArray<int>.Indexer _interactions;
            private int _begin, _end;
            private int _current;

            public slObjectHandle Current => *_interactions[_current];
            object IEnumerator.Current => Current;
            public slObjectHandle CurrentSubject { get; private set; }
            public int TargetLength { get; private set; }

            public Enumerator(ref InteractGroup group)
            {
                _interactions = group._interactions;
                _begin = group._begin;
                _end = group._end;
                _current = -1;

                CurrentSubject = EmptyHandle;
                TargetLength = 0;
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

                if (TargetLength == 0)
                {
                    Library.Tracer.Assert(_current + 1 < _end);
                    CurrentSubject = new slObjectHandle(*_interactions[_current++]);
                    TargetLength = *_interactions[_current++];
                }

                TargetLength--;
                return _current++ < _end;
            }

            public void Reset()
            {
                _current = -1;
                CurrentSubject = EmptyHandle;
                TargetLength = 0;
            }
        }

        private readonly Library.umnArray<int>.Indexer _interactions;
        private int _begin, _end;
        
        public int Length => _end - _begin + 1;
        
        public InteractGroup(Library.umnArray<int>.Indexer indexer, int begin, int end)
        {
            _interactions = indexer;
            _begin = begin;
            _end = end;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(ref this);
        }

        IEnumerator<slObjectHandle> IEnumerable<slObjectHandle>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
