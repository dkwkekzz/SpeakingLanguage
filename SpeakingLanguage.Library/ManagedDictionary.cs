using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpeakingLanguage.Library
{
    public class ManagedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dic = new Dictionary<TKey, TValue>();

        public TValue this[TKey key] { get => _dic[key]; set => _dic[key] = value; }

        public ICollection<TKey> Keys => _dic.Keys;

        public ICollection<TValue> Values => _dic.Values;

        public int Count => _dic.Count;

        public bool IsReadOnly => false;

        public ManagedDictionary() { }
        public ManagedDictionary(int capacity) { }
        public ManagedDictionary(IEqualityComparer<TKey> comparer) { }
        public ManagedDictionary(IDictionary<TKey, TValue> dictionary) { }
        public ManagedDictionary(int capacity, IEqualityComparer<TKey> comparer) { }

        public void Add(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
