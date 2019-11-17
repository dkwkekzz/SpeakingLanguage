using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    public static class Update
    {
        public static void Execute()
        {
            var sceneCol = World.Instance.SceneCollection;
            var agentCol = World.Instance.AgentCollection;
            var agentIter = agentCol.GetEnumerator();
            while (agentIter.MoveNext())
            {
                var agent = agentIter.Current;
                var select = Logic.PropertyHelper.GetReadonly<Logic.Property.Selector>(agent.LogicObject);
                var pos = Logic.PropertyHelper.GetReadonly<Logic.Property.Position>(select.handle);
                var selectSceneIdx = (pos.x >> 10) | ((pos.y >> 9) << 8) | (pos.z << 16) | (pos.w << 20);
                if (!sceneCol.TryGetScene(selectSceneIdx, out Scene scene)) 
                    continue;

                if (agent.CurrentScene != scene)
                {
                    scene.AddSubscriber(agent);
                    agent.CurrentScene = scene;
                }
            }

            var sceneIter = sceneCol.GetEnumerator();
            while (sceneIter.MoveNext())
            {
                sceneIter.Current.Notify();
            }
        }
    }
}
