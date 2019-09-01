using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server.Network
{
    internal class PacketReceiver
    {
        private struct Result
        {
            public Protocol.Code.Error Error { get; }
            public bool Success => Error == Protocol.Code.Error.None;

            public Result(Protocol.Code.Error error)
            {
                Error = error;
            }
        }

        private readonly WorldManager _world;
        private readonly NetDataWriter _writer;
        private readonly Func<NetPeer, NetPacketReader, Result>[] _packetActions;

        public PacketReceiver()
        {
            _world = WorldManager.Instance;
            _writer = new NetDataWriter(true, 256);
            _packetActions = new Func<NetPeer, NetPacketReader, Result>[(int)Protocol.Code.Packet.__MAX__];
            _packetActions[(int)Protocol.Code.Packet.Authentication] = _onAuthentication;
            _packetActions[(int)Protocol.Code.Packet.Terminate] = _onTerminate;
            _packetActions[(int)Protocol.Code.Packet.Keyboard] = _onKeyboard;
            _packetActions[(int)Protocol.Code.Packet.Touch] = _onTouch;
            _packetActions[(int)Protocol.Code.Packet.SelectSubject] = _onSelectSubject;
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

            _world.Agents.Remove(agent);
            _world.UserFactory.Destroy(agent);
        }

        public void OnReceive(NetPeer peer, NetPacketReader reader)
        {
            var code = reader.GetInt();
            var res = _packetActions[code](peer, reader);
            if (!res.Success)
            {
                Library.Tracer.Error($"[CastFailure] id: {peer.Id.ToString()}, code: {((Protocol.Code.Packet)code).ToString()}, err: {res.Error.ToString()}");

                _writer.Reset();
                _writer.Put(code);
                _writer.Put((int)res.Error);
                peer.Disconnect(_writer);
                return;
            }

            Library.Tracer.Write($"[CastSuccess] id: {peer.Id.ToString()}, code: {((Protocol.Code.Packet)code).ToString()}");
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

            _world.Database.ConstructUser(agent, data.id, data.pswd);

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
            eventManager.Insert(new Logic.Controller { type = Logic.ControlType.Keyboard, key = data.key, value = data.press ? 1 : 0 });

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
            if (!ReceiverHelper.CanCaptureSubject(agent, ref subjectData))
                return new Result(Protocol.Code.Error.IllegalityDataForSelectSubject);

            if (!_world.Colliders.TryGetCollider(subjectData.handleValue, out Collider collider))
            {
                Library.Reader objReader;
                _world.Database.ConstructObject(subjectData.handleValue, out objReader);

                var eventManager = Logic.EventManager.Instance;
                eventManager.Service.DeserializeObject(ref objReader);
            }

            agent.CaptureSubject(subjectData.handleValue);

            return new Result();
        }

        private Result _onSubscribeScene(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            _writer.Reset();
            _writer.Put((int)Protocol.Code.Packet.SubscribeScene);

            var lenPos = _writer.Length;
            _writer.Put(0);

            var eventManager = Logic.EventManager.Instance;

            var writer = new Library.Writer(_writer.Data, _writer.Length);
            var subjectCount = 0;
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
                    eventManager.Service.SerializeObject(subjectHandle, ref writer);

                    subjectCount++;
                }
            }

            _writer.Length = writer.Offset;
            _writer.PutAt(lenPos, subjectCount);
            peer.Send(_writer, DeliveryMethod.ReliableOrdered);

            return new Result();
        }

        private Result _onUnsubscribeScene(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            _writer.Reset();
            _writer.Put((int)Protocol.Code.Packet.UnsubscribeScene);

            var lenPos = _writer.Length;
            _writer.Put(0);

            var subData = reader.Get<Protocol.Packet.SubscribeData>();
            var subjectCount = 0;
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
                        _writer.Put(subjectHandle.value);

                        subjectCount++;
                    }
                }
                else
                {
                    _world.Scenes.Remove(new SceneHandle(subData.worldIndex, data));
                }
            }

            _writer.PutAt(lenPos, subjectCount);  
            peer.Send(_writer, DeliveryMethod.ReliableOrdered);

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
            eventManager.Insert(new Logic.Interaction { lhs = data.lhsValue, rhs = data.rhsValue });

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
