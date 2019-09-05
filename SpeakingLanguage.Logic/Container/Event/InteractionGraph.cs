using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpeakingLanguage.Logic.Container
{
    // umn버전으로 바꿔야함... service안의 객체들은 umn으로 접근해야함
    internal struct InteractionGraph : IDisposable
    {
        private sealed class EdgeList : List<slObjectHandle>
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
        
        private readonly Dictionary<slObjectHandle, EdgeList> _dicGraph;
        private readonly Queue<EdgeList> _listPool;
        private readonly Queue<slObjectHandle> _queue;

        private readonly Library.umnMarshal _allocator;
        private readonly Library.umnArray<InteractPair> _arrPair;
        private readonly int _defaultInteractCount;

        public InteractionGraph(int defaultObjectCount, int defaultInteractCount)
        {
            _dicGraph = new Dictionary<slObjectHandle, EdgeList>(defaultObjectCount);
            _listPool = new Queue<EdgeList>();
            _queue = new Queue<slObjectHandle>();

            _allocator = new Library.umnMarshal();
            _arrPair = Library.umnArray<InteractPair>.CreateNew(ref _allocator, defaultObjectCount * defaultInteractCount);
            _defaultInteractCount = defaultInteractCount;
        }

        public void Dispose()
        {
            _allocator.Dispose();
        }

        public void Insert(ref Interaction stInter)
        {
            if (!_dicGraph.TryGetValue(stInter.subject, out EdgeList list))
            {
                if (_listPool.Count > 0)
                    list = _listPool.Dequeue();
                else
                    list = new EdgeList(_dicGraph.Count, _defaultInteractCount);
                _dicGraph.Add(stInter.subject, list);
            }
            
            // [!] 성능상 문제. 
            if (!list.Contains(stInter.target))
                list.Add(stInter.target);
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
            _arrPair.Clear();
        }

        public unsafe bool TryGetInteractGroup(ref slObjectCollection.Enumerator objIter, int capacity, out InteractGroup group)
        {
            int begin = _arrPair.Length;
            int count = 0;
            while (objIter.MoveNext())
            {
                var pObj = objIter.Current;
                count += _bfsSelect(pObj->handle);
                if (capacity >= 0 && count >= capacity)
                    break;
            }

            if (count == 0)
            {
                group = default;
                return false;
            }

            group = new InteractGroup(_arrPair.GetIndexer(), begin, begin + count);
            return true;
        }

        private unsafe int _bfsSelect(slObjectHandle first)
        {
            Library.Tracer.Assert(_queue.Count == 0);

            int count = 0;
            
            _queue.Enqueue(first);
            while (_queue.Count > 0)
            {
                var here = _queue.Dequeue();

                int length = 0;
                if (_dicGraph.TryGetValue(here, out EdgeList thereList))
                    length = thereList.Count;

                _arrPair.PushBack(new InteractPair(here, length));
                count++;

                if (thereList.Order > 0 || length == 0)
                    continue;
                thereList.Order = 1;

                for (int i = 0; i != length; i++)
                {
                    var there = thereList[i];
                    _queue.Enqueue(there);
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
        public Interaction Current => new Interaction { subject = _selectedKey, target = _graph[_here] };
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
