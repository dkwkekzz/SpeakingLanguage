using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public abstract class PoolingObject<T> where T : PoolingObject<T>, new()
    {
        public static ObjectPool<T> Factory { get; } = new ObjectPool<T>();

        public static void Initialize(int capacity) => Factory.Initialize(capacity, null, null);
        public static void Initialize(int capacity, Func<T> objectGenerator) => Factory.Initialize(capacity, objectGenerator, null);
        public static void Initialize(int capacity, Func<T> objectGenerator, Action<T> objectDisposer) => Factory.Initialize(capacity, objectGenerator, objectDisposer);
        public static T Create() => Factory.GetObject();
        public static void Destroy(T obj)
        {
            obj.OnRelease();
            Factory.PutObject(obj);
        }
        
        protected virtual void OnRelease() { }
    }
}
