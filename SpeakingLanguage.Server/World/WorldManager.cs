using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal class WorldManager
    {
        private class SceneFactory
        {
            private class Scene : IScene
            {
                private readonly Dictionary<int, Agent> _dicAgent;

                public int Capacity { get; }
                public int Count => _dicAgent.Count;
                
                public Scene(int capacity)
                {
                    _dicAgent = new Dictionary<int, Agent>(capacity);
                    Capacity = capacity;
                }

                public bool TryInsert(ref Agent agent)
                {
                    if (Capacity > 0 && _dicAgent.Count >= Capacity)
                        return false;

                    _dicAgent[agent.Id] = agent;
                    return true;
                }

                public bool Remove(int id)
                {
                    return _dicAgent.Remove(id);
                }

                public void Dispose()
                {
                    _dicAgent.Clear();
                }

                public override string ToString()
                {
                    return $"capacity: {Capacity.ToString()}, count: {Count.ToString()}";
                }

                public void MoveTo(IScene dest)
                {
                    var iter = _dicAgent.Values.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        var agent = iter.Current;
                        dest.TryInsert(ref agent);
                    }
                }

                public Dictionary<int, Agent>.ValueCollection.Enumerator GetEnumerator()
                {
                    return _dicAgent.Values.GetEnumerator();
                }
            }

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

        private class SceneComparer : IEqualityComparer<SceneHandle>
        {
            public bool Equals(SceneHandle x, SceneHandle y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(SceneHandle obj)
            {
                return obj.sceneX ^ (obj.sceneY << 16);
            }
        }
        
        private Dictionary<SceneHandle, IScene> _dicScene;
        private Dictionary<int, IScene> _dicAgent2Scene;    
        private SceneFactory _factory;

        public WorldManager(ref Logic.StartInfo info)
        {
            _dicScene = new Dictionary<SceneHandle, IScene>(info.default_scenecount, new SceneComparer());
            _dicAgent2Scene = new Dictionary<int, IScene>(info.default_agentcount);
            _factory = new SceneFactory();
        }
        
        public IScene FindScene(int agentId)
        {
            if (!_dicAgent2Scene.TryGetValue(agentId, out IScene scene))
                return null;
            return scene;
        }

        public IScene GetScene(SceneHandle sceneHandle)
        {
            if (!_dicScene.TryGetValue(sceneHandle, out IScene scene))
            {
                scene = _factory.GetScene();
                _dicScene.Add(sceneHandle, scene);
            }

            return scene;
        }

        public IScene SubscribeScene(ref Agent agent, SceneHandle sceneHandle)
        {
            if (!_dicScene.TryGetValue(sceneHandle, out IScene scene))
            {
                scene = _factory.GetScene();
                _dicScene.Add(sceneHandle, scene);
            }

            if (!scene.TryInsert(ref agent))
            {
                var newScene = _factory.GetScene(scene.Capacity + 2);
                scene.MoveTo(newScene);
                _factory.PutScene(scene);

                newScene.TryInsert(ref agent);
                _dicScene[sceneHandle] = newScene;

                return newScene;
            }

            return scene;
        }

        public IScene UnsubscribeScene(int agentId, SceneHandle sceneHandle)
        {
            if (!_dicScene.TryGetValue(sceneHandle, out IScene scene))
                return null;

            var exist = scene.Remove(agentId);
            if (!exist)
                return null;

            return scene;
        }

        public void EnterScene(ref Agent agent, IScene scene)
        {
            _dicAgent2Scene[agent.Id] = scene;
        }

        public bool LeaveScene(int agentId)
        {
            if (!_dicAgent2Scene.TryGetValue(agentId, out IScene scene))
                return false;

            return _dicAgent2Scene.Remove(agentId);
        }
    }
}
