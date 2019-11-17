using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal class AccountTable
    {
        private struct KeyTable
        {
            public Dictionary<string, object> value;
        }

        private Dictionary<int, KeyTable> _accTableDic = new Dictionary<int, KeyTable>();

        public Dictionary<TKey, TValue> GetTable<TKey, TValue>(int id, string key)
        {
            if (!_accTableDic.TryGetValue(id, out KeyTable table))
            {
                table = new KeyTable { value = new Dictionary<string, object>() };
                _accTableDic.Add(id, table);
            }

            var keyDic = table.value;
            if (!keyDic.TryGetValue(key, out object value))
            {
                value = new Dictionary<TKey, TValue>();
                keyDic.Add(key, value);
            }

            return value as Dictionary<TKey, TValue>;
        }

        public void Remove(int id) => _accTableDic.Remove(id);
    }

    internal class AgentCollection
    {
        private Dictionary<int, Agent> _agentDic = new Dictionary<int, Agent>();

        public Dictionary<int, Agent>.ValueCollection.Enumerator GetEnumerator() => _agentDic.Values.GetEnumerator();

        public int Count => _agentDic.Count;

        public void Insert(NetPeer peer)
        {
            _agentDic.Add(peer.Id, new Agent(peer));
        }

        public bool Contains(int id)
        {
            return _agentDic.ContainsKey(id);
        }

        public bool TryGetAgent(int id, out Agent agent)
        {
            return _agentDic.TryGetValue(id, out agent);
        }

        public void Remove(int id)
        {
            _agentDic.Remove(id);
        }
    }

    internal class SceneCollection
    {
        private Dictionary<int, Scene> _sceneDic = new Dictionary<int, Scene>();

        public Dictionary<int, Scene>.ValueCollection.Enumerator GetEnumerator() => _sceneDic.Values.GetEnumerator();

        public void Insert(Scene scene)
        {
            _sceneDic.Add(scene.Index, scene);
        }

        public bool TryGetScene(int idx, out Scene scene)
        {
            return _sceneDic.TryGetValue(idx, out scene);
        }
    }
}
