using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal class Scene : IScene, IEnumerable<ISubscriber>
    {
        public class Factory
        {
            private Queue<IScene> _pool11 = new Queue<IScene>();
            private Queue<IScene> _pool29 = new Queue<IScene>();
            private Queue<IScene> _pool59 = new Queue<IScene>();
            private Queue<IScene> _pool107 = new Queue<IScene>();
            private Queue<IScene> _poolInf = new Queue<IScene>();

            public IScene GetScene(int capacity = 11)
            {
                if (capacity <= 11)
                {
                    if (_pool11.Count > 0)
                        return _pool11.Dequeue();
                    return new Scene(11);
                }
                else if (capacity <= 29)
                {
                    if (_pool29.Count > 0)
                        return _pool29.Dequeue();
                    return new Scene(29);
                }
                else if (capacity <= 59)
                {
                    if (_pool59.Count > 0)
                        return _pool59.Dequeue();
                    return new Scene(59);
                }
                else if (capacity <= 107)
                {
                    if (_pool107.Count > 0)
                        return _pool107.Dequeue();
                    return new Scene(107);
                }

                if (_poolInf.Count > 0)
                    return _poolInf.Dequeue();
                return new Scene(-1);
            }

            public void PutScene(IScene scene)
            {
                scene.Dispose();

                var capacity = scene.Capacity;
                if (capacity < 0)
                {
                    _poolInf.Enqueue(scene);
                }
                else if (capacity <= 11)
                {
                    _pool11.Enqueue(scene);
                }
                else if (capacity <= 29)
                {
                    _pool29.Enqueue(scene);
                }
                else if (capacity <= 59)
                {
                    _pool59.Enqueue(scene);
                }
                else if (capacity <= 107)
                {
                    _pool107.Enqueue(scene);
                }
            }
        }

        private readonly Dictionary<int, ISubscriber> _dicSubs;

        public int Capacity { get; }
        public int Count => _dicSubs.Count;

        private Scene(int capacity)
        {
            _dicSubs = new Dictionary<int, ISubscriber>(capacity);
            Capacity = capacity;
        }

        public bool TryAddNotification(ISubscriber subscriber)
        {
            if (Capacity > 0 && _dicSubs.Count >= Capacity)
                return false;

            _dicSubs[subscriber.Id] = subscriber;
            return true;
        }

        public bool CancelNotification(int id)
        {
            return _dicSubs.Remove(id);
        }

        public void Dispose()
        {
            _dicSubs.Clear();
        }

        public override string ToString()
        {
            return $"capacity: {Capacity.ToString()}, count: {Count.ToString()}";
        }

        public void MoveTo(IScene dest)
        {
            var iter = _dicSubs.Values.GetEnumerator();
            while (iter.MoveNext())
            {
                var subscriber = iter.Current;
                dest.TryAddNotification(subscriber);
            }
        }
        
        public Dictionary<int, ISubscriber>.ValueCollection.Enumerator GetEnumerator()
        {
            return _dicSubs.Values.GetEnumerator();
        }

        IEnumerator<ISubscriber> IEnumerable<ISubscriber>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
