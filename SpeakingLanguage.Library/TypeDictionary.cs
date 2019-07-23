using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpeakingLanguage.Library
{
    public class TypeDictionary<TValue> : IEnumerable<KeyValuePair<Type, TValue>>
    {
        private Dictionary<Type, TValue> _typeDic;
        
        public TypeDictionary(int capacity = 0)
        {
            _typeDic = new Dictionary<Type, TValue>(capacity);
        }

        public TypeDictionary(IEqualityComparer<Type> comparer, int capacity = 0)
        {
            _typeDic = new Dictionary<Type, TValue>(capacity, comparer);
        }

        public TResult Get<TResult>()
            where TResult : class, TValue
        {
            return _typeDic[typeof(TResult)] as TResult;
        }

        public bool Contains(Type key)
        {
            return _typeDic.ContainsKey(key);
        }

        public bool TryGetValue<TResult>(Type key, out TResult value)
            where TResult : class
        {
            TValue orgValue;
            if (!_typeDic.TryGetValue(key, out orgValue))
            {
                value = default(TResult);
                return false;
            }

            value = orgValue as TResult;
            return true;
        }
        
        public void Add<TResult>(TResult value)
            where TResult : class, TValue
        {
            _typeDic.Add(typeof(TResult), value);
        }

        public void Add<TResult>(Type type, TResult value)
            where TResult : class, TValue
        {
            _typeDic.Add(type, value);
        }

        public void Remove(Type key)
        {
            _typeDic.Remove(key);
        }

        public void Clear()
        {
            _typeDic.Clear();
        }

        public IEnumerator<KeyValuePair<Type, TValue>> GetEnumerator()
        {
            return _typeDic.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
