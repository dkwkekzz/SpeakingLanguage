using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpeakingLanguage.Logic.Container
{
    // umn버전으로 바꿔야함... service안의 객체들은 umn으로 접근해야함
    internal struct InteractionGraph : IDisposable
    {
        private class EdgeList : List<slObjectHandle>
        {
            public int Index { get; }
            public int Order { get; set; }

            public EdgeList(int index, int capacity) : base(capacity)
            {
                Index = index;
                Order = 0;
            }

            public void Reset()
            {
                base.Clear();
                Order = 0;
            }
        }

        // [!] 혼자인 객체라도 전부 넣어주면 별도의 과정이 필요없다. 
        private readonly Dictionary<slObjectHandle, EdgeList> _dicGraph;
        private readonly Queue<EdgeList> _listPool;
        private readonly Queue<slObjectHandle> _queue;

        private readonly Library.umnMarshal _allocator;
        private readonly Library.umnArray<int> _arrInter;
        private readonly int _defaultInteractCount;

        private Dictionary<slObjectHandle, EdgeList>.Enumerator _graphEnumerator;
        private int _eventCount;

        public InteractionGraph(int defaultObjectCount, int defaultInteractCount)
        {
            _dicGraph = new Dictionary<slObjectHandle, EdgeList>(defaultObjectCount);
            _listPool = new Queue<EdgeList>();
            _queue = new Queue<slObjectHandle>();

            _allocator = new Library.umnMarshal();
            _arrInter = Library.umnArray<int>.CreateNew(ref _allocator, defaultObjectCount * defaultInteractCount);
            _defaultInteractCount = defaultInteractCount;

            _graphEnumerator = _dicGraph.GetEnumerator();
            _eventCount = 0;
        }

        public void Dispose()
        {
            _allocator.Dispose();
        }

        public void Insert(slObjectHandle subject, slObjectHandle target)
        {
            if (!_dicGraph.TryGetValue(subject, out EdgeList list))
            {
                if (_listPool.Count > 0)
                    list = _listPool.Dequeue();
                else
                    list = new EdgeList(_dicGraph.Count, _defaultInteractCount);
                _dicGraph.Add(subject, list);
            }
            
            // [!] 양방향 여부를 구분해서 넣을 필요가 있다.
            if (list.Contains(target))
                return;

            list.Add(target);
            _eventCount++;
        }

        public bool Remove(slObjectHandle subject)
        {
            _listPool.Enqueue(_dicGraph[subject]);
            return _dicGraph.Remove(subject);
        }

        public void Reset()
        {
            var iter = _dicGraph.Values.GetEnumerator();
            while (iter.MoveNext())
            {
                iter.Current.Reset();
            }
            _arrInter.Clear();

            _graphEnumerator = _dicGraph.GetEnumerator();
            _eventCount = 0;
        }

        public bool TryGetInteractGroup(int capacity, out InteractGroup group)
        {
            if (capacity < 0)
                capacity = _eventCount;

            int begin = _arrInter.Length;
            int count = 0;
            while (_graphEnumerator.MoveNext())
            {
                var pair = _graphEnumerator.Current;
                if (pair.Value.Order > 0)
                    continue;

                count += _bfsSelect(pair.Key);
                if (count >= capacity)
                    break;
            }

            if (count == 0)
            {
                group = default;
                return false;
            }

            group = new InteractGroup(_arrInter.GetIndexer(), begin, begin + count);
            return true;
        }

        private int _bfsSelect(slObjectHandle first)
        {
            if (_queue.Count > 0)
                _queue.Clear();

            int order = 0, count = 0;
            
            _queue.Enqueue(first);
            while (_queue.Count > 0)
            {
                var here = _queue.Dequeue();
                if (!_dicGraph.TryGetValue(here, out EdgeList thereList))
                    continue;

                thereList.Order = ++order;
                unsafe
                {
                    // rule: subject -> target1, target2, ... -1, subject -> t1, t2...
                    _arrInter.PushBack(here.value);
                }

                var length = thereList.Count;
                unsafe
                {
                    _arrInter.PushBack(length);
                }

                for (int i = 0; i != length; i++)
                {
                    var there = thereList[i];
                    _queue.Enqueue(there);

                    unsafe
                    {
                        //_arrInter.PushBack(new Interaction { lhs = here, rhs = there });
                        _arrInter.PushBack(there.value);
                    }
                    count++;
                }
            }

            return count;
        }
    }

    internal struct InteractionGraph2 : IEnumerator<Interaction>
    {
        private List<int> _graph;
        private Dictionary<int, int> _indexMap;
        private Queue<int> _waitQueue;
        private int _selectedKey;
        private int _here;

        public int CurrentKey => _selectedKey;
        public int CurrentValue => _graph[_here];
        public Interaction Current => new Interaction { lhs = _selectedKey, rhs = _graph[_here] };
        object IEnumerator.Current => Current;

        public InteractionGraph2(List<int> list, Dictionary<int, int> map, Queue<int> queue)
        {
            _graph = list;
            _indexMap = map;
            _waitQueue = queue;
            _selectedKey = -1;
            _here = -1;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_waitQueue.Count == 0)
            {
                if (_indexMap.Count == 0)
                {
                    _selectedKey = -1;
                    _here = -1;
                    return false;
                }

                var first = _indexMap.First();
                _selectedKey = first.Key;
                _waitQueue.Enqueue(first.Value);
            }

            _here = _waitQueue.Dequeue();
            if (_here == -1)
            {
                _selectedKey = _waitQueue.Dequeue();
                _here = _waitQueue.Dequeue();
            }

            if (!_indexMap.TryGetValue(_here, out int thereIdx))
                return true;
            _indexMap.Remove(_here);

            _waitQueue.Enqueue(-1);
            _waitQueue.Enqueue(_here);
            while (thereIdx < _graph.Count)
            {
                var there = _graph[thereIdx];
                if (there < 0)
                    break;

                _waitQueue.Enqueue(there);
                thereIdx++;
            }

            return true;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
