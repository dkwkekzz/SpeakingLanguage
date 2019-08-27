using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal class SceneCollection
    {
        private Dictionary<SceneHandle, IScene> _dicScene;
        private Scene.Factory _factory;

        public SceneCollection() : this(0)
        {
        }

        public SceneCollection(int capacity)
        {
            _dicScene = new Dictionary<SceneHandle, IScene>(capacity, new SceneComparer());
            _factory = new Scene.Factory();
        }
        
        public IScene Get(SceneHandle sceneHandle)
        {
            if (!_dicScene.TryGetValue(sceneHandle, out IScene scene))
            {
                scene = _factory.GetScene();
                _dicScene.Add(sceneHandle, scene);
            }
        
            return scene;
        }
        
        public IScene SubscribeScene(ISubscriber subscriber, SceneHandle sceneHandle)
        {
            var scene = Get(sceneHandle);
            if (!scene.TryInsert(subscriber))
            {
                var newScene = _factory.GetScene(scene.Capacity + 2);
                scene.MoveTo(newScene);
                _factory.PutScene(scene);
        
                newScene.TryInsert(subscriber);
                _dicScene[sceneHandle] = newScene;
        
                return newScene;
            }
        
            return scene;
        }
        
        public IScene UnsubscribeScene(int id, SceneHandle sceneHandle)
        {
            if (!_dicScene.TryGetValue(sceneHandle, out IScene scene))
                return null;
        
            var exist = scene.Remove(id);
            if (!exist)
                return null;
        
            return scene;
        }
    }

    internal class Scene : IScene
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

        public bool TryInsert(ISubscriber subscriber)
        {
            if (Capacity > 0 && _dicSubs.Count >= Capacity)
                return false;

            _dicSubs[subscriber.Id] = subscriber;
            return true;
        }

        public bool Remove(int id)
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
                dest.TryInsert(subscriber);
            }
        }

        public Dictionary<int, ISubscriber>.ValueCollection.Enumerator GetEnumerator()
        {
            return _dicSubs.Values.GetEnumerator();
        }
    }
}
