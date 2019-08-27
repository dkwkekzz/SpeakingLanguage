using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal interface IAgent
    {

    }

    internal interface ISubscriber
    {
        int Id { get; }
        void Read(NetDataWriter writer);
        Logic.slObjectHandle SubjectHandle { get; }
    }

    internal class AgentCollection
    {
        private Dictionary<int, Agent> _dicAgent;
        private Agent.Factory _factory;
        
        public AgentCollection() : this(0)
        {
        }

        public AgentCollection(int capacity)
        {
            _dicAgent = new Dictionary<int, Agent>(capacity);
            _factory = new Agent.Factory();
        }

        public bool Contains(int id)
        {
            return _dicAgent.ContainsKey(id);
        }

        public Agent Get(int id)
        {
            if (!_dicAgent.TryGetValue(id, out Agent agent))
                return null;

            return agent;
        }

        public Agent Insert(int id)
        {
            if (_dicAgent.ContainsKey(id))
                return null;

            var agent = _factory.GetAgent(id);
            _dicAgent.Add(id, agent);
            return agent;
        }

        public bool Remove(int id)
        {
            return _dicAgent.Remove(id);
        }
    }

    internal class Agent : ISubscriber
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
                    _pool.Enqueue(new Agent(i));
            }

            public Agent GetAgent(int id)
            {
                if (_pool.Count == 0)
                    return new Agent(id);

                return _pool.Dequeue();
            }

            public void PutAgent(Agent agent)
            {
                _pool.Enqueue(agent);
            }
        }

        public enum ESort
        {
            NPC = 0,
            PC,
        }
        
        public ESort Sort => Peer != null ? ESort.PC : ESort.NPC;

        public int Id { get; }
        public IScene CurrentScene { get; set; }
        public Logic.slObjectHandle SubjectHandle { get; set; }

        public NetPeer Peer { get; private set; }

        public Agent(int id)
        {
            Id = id;
        }

        private Agent(NetPeer peer, int objectHandleValue)
        {
            Peer = peer;
            Id = Peer.Id;
            SubjectHandle = objectHandleValue;
        }

        public void Read(NetDataWriter writer)
        {
            if (Peer != null)
            {
                Peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }
    }
}
