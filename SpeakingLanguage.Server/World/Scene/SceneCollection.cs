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

        //public IScene SubscribeScene(ISubscriber subscriber, SceneHandle sceneHandle)
        //{
        //    var scene = Get(sceneHandle);
        //    if (!scene.TryInsert(subscriber))
        //    {
        //        var newScene = _factory.GetScene(scene.Capacity + 2);
        //        scene.MoveTo(newScene);
        //        _factory.PutScene(scene);
        //
        //        newScene.TryInsert(subscriber);
        //        _dicScene[sceneHandle] = newScene;
        //
        //        return newScene;
        //    }
        //
        //    return scene;
        //}
        //
        //public IScene Delete(SceneHandle sceneHandle)
        //{
        //    if (!_dicScene.TryGetValue(sceneHandle, out IScene scene))
        //        return null;
        //
        //    var exist = scene.Remove(id);
        //    if (!exist)
        //        return null;
        //
        //    return scene;
        //}
    }
}
