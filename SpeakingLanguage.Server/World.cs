using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal sealed class World : Library.SingletonLazy<World>
    {
        public WriteHolder WriteHolder { get; } = new WriteHolder();
        public KeyGenerator KeyGenerator { get; } = new KeyGenerator();
        public SceneCollection SceneCollection { get; } = new SceneCollection();
        public AgentCollection AgentCollection { get; } = new AgentCollection(); 
    }
}
