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

                public IScene Left => throw new NotImplementedException();

                public IScene Top => throw new NotImplementedException();

                public IScene Right => throw new NotImplementedException();

                public IScene Bottom => throw new NotImplementedException();

                public IScene LeftTop => throw new NotImplementedException();

                public IScene TopRight => throw new NotImplementedException();

                public IScene RightBottom => throw new NotImplementedException();

                public IScene BottomLeft => throw new NotImplementedException();

                public Scene(int capacity)
                {
                    _dicAgent = new Dictionary<int, Agent>(capacity);
                    Capacity = capacity;
                }

                public bool TryInsert(Agent agent)
                {
                    if (Capacity > 0 && _dicAgent.Count >= Capacity)
                        return false;

                    _dicAgent[agent.Id] = agent;
                    return true;
                }

                public bool Remove(int clientId)
                {
                    return _dicAgent.Remove(clientId);
                }

                public void Dispose()
                {
                    _dicAgent.Clear();
                }

                public override string ToString()
                {
                    return $"capacity: {Capacity.ToString()}, count: {Count.ToString()}";
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
                throw new NotImplementedException();
            }

            public int GetHashCode(SceneHandle obj)
            {
                throw new NotImplementedException();
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
        
        public IScene FindScene(int clientId)
        {
            if (_dicAgent2Scene == null)
                Library.ThrowHelper.ThrowWrongState("Please call install first.");

            if (!_dicAgent2Scene.TryGetValue(clientId, out IScene scene))
                return null;
            return scene;
        }

        public void EnterScene(SceneHandle sceneHandle, Agent agent)
        {
            if (_dicScene == null)
                Library.ThrowHelper.ThrowWrongState("Please call install first.");

            if (!_dicScene.TryGetValue(sceneHandle, out IScene scene))
            {
                scene = _factory.GetScene();
                _dicScene.Add(sceneHandle, scene);

                scene.TryInsert(agent);
            }
            else
            {
                if (!scene.TryInsert(agent))
                {
                    _factory.PutScene(scene);

                    var newScene = _factory.GetScene(scene.Capacity + 2);
                    newScene.TryInsert(agent);
                    _dicScene[sceneHandle] = newScene;
                }
            }

            _dicAgent2Scene[agent.Id] = scene;
        }

        public bool LeaveScene(int agentId)
        {
            if (_dicAgent2Scene == null)
                Library.ThrowHelper.ThrowWrongState("Please call install first.");

            if (!_dicAgent2Scene.TryGetValue(agentId, out IScene scene))
                return false;

            scene.Remove(agentId);
            return _dicAgent2Scene.Remove(agentId);
        }
    }
}
