using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
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
        
        public Agent CreateEmpty(int id)
        {
            if (_dicAgent.ContainsKey(id))
                return null;

            var agent = _factory.GetAgent();
            _dicAgent.Add(id, agent);
            return agent;
        }

        public bool Insert(Agent agent)
        {
            var id = agent.Id;
            if (_dicAgent.ContainsKey(id))
                return false;

            _dicAgent.Add(id, agent);
            return true;
        }

        public bool Remove(Agent agent)
        {
            _factory.PutAgent(agent);
            return _dicAgent.Remove(id);
        }
    }
}
