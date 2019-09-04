using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeakingLanguage.Logic.Process
{
    internal sealed class JobPartitioner : IEnumerator<Container.InteractGroup>
    {
        private readonly ConcurrentStack<Container.InteractGroup> _chunks = new ConcurrentStack<Container.InteractGroup>();
        private readonly int _minJobchunkLength;

        public JobPartitioner(int jobchunkLength)
        {
            _minJobchunkLength = jobchunkLength;
        }

        public Container.InteractGroup Current
        {
            get
            {
                Container.InteractGroup group;
                if (!_chunks.TryPop(out group))
                    return default(Container.InteractGroup);
                return group;
            }
        }
        object IEnumerator.Current => Current;

        public void Dispose()
        {
            Reset();
        }

        public int CollectJob(ref Service service, int workerCount)
        {
            var objIter = service.colObj.GetEnumerator();
            var count = 0;
            if (workerCount == 1)
            {
                if (!service.itrGraph.TryGetInteractGroup(ref objIter, -1, out Container.InteractGroup group))
                    return count;

                _chunks.Push(group);
                return ++count;
            }

            // partitional by n
            //var offset = capacity >> 2;
            //for (int i = 0; i != workerCount; i++)
            //{
            //    _chunks.Push(new JobChunk { begin = 0, end = System.Math.Min(offset * i, capacity) });
            //}

            // partitional by small chunk
            //var head = 0;
            //var offset = Config.LENGTH_MIN_CHUNK;
            //while (head < capacity - 1)
            //{
            //    _chunks.Push(new JobChunk { begin = head, end = System.Math.Min((head += offset), capacity - 1) });
            //}

            var offset = _minJobchunkLength;
            while (true)
            {
                for (int i = 0; i != workerCount; i++)
                {
                    if (!service.itrGraph.TryGetInteractGroup(ref objIter, offset, out Container.InteractGroup group))
                        return count;

                    _chunks.Push(group);
                    ++count;
                }

                offset <<= 1;
            }
            //var log2 = Library.Math.Log2ge(capacity);
            //var depth = Library.Math.Log2ge(log2) >> 1;
            //var bigOffset = (1 << Library.Math.Log2ge(capacity)) >> depth;
            //var offset = bigOffset >> Library.Math.Log2ge(workerCount);
            //var head = 0;
            //while (head < capacity - 1)
            //{
            //    var begin = head;
            //    while (head < begin + bigOffset && head < capacity - 1)
            //    {
            //        head += offset;
            //        if (head > capacity - 1)
            //            head = capacity - 1;

            //        _chunks.Add(new JobChunk { begin = head - offset, end = head });
            //    }

            //    offset >>= 1;
            //}
        }

        public bool MoveNext()
        {
            //_currentChunkIndex++;
            //if (_currentChunkIndex < 0 || _currentChunkIndex >= _chunks.Count)
            //    return false;
            //return true;
            return _chunks.Count > 0;
        }

        public void Reset()
        {
            if (_chunks.Count > 0)
                _chunks.Clear();
            //_currentChunkIndex = -1;
        }
    }
}
