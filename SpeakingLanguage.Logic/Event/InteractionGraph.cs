using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpeakingLanguage.Logic
{
    internal struct InteractionGraph : IEnumerator<Interaction>
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

        public InteractionGraph(List<int> list, Dictionary<int, int> map, Queue<int> queue)
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
