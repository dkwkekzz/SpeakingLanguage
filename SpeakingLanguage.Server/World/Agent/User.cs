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
            public Dictionary<Logic.slObjectHandle, long> subjects;
        }
        
        private readonly List<IScene> _subscribeScenes = new List<IScene>(4);
        private NetPeer _peer;
        private Information _info;

        // ISubscriber
        public int Id { get; private set; }
        public Logic.slObjectHandle SubjectHandle { get; private set; }
        public NetDataWriter DataWriter { get; private set; }

        private User()
        {
            _info.subjects = new Dictionary<Logic.slObjectHandle, long>(8);
        }

        public void Dispose()
        {
            _subscribeScenes.Clear();
            Id = -1;
        }

        public void Save(Networks.IDatabase database)
        {
            if (_info.createdTicks == 0) return;

            database.RequestWriteUser(this, $"user_{_info.id}_{_info.pswd}");
        }
        
        public void DeserializeInfo(ref Library.Reader reader)
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

            for (int i = 0; i != length; i++)
            {
                read &= reader.ReadLong(out long objUid);
                var eRet = Logic.EventManager.Instance.DeserializeObject(ref reader);
                if (!eRet.Success) Library.ThrowHelper.ThrowFailToConvert("User::DeserializeInfo");

                var handleValue = eRet.result;
                _info.subjects.Add(new Logic.slObjectHandle(handleValue), objUid);
            }

            DataWriter.Put((int)Protocol.Code.Packet.DeserializeUser);
            DataWriter.Put(new Protocol.Packet.SerializeResultData { success = true });
            return;
        }

        public void SerializeInfo(ref Library.Writer writer)
        {
            writer.WriteString(_info.id);
            writer.WriteString(_info.pswd);
            writer.WriteLong(_info.createdTicks);
            writer.WriteInt(_info.subjects.Count);

            var iter = _info.subjects.GetEnumerator();
            while (iter.MoveNext())
            {
                var pair = iter.Current;
                writer.WriteLong(pair.Value);

                var eRet = Logic.EventManager.Instance.SerializeObject(pair.Key.value, ref writer);
                if (!eRet.Success) Library.ThrowHelper.ThrowFailToConvert("User::SerializeInfo");
            }

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

        public void InsertSubject(long objUid, int handleValue)
        {
            _info.subjects.Add(new Logic.slObjectHandle(handleValue), objUid);
        }

        public bool TryCaptureSubject(Logic.slObjectHandle selectedHandle, out long uid)
        {
            if (!_info.subjects.TryGetValue(selectedHandle, out uid))
                return false;

            SubjectHandle = selectedHandle;
            return true;
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
