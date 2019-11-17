using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal sealed class WorldManager : Library.SingletonLazy<WorldManager>
    {
        public SceneCollection Scenes { get; private set; }
        public ColliderCollection Colliders { get; private set; }
        public AgentCollection Agents { get; private set; }
        public User.Factory UserFactory { get; private set; }
        
        public void Install(ref World.StartInfo info)
        {
            Scenes = new SceneCollection(info.default_scenecount);
            Colliders = new ColliderCollection(info.default_usercount);
            Agents = new AgentCollection(info.default_usercount, info.default_dummycount);
            UserFactory = new User.Factory(info.default_usercount);
        }

        public void FlushEvents()
        {
            var userIter = Agents.GetUserEnumerator();
            while (userIter.MoveNext())
            {
                var user = userIter.Current;
                user.FlushNetData();
            }
        }
    }
}
