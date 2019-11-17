using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal sealed class AgentCollection : IEnumerable<User>
    {
        private readonly Dictionary<int, User> _dicUser;
        private readonly Dictionary<int, Dummy> _dicDummy;

        public AgentCollection() : this(0, 0)
        {
        }

        public AgentCollection(int userCapacity, int dummyCapacity)
        {
            _dicUser = new Dictionary<int, User>(userCapacity);
            _dicDummy = new Dictionary<int, Dummy>(dummyCapacity);
        }

        public bool Contains(int id)
        {
            if (id < 0)
                return _dicDummy.ContainsKey(id);
            return _dicUser.ContainsKey(id);
        }

        public User GetUser(int id)
        {
            if (!_dicUser.TryGetValue(id, out User user))
                return null;
            return user;
        }

        public IAgent Get(int id)
        {
            IAgent agent;
            if (id < 0)
            {
                if (!_dicDummy.TryGetValue(id, out Dummy dummy))
                    return null;
                agent = dummy;
            }
            else
            {
                if (!_dicUser.TryGetValue(id, out User user))
                    return null;
                agent = user;
            }

            return agent;
        }
        
        public void Insert(IAgent agent)
        {
            var id = agent.Id;
            if (Contains(id))
                Library.ThrowHelper.ThrowWrongArgument($"Duplicate agent id in collection: {id.ToString()}");

            if (id < 0)
                _dicDummy.Add(id, agent as Dummy);
            _dicUser.Add(id, agent as User);
        }

        public bool Remove(IAgent agent)
        {
            var id = agent.Id;
            if (id < 0)
                return _dicDummy.Remove(id);
            return _dicUser.Remove(id);
        }

        public Dictionary<int, User>.ValueCollection.Enumerator GetUserEnumerator()
        {
            return _dicUser.Values.GetEnumerator();
        }

        IEnumerator<User> IEnumerable<User>.GetEnumerator()
        {
            return GetUserEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetUserEnumerator();
        }
    }
}
