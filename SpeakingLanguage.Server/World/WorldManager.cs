using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal partial class WorldManager
    {
        private static readonly Lazy<WorldManager> lazy = new Lazy<WorldManager>(() => new WorldManager());

        public static WorldManager Locator => lazy.Value;
        public static bool IsCreated => lazy.IsValueCreated;

        //private Dictionary<SceneHandle, IScene> _dicScene;
        //private Dictionary<int, IScene> _dicAgent2Scene;

        public SceneCollection Scenes { get; private set; }
        public AgentCollection Agents { get; private set; }
        public LogicExecutor Executor { get; private set; }

        private Logic.Service _logicService;
        public ref Logic.Service Service => ref _logicService;

        private WorldManager()
        {
        }
        
        public void Install(ref Logic.StartInfo info)
        {
            //_dicScene = new Dictionary<SceneHandle, IScene>(info.default_scenecount, new SceneComparer());
            //_dicAgent2Scene = new Dictionary<int, IScene>(info.default_agentcount);
            _logicService = new Logic.Service(ref info);

            Scenes = new SceneCollection(info.default_scenecount);
            Agents = new AgentCollection(info.default_agentcount);
            Executor = new LogicExecutor();
        }
        
        //public void EnterWorld(int agentId, IScene scene)
        //{
        //    _dicAgent[agentId] = _agentFactory.GetAgent()
        //}
        //public IScene FindAgent(int agentId)
        //{
        //    if (!_dicAgent.TryGetValue(agentId, out Agent agent))
        //        return null;
        //    return scene;
        //}
        //
        //public IScene FindScene(SceneHandle sceneHandle)
        //{
        //    if (!_dicScene.TryGetValue(sceneHandle, out IScene scene))
        //    {
        //        scene = _factory.GetScene();
        //        _dicScene.Add(sceneHandle, scene);
        //    }
        //
        //    return scene;
        //}
        //
        //public IScene SubscribeScene(ISubscriber subscriber, SceneHandle sceneHandle)
        //{
        //    if (!scene.TryInsert(ref agent))
        //    {
        //        var newScene = _factory.GetScene(scene.Capacity + 2);
        //        scene.MoveTo(newScene);
        //        _factory.PutScene(scene);
        //
        //        newScene.TryInsert(ref agent);
        //        _dicScene[sceneHandle] = newScene;
        //
        //        return newScene;
        //    }
        //
        //    return scene;
        //}
        //
        //public IScene UnsubscribeScene(int agentId, SceneHandle sceneHandle)
        //{
        //    if (!_dicScene.TryGetValue(sceneHandle, out IScene scene))
        //        return null;
        //
        //    var exist = scene.Remove(agentId);
        //    if (!exist)
        //        return null;
        //
        //    return scene;
        //}
        //
        //public void EnterScene(Agent agent, IScene scene)
        //{
        //    _dicAgent2Scene[agent.Id] = scene;
        //}
        //
        //public bool LeaveScene(int agentId)
        //{
        //    if (!_dicAgent2Scene.TryGetValue(agentId, out IScene scene))
        //        return false;
        //
        //    return _dicAgent2Scene.Remove(agentId);
        //}
    }
}
