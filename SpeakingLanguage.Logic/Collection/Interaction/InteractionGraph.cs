using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpeakingLanguage.Logic.Collection
{
    // umn버전으로 바꿔야함... service안의 객체들은 umn으로 접근해야함
    internal struct InteractionGraph
    {
        private readonly Dictionary<slObjectHandle, List<slObjectHandle>> _dicGraph;
        private readonly int _defaultInteractCount;
        
        public InteractionGraph(int defaultObjectCount, int defaultInteractCount)
        {
            _dicGraph = new Dictionary<slObjectHandle, List<slObjectHandle>>(defaultObjectCount);
            _defaultInteractCount = defaultInteractCount;
        }
        
        public void Insert(slObjectHandle subject, slObjectHandle target)
        {
            //slObjectHandle subject, target;
            //if (interaction.lhs.value > interaction.rhs.value)
            //{
            //    subject = interaction.rhs;
            //    target = interaction.lhs;
            //}
            //else
            //{
            //    subject = interaction.lhs;
            //    target = interaction.rhs;
            //}
            
            if (!_dicGraph.TryGetValue(subject, out List<slObjectHandle> list))
            {
                    list = new List<slObjectHandle>(_defaultInteractCount);
                _dicGraph.Add(subject, list);
            }
            
            if (list.Contains(target))
                return;

            list.Add(target);
        }

        public bool Remove(slObjectHandle subject)
        {
            return _dicGraph.Remove(subject);
        }

        public void Reset()
        {
            var iter = _dicGraph.Values.GetEnumerator();
            while (iter.MoveNext())
            {
                iter.Current.Clear();
            }
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
