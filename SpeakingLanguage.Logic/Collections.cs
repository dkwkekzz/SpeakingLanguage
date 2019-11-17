using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Logic
{
    /// <summary>
    /// 콜렉션의 종류
    /// 1. Dynamic: 매틱마다 갱신되는 콜렉션
    /// 2. Static: 부팅시 최초 한번 갱신되고 바뀌지 않는 콜렉션.
    /// </summary>
    public interface IDynamicCollection
    {
    }

    public interface IStaticCollection
    {
    }

    public class MessageQueue : IDynamicCollection
    {
        public struct SyncData
        {
            public Library.Reader reader;
        }

        private readonly Queue<SyncData> _syncs = new Queue<SyncData>();

        public void Push(Library.Reader reader)
        {
            _syncs.Enqueue(new SyncData { reader = reader });
        }

        public bool TryPop(out SyncData data)
        {
            if (_syncs.Count == 0)
            {
                data = default(SyncData);
                return false;
            }

            data = _syncs.Dequeue();
            return true;
        }

        public void Reset()
        {
            _syncs.Clear();
        }
    }

    public class PropertyTable : IDynamicCollection, IDisposable
    {
        public struct Row
        {
            public Archetype type;
            public IntPtr head;
        }

        private struct Chunk
        {
            public IntPtr ptr;
            public int length;
        }

        private readonly Dictionary<int, Row> _obj2RowDic = new Dictionary<int, Row>(Config.PROPTABLE_DEFAULT_HEIGHT);
        private readonly IntPtr _root;
        private IntPtr _head;
        private readonly List<Chunk> _unusedChunks = new List<Chunk>();

        public PropertyTable()
        {
            _root = Marshal.AllocHGlobal(Config.PROPTABLE_MAX_BUFFERSIZE);
            _head = _root;
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_root);
        }

        public void Insert(int handle, Archetype type, out Row row)
        {
            row = new Row();
            row.head = _head;
            row.type = type;
            _head += row.type.Capacity;
            _obj2RowDic[handle] = row;
        }

        public void Remove(int handle)
        {
            var row = _obj2RowDic[handle];
            _unusedChunks.Add(new Chunk { ptr = row.head, length = row.type.Capacity });

            _obj2RowDic.Remove(handle);
        }

        public bool TryGetRow(int handle, out Row row)
        {
            return _obj2RowDic.TryGetValue(handle, out row);
        }

        public void Compact()
        {
            var headOfs = _head.ToInt64();
            var totalCompactedLen = 0L;
            var chkIter = _unusedChunks.GetEnumerator();
            while (chkIter.MoveNext())
            {
                var chk = chkIter.Current;
                var chkOfs = chk.ptr.ToInt64() - totalCompactedLen;
                var dest = chkOfs + chk.length;
                var len = headOfs - dest;
                unsafe
                {
                    Buffer.MemoryCopy((void*)chkOfs, (void*)dest, len, len);
                }

                totalCompactedLen += len;
                _head = IntPtr.Subtract(_head, (int)len);
                headOfs = _head.ToInt64();
            }
        }
    }

    public sealed class ArchetypeCollection : IStaticCollection
    {
        private readonly List<Archetype> _types;

        public ArchetypeCollection()
        {
            _types = Library.AssemblyHelper.Collect<Archetype>();
        }

        public List<Archetype>.Enumerator GetEnumerator()
        {
            return _types.GetEnumerator();
        }

        public void Insert(Archetype type)
        {
            _types[type.Index] = type;
        }

        public bool TryGetArchetype(int idx, out Archetype type)
        {
            if (idx < 0 || _types.Count <= idx)
            {
                type = null;
                return false;
            }

            type = _types[idx];
            return true;
        }
    }

    class LawMediator : IStaticCollection
    {
        private struct TypePair
        {
            public Archetype subject;
            public Archetype target;
        }

        private class TypePairComparer : IEqualityComparer<TypePair>
        {
            public bool Equals(TypePair x, TypePair y) => object.ReferenceEquals(x.subject, y.subject) && object.ReferenceEquals(x.target, y.target);
            public int GetHashCode(TypePair obj) => obj.subject.Index ^ obj.target.Index;
        }

        private readonly Dictionary<TypePair, List<Law>> _typePair2LawListDic = new Dictionary<TypePair, List<Law>>(new TypePairComparer());

        public void Insert(Archetype subjectType, Archetype targetType, Law law)
        {
            if (!_typePair2LawListDic.TryGetValue(new TypePair { subject = subjectType, target = targetType }, out List<Law> laws))
            {
                laws = new List<Law>();
                _typePair2LawListDic.Add(new TypePair { subject = subjectType, target = targetType }, laws);
            }

            laws.Add(law);
        }

        public bool TryGetLaws(Archetype subjectType, Archetype targetType, out List<Law> laws)
        {
            return _typePair2LawListDic.TryGetValue(new TypePair { subject = subjectType, target = targetType }, out laws);
        }
    }

    class LawCollection : IStaticCollection
    {
        private readonly List<Law> _laws;

        public LawCollection()
        {
            _laws = Library.AssemblyHelper.Collect<Law>();
        }

        public List<Law>.Enumerator GetEnumerator()
        {
            return _laws.GetEnumerator();
        }

        public void Insert(Law law)
        {
            _laws.Add(law);
        }
    }

    class Graph : IDynamicCollection
    {
        public struct Enumerator : IEnumerator<slInteraction>
        {
            private readonly Dictionary<int, List<int>>.Enumerator _linksIter;
            private int _thereIdx;

            public Enumerator(Dictionary<int, List<int>> links)
            {
                _linksIter = links.GetEnumerator();
                _thereIdx = -1;
            }

            public slInteraction Current => new slInteraction(_linksIter.Current.Key, _linksIter.Current.Value[_thereIdx]);
            object IEnumerator.Current => this.Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_thereIdx < 0)
                {
                    if (!_linksIter.MoveNext()) return false;
                    _thereIdx = 0;
                }
                else
                {
                    _thereIdx++;
                }

                if (_linksIter.Current.Value.Count >= _thereIdx)
                    _thereIdx = -1;

                return true;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        private readonly Dictionary<int, List<int>> _links = new Dictionary<int, List<int>>(Config.GRAPH_DEFAULT_HEIGHT);

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_links);
        }

        public void Insert(slInteraction itr)
        {
            if (!_links.TryGetValue(itr.subject, out List<int> targetList))
            {
                targetList = new List<int>(Config.GRAPH_DEFAULT_WIDTH);
                _links.Add(itr.subject, targetList);
            }

            targetList.Add(itr.target);
        }

        public void Reset()
        {
            _links.Clear();
        }
    }
}
