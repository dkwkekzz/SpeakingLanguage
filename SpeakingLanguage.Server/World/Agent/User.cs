using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal sealed class User : ISubscriber, IDisposable, ISerializable
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

        private struct Information
        {
            public string id;
            public string pswd;
            public long createdTicks;
            //public Dictionary<Logic.slObjectHandle, long> subjects;
        }

        private readonly List<IScene> _subscribeScenes = new List<IScene>(4);
        private NetPeer _peer;
        private Information _info;

        public bool Authentication => _info.createdTicks != 0;

        // ISubscriber
        public int Id { get; private set; }
        public Logic.slObjectHandle SubjectHandle => SubjectSelector.Current;
        public NetDataWriter DataWriter { get; private set; }

        public SubjectSelector SubjectSelector { get; } = new SubjectSelector();

        private User()
        {
            //_info.subjects = new Dictionary<Logic.slObjectHandle, long>(8);
        }

        public void Dispose()
        {
            _subscribeScenes.Clear();
            Id = -1;
        }

        public void Save(Networks.IDatabase database)
        {
            if (!Authentication) return;

            database.RequestWriteUser(this, $"user_{_info.id}_{_info.pswd}");
        }
        
        public void OnDeserialize(ref Library.Reader reader)
        {
            if (reader.LengthToRead == 0)
            {
                DataWriter.Put((int)Protocol.Code.Packet.DeserializeUser);
                DataWriter.Put(new Protocol.Packet.SerializeResultData { success = false });
                return;
            }

            var read = true;
            read &= reader.ReadString(out _info.id);
            read &= reader.ReadString(out _info.pswd);
            read &= reader.ReadLong(out _info.createdTicks);
            read &= reader.ReadInt(out int length);
            if (!read) Library.ThrowHelper.ThrowFailToConvert($"[User::Construct::reader] position:{reader.Position.ToString()}");

            SubjectSelector.OnDeserialize(ref reader);

            DataWriter.Put((int)Protocol.Code.Packet.DeserializeUser);
            DataWriter.Put(new Protocol.Packet.SerializeResultData { success = true });
            return;
        }

        public void OnSerialize(ref Library.Writer writer)
        {
            writer.WriteString(_info.id);
            writer.WriteString(_info.pswd);
            writer.WriteLong(_info.createdTicks);

            SubjectSelector.OnSerialize(ref writer);

            DataWriter.Put((int)Protocol.Code.Packet.SerializeUser);
            DataWriter.Put(new Protocol.Packet.SerializeResultData { success = true });
        }

        public void CapturePeer(NetPeer peer)
        {
            _peer = peer;
            if (null == DataWriter)
                DataWriter = new NetDataWriter();
            else
                DataWriter.Reset();
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

        public void FlushNetData()
        {
            _peer.Send(DataWriter, DeliveryMethod.ReliableOrdered);
            DataWriter.Reset();
        }
    }
}
