using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal sealed class World : Library.SingletonLazy<World>
    {
        public SceneCollection SceneCollection { get; } = new SceneCollection();
        public KeyGenerator KeyGenerator { get; } = new KeyGenerator();
        public AgentCollection AgentCollection { get; } = new AgentCollection(); 
    }
}
