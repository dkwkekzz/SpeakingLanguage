using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    public static class Process
    {
        public static void Start()
        {
            var agentCol = World.Instance.AgentCollection;
            var agentIter = agentCol.GetEnumerator();
            while (agentIter.MoveNext())
            {
                var agent = agentIter.Current;
                if (!agent.IsAlive) continue;

                agent.LogicUpdate();
            }
        }

        public static void Update()
        {
            var sceneCol = World.Instance.SceneCollection;
            var agentCol = World.Instance.AgentCollection;
            var agentIter = agentCol.GetEnumerator();
            while (agentIter.MoveNext())
            {
                var agent = agentIter.Current;
                if (!agent.IsAlive) continue;

                var pos = agent.LogicPosition;
                var selectSceneIdx = (pos.x >> 10) | ((pos.y >> 9) << 8) | (pos.z << 16) | (pos.w << 20);
                if (!sceneCol.TryGetScene(selectSceneIdx, out Scene scene)) 
                    continue;

                scene.AddVisitor(agent.LogicController.handle);

                var lastScene = agent.CurrentScene;
                if (lastScene != scene)
                {
                    lastScene.CancelSubscriber(agent);
                    scene.ReserveSubscriber(agent);
                    agent.CurrentScene = scene;
                }
            }

            var writer = World.Instance.WriteHolder.NewWriter;
            var sceneIter = sceneCol.GetEnumerator();
            while (sceneIter.MoveNext())
            {
                var scene = sceneIter.Current;
                
                var cntVisitor = scene.VisitorCount;
                if (cntVisitor != 0)
                {
                    writer.WriteInt((int)Protocol.Code.Packet.Synchronization);
                    writer.WriteInt(cntVisitor);

                    var visIter = scene.Visitors;
                    while (visIter.MoveNext())
                    {
                        Logic.Synchronization.Serialize(visIter.Current.handle, ref writer);
                    }

                    var resIter = scene.Reservers;
                    while (resIter.MoveNext())
                    {
                        if (!resIter.Current.TryGetTarget(out Agent agent))
                            continue;

                        agent.Send(writer.Buffer, 0, writer.Offset);
                        scene.AddSubscriber(agent.Id, resIter.Current);
                    }
                }

                var lenData = scene.DataWriter.Length;
                if (lenData > 0)
                {
                    unsafe
                    {
                        var checkIds = stackalloc int[scene.SubscriberCount];
                        var checkCnt = 0;

                        var subIter = scene.Subscribers;
                        while (subIter.MoveNext())
                        {
                            if (!subIter.Current.TryGetTarget(out Agent agent))
                            {
                                checkIds[checkCnt++] = agent.Id;
                                continue;
                            }

                            agent.Send(scene.DataWriter.Data, 0, lenData);
                        }

                        for (int i = 0; i != checkCnt; i++)
                        {
                            scene.RemoveSubscriber(checkIds[i]);
                        }
                    }
                    scene.DataWriter.Reset();
                    scene.DataWriter.Put((int)Protocol.Code.Packet.Subscribe);
                }
                scene.Reset();
            }
        }
    }
}
