using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace SpeakingLanguage.Library
{
    public interface IObjectPool<TValue>
    {
        TValue GetObject();
        void PutObject(TValue item);
    }

    public class ObjectPool<TValue> : IObjectPool<TValue>
        where TValue : class, new()
    {
        private int _generateCounter = 1 << 16;
        public int CurrentIndex => _generateCounter;
        public int GenerateIndex => Interlocked.Increment(ref _generateCounter);

        private ConcurrentBag<TValue> _objects = new ConcurrentBag<TValue>();
        private Func<TValue> _objectGenerator;
        private Action<TValue> _objectDisposer;

        public int Capacity => _objects.Count;

        public ObjectPool(int capacity = 0, Func<TValue> objectGenerator = null, Action<TValue> objectDisposer = null)
        {
            Initialize(capacity, objectGenerator, objectDisposer);
        }
        
        public void Initialize(int capacity, Func<TValue> objectGenerator, Action<TValue> objectDisposer)
        {
            _objectGenerator = objectGenerator;
            _objectDisposer = objectDisposer;

            for (int i = _objects.Count; i != capacity; i++)
                this.PutObject(objectGenerator());
        }
        
        public TValue GetObject()
        {
               TValue item;
            if (_objects.TryTake(out item))
                return item;
            
            return _objectGenerator();
        }

        public void PutObject(TValue item)
        {
            _objectDisposer?.Invoke(item);
            _objects.Add(item);
        }
    }
}
