using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace SpeakingLanguage.Library
{
    public class KeyObjectPool<TKey, TValue>
    {
        private ConcurrentDictionary<TKey, TValue> _objects;
        private Func<TKey, TValue> _objectGenerator;
        private Action<TValue> _objectDisposer;

        public int Capacity => _objects.Count;

        public KeyObjectPool(Func<TKey, TValue> objectGenerator, Action<TValue> objectDisposer = null)
        {
            if (objectGenerator == null)
                throw new ArgumentNullException("objectGenerator");

            _objects = new ConcurrentDictionary<TKey, TValue>();
            _objectGenerator = objectGenerator;
            _objectDisposer = objectDisposer;
        }
        
        public TValue GetObject(TKey key)
        {
            TValue item;
            if (_objects.TryGetValue(key, out item))
                return item;
            
            return _objectGenerator(key);
        }

        public void PutObject(TKey key, TValue item)
        {
            _objectDisposer(item);
            _objects.TryAdd(key, item);
        }
    }
}
