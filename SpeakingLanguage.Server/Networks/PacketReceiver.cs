using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server.Networks
{
    internal class PacketReceiver
    {
        private struct Result
        {
            public Protocol.Code.Error Error { get; }
            public Logic.EventError LogicError { get; }
            public bool Success => Error == Protocol.Code.Error.None;

            public Result(Protocol.Code.Error error, Logic.EventError eventError = Logic.EventError.None)
            {
                Error = error;
                LogicError = eventError;
            }
        }

        private readonly WorldManager _world;
        private readonly IDatabase _database;
        private readonly NetDataWriter _errorWriter;
        private readonly byte[] _serializeBuffer;
        private readonly Func<NetPeer, NetPacketReader, Result>[] _packetActions;

        public PacketReceiver()
        {
            _world = WorldManager.Instance;
            _database = new FileDatabase();
            _errorWriter = new NetDataWriter(true, 128);
            _serializeBuffer = new byte[512];
            _packetActions = new Func<NetPeer, NetPacketReader, Result>[(int)Protocol.Code.Packet.__MAX__];
            _packetActions[(int)Protocol.Code.Packet.Authentication] = _onAuthentication;
            _packetActions[(int)Protocol.Code.Packet.ConstructIdentifier] = _onConstructIdentifier;
            _packetActions[(int)Protocol.Code.Packet.Terminate] = _onTerminate;
            _packetActions[(int)Protocol.Code.Packet.Keyboard] = _onKeyboard;
            _packetActions[(int)Protocol.Code.Packet.Touch] = _onTouch;
            _packetActions[(int)Protocol.Code.Packet.SelectSubject] = _onSelectSubject;
            _packetActions[(int)Protocol.Code.Packet.CreateSubject] = _onCreateSubject;
            _packetActions[(int)Protocol.Code.Packet.SubscribeScene] = _onSubscribeScene;
            _packetActions[(int)Protocol.Code.Packet.UnsubscribeScene] = _onUnsubscribeScene;
            _packetActions[(int)Protocol.Code.Packet.Interaction] = _onInteraction;
        }

        public void OnEnter(NetPeer peer)
        {
            var id = peer.Id;
            var agent = _world.UserFactory.Create(id);
            if (null == agent)
                Library.ThrowHelper.ThrowWrongArgument("Duplicate agent id on enter.");

            agent.CapturePeer(peer);
            _world.Agents.Insert(agent);
        }

        public void OnLeave(NetPeer peer)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                Library.ThrowHelper.ThrowKeyNotFound("Not found agent id on leave.");

            var iter = agent.GetSceneEnumerator();
            while (iter.MoveNext())
            {
                iter.Current.CancelNotification(id);
            }
            
            agent.Save(_database);

            _world.Agents.Remove(agent);
            _world.UserFactory.Destroy(agent);
        }

        public void OnReceive(NetPeer peer, NetPacketReader reader)
        {
            var code = reader.GetInt();
            var res = _packetActions[code](peer, reader);
            if (!res.Success)
            {
                Library.Tracer.Error($"[CastFailure] id: {peer.Id.ToString()}, code: {((Protocol.Code.Packet)code).ToString()}, err: {res.Error.ToString()}, subMsg: {res.LogicError.ToString()}");

                _errorWriter.Reset();
                _errorWriter.Put((int)Protocol.Code.Packet.None);
                _errorWriter.Put(code);
                _errorWriter.Put((int)res.Error);
                _errorWriter.Put((int)res.LogicError);

                if (!Config.debug)
                    peer.Disconnect(_errorWriter);
                else
                    peer.Send(_errorWriter, DeliveryMethod.ReliableOrdered);
                return;
            }

            Library.Tracer.Write($"[CastSuccess] id: {peer.Id.ToString()}, code: {((Protocol.Code.Packet)code).ToString()}");
        }

        public void FlushReponses()
        {
            _database.FlushResponse();
        }

        private Result _onAuthentication(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            var data = reader.Get<Protocol.Packet.AuthenticationData>();
            if (!ReceiverHelper.ValidateAuthenticate(ref data))
                return new Result(Protocol.Code.Error.FailToAuthentication);

            _database.RequestReadUser(agent, data.id, data.pswd);
            return new Result();
        }

        private Result _onConstructIdentifier(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            var data = reader.Get<Protocol.Packet.AuthenticationData>();
            if (!ReceiverHelper.ValidateAuthenticate(ref data))
                return new Result(Protocol.Code.Error.FailToAuthentication);

            var writer = new Library.Writer(_serializeBuffer, 0);
            writer.WriteString(data.id);
            writer.WriteString(data.pswd);
            writer.WriteLong(Library.Ticker.GlobalTicks);
            writer.WriteInt(0);

            var reader2 = new Library.Reader(_serializeBuffer, writer.Offset);
            agent.DeserializeInfo(ref reader2);

            return new Result();
        }

        private Result _onTerminate(NetPeer peer, NetPacketReader reader)
        {
            return new Result();
        }

        private Result _onKeyboard(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            var scene = ReceiverHelper.GetCurrentScene(_world, agent);
            if (null == scene)
                return new Result(Protocol.Code.Error.NullReferenceCollider);

            var data = reader.Get<Protocol.Packet.KeyboardData>();

            var eventManager = Logic.EventManager.Instance;
            var eRes = eventManager.InsertKeyboard(agent.SubjectHandle.value, data.key, data.press ? 1 : 0);
            if (!eRes.Success)
                return new Result(Protocol.Code.Error.NullReferenceSubjectHandle, eRes.error);

            var sceneIter = scene.GetEnumerator();
            while (sceneIter.MoveNext())
            {
                var subscriber = sceneIter.Current;
                subscriber.DataWriter.Put((int)Protocol.Code.Packet.Keyboard);
                subscriber.DataWriter.Put(data);
            }
            
            return new Result();
        }

        private Result _onTouch(NetPeer peer, NetPacketReader reader)
        {
            return new Result();
        }

        private Result _onSelectSubject(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);
            
            var subjectData = reader.Get<Protocol.Packet.ObjectData>();
            if (!agent.TryCaptureSubject(subjectData.handleValue, out long uid))
                return new Result(Protocol.Code.Error.NullReferenceSubjectHandle);

            agent.DataWriter.Put((int)Protocol.Code.Packet.SelectSubject);
            agent.DataWriter.Put(new Protocol.Packet.ObjectData { handleValue = subjectData.handleValue });

            return new Result();
        }

        private Result _onCreateSubject(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            var objUid = ReceiverHelper.CreateObjectUid(id);
            var eRes = Logic.EventManager.Instance.CreateObject();
            if (!eRes.Success)
                return new Result(Protocol.Code.Error.NullReferenceSubjectHandle, eRes.error); 

            var handleValue = eRes.result;
            agent.InsertSubject(objUid, handleValue);
            
            agent.DataWriter.Put((int)Protocol.Code.Packet.CreateSubject);
            agent.DataWriter.Put(new Protocol.Packet.ObjectData { handleValue = handleValue });

            return new Result();
        }

        private Result _onSubscribeScene(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            var eventManager = Logic.EventManager.Instance;

            var writer = new Library.Writer(_serializeBuffer, 0);
            var subData = reader.Get<Protocol.Packet.SubscribeData>();
            for (int i = 0; i != subData.count; i++)
            {
                var data = reader.Get<Protocol.Packet.SceneData>();
                var scene = _world.Scenes.Get(new SceneHandle(subData.worldIndex, data));
                if (!scene.TryAddNotification(agent))
                {
                    scene = _world.Scenes.ExpandScene(scene);
                    scene.TryAddNotification(agent);
                }

                scene = agent.SubscribeScene(scene);
                if (null == scene)
                    return new Result(Protocol.Code.Error.OverflowSubscribe);

                var sceneIter = scene.GetEnumerator();
                while (sceneIter.MoveNext())
                {
                    var subjectHandle = sceneIter.Current.SubjectHandle;
                    var eRes = eventManager.SerializeObject(subjectHandle, ref writer);
                    if (!eRes.Success)
                        return new Result(Protocol.Code.Error.FailToSerialize, eRes.error);
                }
            }
            writer.WriteInt(0); // for null subject

            agent.DataWriter.Put((int)Protocol.Code.Packet.SubscribeScene);
            agent.DataWriter.Put(writer.Buffer, 0, writer.Offset);

            return new Result();
        }

        private Result _onUnsubscribeScene(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);
            
            agent.DataWriter.Put((int)Protocol.Code.Packet.UnsubscribeScene);
            
            var subData = reader.Get<Protocol.Packet.SubscribeData>();
            for (int i = 0; i != subData.count; i++)
            {
                var data = reader.Get<Protocol.Packet.SceneData>();
                var scene = _world.Scenes.Get(new SceneHandle(subData.worldIndex, data));
                if (!agent.UnsubscribeScene(scene))
                    return new Result(Protocol.Code.Error.NullReferenceScene);

                if (!scene.CancelNotification(id))
                    return new Result(Protocol.Code.Error.NullReferenceSubsrciber);

                var subCount = scene.Count;
                if (subCount != 0)
                {
                    var sceneIter = scene.GetEnumerator();
                    while (sceneIter.MoveNext())
                    {
                        var subjectHandle = sceneIter.Current.SubjectHandle;
                        agent.DataWriter.Put(new Protocol.Packet.ObjectData { handleValue = subjectHandle.value });
                    }
                }
                else
                {
                    _world.Scenes.Remove(new SceneHandle(subData.worldIndex, data));
                }
            }

            agent.DataWriter.Put(new Protocol.Packet.ObjectData { handleValue = 0 });   // for null subject
            
            return new Result();
        }

        private Result _onInteraction(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            var scene = ReceiverHelper.GetCurrentScene(_world, agent);
            if (null == scene)
                return new Result(Protocol.Code.Error.NullReferenceCollider);

            var data = reader.Get<Protocol.Packet.InteractionData>();
            
            var eventManager = Logic.EventManager.Instance;
            var eRes = eventManager.InsertInteraction(data.lhsValue, data.rhsValue);
            if (!eRes.Success)
                return new Result(Protocol.Code.Error.FailToInteraction, eRes.error);

            var sceneIter = scene.GetEnumerator();
            while (sceneIter.MoveNext())
            {
                var subscriber = sceneIter.Current;
                subscriber.DataWriter.Put((int)Protocol.Code.Packet.Interaction);
                subscriber.DataWriter.Put(data);
            }
            
            return new Result();
        }
    }
}
