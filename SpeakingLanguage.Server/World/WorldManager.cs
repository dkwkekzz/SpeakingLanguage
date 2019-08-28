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
        
        public SceneCollection Scenes { get; private set; }
        public AgentCollection Agents { get; private set; }
        public ColliderCollection Colliders { get; private set; }
        public LogicExecutor Executor { get; private set; }

        private Logic.Service _logicService;
        public ref Logic.Service Service => ref _logicService;

        private WorldManager()
        {
        }
        
        public void Install(ref Logic.StartInfo info)
        {
            _logicService = new Logic.Service(ref info);

            Scenes = new SceneCollection(info.default_scenecount);
            Agents = new AgentCollection(info.default_agentcount);
            Colliders = new ColliderCollection(info.default_agentcount);
            Executor = new LogicExecutor();
        }
    }
}
