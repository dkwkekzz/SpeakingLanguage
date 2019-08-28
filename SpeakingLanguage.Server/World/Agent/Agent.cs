using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal class Agent : ISubscriber, IDisposable
    {
        public class Factory
        {
            private Queue<Agent> _pool = new Queue<Agent>();

            public Factory() : this(0)
            {
            }

            public Factory(int capacity)
            {
                for (int i = 0; i != capacity; i++)
                    _pool.Enqueue(new Agent());
            }

            public Agent GetAgent()
            {
                if (_pool.Count == 0)
                    return new Agent();

                return _pool.Dequeue();
            }

            public void PutAgent(Agent agent)
            {
                agent.Dispose();
                _pool.Enqueue(agent);
            }
        }

        public enum ESort
        {
            NPC = 0,
            PC,
        }

        private readonly List<IScene> _subscribeScenes = new List<IScene>(4);

        // ISubscriber
        public int Id { get; private set; }
        public Logic.slObjectHandle SubjectHandle { get; private set; }
        
        public NetPeer Peer { get; private set; }
        public ESort Sort => Peer != null ? ESort.PC : ESort.NPC;

        private Agent()
        {
        }

        public void ConstructUser(NetPeer peer)
        {
            _subscribeScenes.Clear();
            SubjectHandle = 0;
            Id = peer.Id;
            Peer = peer;
        }

        public void Dispose()
        {
            ClearSubscribe();
        }

        public Logic.slObjectHandle CaptureSubject(int subjectValue)
        {
            var lastSubject = SubjectHandle;
            SubjectHandle = new Logic.slObjectHandle { value = subjectValue };
            return lastSubject;
        }
        
        public List<IScene>.Enumerator GetSceneEnumerator()
        {
            return _subscribeScenes.GetEnumerator();
        }

        public IScene SubscribeScene(IScene scene)
        {
            if (_subscribeScenes.Count < 4)
            {
                _subscribeScenes.Add(scene);
                return scene;
            }

            for (int i = 0; i != _subscribeScenes.Count; i++)
            {
                if (null == _subscribeScenes[i])
                {
                    _subscribeScenes[i] = scene;
                    return scene;
                }
            }
            return null;
        }

        public bool UnsubscribeScene(IScene scene)
        {
            for (int i = 0; i != _subscribeScenes.Count; i++)
            {
                if (scene == _subscribeScenes[i])
                {
                    _subscribeScenes[i] = null;
                    return true;
                }
            }
            return false;
        }

        public void ClearSubscribe()
        {
            _subscribeScenes.Clear();
        }

        public void Push(NetDataWriter writer)
        {
            if (Peer != null)
            {
                Peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }
    }
}
