using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal sealed class User : ISubscriber, IDisposable
    {
        public class Factory
        {
            private readonly Queue<User> _pool = new Queue<User>();

            public Factory() : this(0)
            {
            }

            public Factory(int capacity)
            {
                _pool = new Queue<User>(capacity);
                for (int i = 0; i != capacity; i++)
                    _pool.Enqueue(new User());
            }

            public User Create(int id)
            {
                User agent;
                if (_pool.Count == 0)
                    agent = new User();
                else
                    agent = _pool.Dequeue();

                agent.Id = id;
                return agent;
            }

            public void Destroy(User agent)
            {
                agent.Dispose();
                _pool.Enqueue(agent);
            }
        }
        
        private readonly List<IScene> _subscribeScenes = new List<IScene>(4);
        private NetPeer _peer;

        // ISubscriber
        public int Id { get; private set; }
        public Logic.slObjectHandle SubjectHandle { get; private set; }
        public NetDataWriter DataWriter { get; private set; }
        
        private User()
        {
        }

        public void Dispose()
        {
            _subscribeScenes.Clear();
            Id = -1;
        }

        public void CapturePeer(NetPeer peer)
        {
            _peer = peer;
            if (null == DataWriter)
                DataWriter = new NetDataWriter();
            else
                DataWriter.Reset();
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

        public void FlushData()
        {
            _peer.Send(DataWriter, DeliveryMethod.ReliableOrdered);
            DataWriter.Reset();
        }
    }
}
