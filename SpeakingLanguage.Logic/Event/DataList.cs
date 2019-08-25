using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal struct DataPair<TData>
    {
        public int frame;
        public TData data;
    }

    internal struct DataList<TData> : IEnumerable<TData>
        where TData : IEventData<TData>
    {
        public struct Enumerator : IEnumerator<TData>
        {
            private List<TData> _list;
            private int _index;
            private int _length;

            public Enumerator(List<TData> list)
            {
                _list = list;
                Current = default;
                _index = -1;
                _length = list.Count;
            }

            public TData Current { get; private set; }
            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                while (true)
                {
                    if (_length <= ++_index)
                        return false;

                    var current = _list[_index];
                    if (current.Equals(Current))
                        continue;

                    Current = current;
                    return true;
                }
            }

            public void Reset()
            {
                _index = -1;
            }
        }

        private readonly IComparer<TData> _comparer;
        private List<TData> _lstData;
        private Queue<DataPair<TData>> _lstBackData;

        public int CurrentFrame { get; private set; }
        public int Count => _lstData.Count;

        public DataList(int capacity) : this(capacity, null)
        {
        }

        public DataList(int capacity, IComparer<TData> comparer)
        {
            _comparer = comparer;
            _lstData = new List<TData>(capacity);
            _lstBackData = new Queue<DataPair<TData>>(capacity >> 1);

            CurrentFrame = 0;
        }

        public void Add(DataPair<TData> pair)
        {
            var dataCount = _lstData.Count;
            if (dataCount == 0)
            {
                CurrentFrame = pair.frame;
                _lstData.Add(pair.data);
                return;
            }

            if (CurrentFrame == pair.frame)
                _lstData.Add(pair.data);
            else
                _lstBackData.Enqueue(pair);
        }

        public Enumerator GetSortedEnumerator()
        {
            _lstData.Sort(_comparer);
            return new Enumerator(_lstData);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_lstData);
        }

        IEnumerator<TData> IEnumerable<TData>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Swap()
        {
            _lstData.Clear();

            var backDataCount = _lstBackData.Count;
            if (backDataCount == 0) return;

            CurrentFrame = _lstBackData.Peek().frame;
            for (int i = 0; i < backDataCount; i++)
            {
                var pair = _lstBackData.Dequeue();
                if (pair.frame != CurrentFrame)
                    break;

                _lstData.Add(pair.data);
            }
        }
    }
}
