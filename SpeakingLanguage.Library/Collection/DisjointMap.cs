using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library.Collection
{
    public struct DisjointMap<TKey> where TKey : IEquatable<TKey>
    {
        private struct Value
        {
            public TKey parent;
            public int rank;
            public int size;
            
            public Value(TKey i)
            {
                parent = i;
                rank = 1;
                size = 1;
            }
        }

        private readonly Dictionary<TKey, Value> _dicValue;
        
        public DisjointMap(int n)
        {
            _dicValue = new Dictionary<TKey, Value>(n);
        }

        private Value Find(TKey key)
        {
            var val = _dicValue[key];
            if (val.parent.Equals(key))
                return val;
            return _dicValue[key] = Find(val.parent);
        }

        public void Merge(TKey key1, TKey key2)
        {
            var val1 = Find(key1);
            var val2 = Find(key2);
            if (val1.parent.Equals(val2.parent))
                return;
            if (val1.rank > val2.rank)
            {
                var temp = val1;
                val1 = val2;
                val2 = temp;
                
                var tempKey = key1;
                key1 = key2;
                key2 = tempKey;
            }

            val1.parent = key2;
            val2.size += val1.size;
            if (val1.rank == val2.rank) val2.rank++;

            _dicValue[key1] = val1;
            _dicValue[key2] = val2;
        }

        public void Reset()
        {
            _dicValue.Clear();
        }

        public void Divide(int u, int v)
        {
        }
    }
}
