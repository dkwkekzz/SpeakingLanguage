using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal class SceneCollection
    {
        private class _sceneComparer : IEqualityComparer<SceneHandle>
        {
            public bool Equals(SceneHandle x, SceneHandle y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(SceneHandle obj)
            {
                return obj.xValue ^ (obj.yValue << 16);
            }
        }

        private Dictionary<SceneHandle, IScene> _dicScene;
        private Scene.Factory _factory;

        public SceneCollection() : this(0)
        {
        }

        public SceneCollection(int capacity)
        {
            _dicScene = new Dictionary<SceneHandle, IScene>(capacity, new _sceneComparer());
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

        public bool Remove(SceneHandle sceneHandle)
        {
            return _dicScene.Remove(sceneHandle);
        }

        public IScene ExpandScene(IScene scene)
        {
            var newScene = _factory.GetScene(scene.Capacity + 2);
            scene.MoveTo(newScene);
            _factory.PutScene(scene);
            return newScene;
        }
    }
}
