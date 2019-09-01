using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
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
        private List<TData> _lstReadData;
        private List<TData> _lstWriteData;
        
        public DataList(int capacity) : this(capacity, null)
        {
        }

        public DataList(int capacity, IComparer<TData> comparer)
        {
            _comparer = comparer;
            _lstReadData = new List<TData>(capacity);
            _lstWriteData = new List<TData>(capacity);
        }

        public void Add(TData data)
        {
            _lstWriteData.Add(data);
        }

        public Enumerator GetSortedEnumerator()
        {
            _lstReadData.Sort(_comparer);
            return new Enumerator(_lstReadData);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_lstReadData);
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
            var temp = _lstReadData;
            _lstReadData = _lstWriteData;
            _lstWriteData = temp;
            _lstWriteData.Clear();
        }
    }
}
