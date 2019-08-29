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
            _world = WorldManager.Locator;
            _writer = new NetDataWriter();
            _packetActions = new Func<NetPeer, NetPacketReader, Result>[(int)Protocol.Code.Packet.__MAX__];
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
            var agent = _world.Agents.CreateEmpty(id);
            if (null == agent)
                Library.ThrowHelper.ThrowWrongArgument("Duplicate agent id on enter.");

            agent.ConstructUser(peer);
        }

        public void OnLeave(NetPeer peer)
        {
            var id = peer.Id;
            var agent = _world.Agents.Get(id);
            if (agent == null)
                Library.ThrowHelper.ThrowKeyNotFound("Not found agent id on leave.");

            var iter = agent.GetSceneEnumerator();
            while (iter.MoveNext())
            {
                iter.Current.CancelNotification(id);
            }

            _world.Agents.Remove(agent);
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
        
        private Result _onKeyboard(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.Get(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            var scene = ReceiverHelper.GetCurrentScene(_world, agent);
            if (null == scene)
                return new Result(Protocol.Code.Error.NullReferenceCollider);

            var keyData = reader.Get<Protocol.Packet.KeyboardData>();

            _writer.Reset();
            _writer.Put(keyData.press);
            _writer.Put(keyData.key);

            var sceneIter = scene.GetEnumerator();
            while (sceneIter.MoveNext())
            {
                var subscriber = sceneIter.Current;
                subscriber.Push(_writer);
            }
            
            var eventManager = Logic.EventManager.Locator;
            eventManager.Insert(eventManager.CurrentFrame, 
                new Logic.Controller { type = Logic.ControlType.Keyboard, key = keyData.key, value = keyData.press ? 1 : 0 });

            return new Result();
        }

        private Result _onTouch(NetPeer peer, NetPacketReader reader)
        {
            return new Result();
        }

        private Result _onSelectSubject(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.Get(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);
            
            var subjectData = reader.Get<Protocol.Packet.ObjectData>();

            // 해당 계정이 이 핸들을 취할 수 있는지에 대한 검증이 필요하다.

            agent.CaptureSubject(subjectData.handleValue);

            var subjectHandle = agent.SubjectHandle;
            if (!_world.Colliders.TryGetCollider(subjectData.handleValue, out Collider collider))
                return new Result(Protocol.Code.Error.NullReferenceCollider);
            
            return new Result();
        }

        private Result _onSubscribeScene(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.Get(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            _writer.Reset();
            _writer.Put((int)Protocol.Code.Packet.SubscribeScene);

            var writer = new Library.Writer(1 << 10);
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
                    WorldManager.Locator.Service.SerializeObject(subjectHandle, ref writer);

                    subjectCount++;
                }
            }

            _writer.Put(subjectCount);
            _writer.Put(writer.Buffer, 0, writer.Offset);

            peer.Send(_writer, DeliveryMethod.ReliableOrdered);

            return new Result();
        }

        private Result _onUnsubscribeScene(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.Get(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            _writer.Reset();
            _writer.Put((int)Protocol.Code.Packet.UnsubscribeScene);

            var subData = reader.Get<Protocol.Packet.SubscribeData>();
            for (int i = 0; i != subData.count; i++)
            {
                var data = reader.Get<Protocol.Packet.SceneData>();
                var scene = _world.Scenes.Get(new SceneHandle(subData.worldIndex, data));
                if (!agent.UnsubscribeScene(scene))
                    return new Result(Protocol.Code.Error.NullReferenceScene);

                if (!scene.CancelNotification(id))
                    return new Result(Protocol.Code.Error.SubsrciberIdNotFound);

                var subCount = scene.Count;
                if (subCount != 0)
                {
                    var sceneIter = scene.GetEnumerator();
                    while (sceneIter.MoveNext())
                    {
                        var subjectHandle = sceneIter.Current.SubjectHandle;
                        _writer.Put(subjectHandle.value);
                    }
                }
                else
                {
                    _world.Scenes.Remove(new SceneHandle(subData.worldIndex, data));
                }
            }

            _writer.Put(-1);    // end
            peer.Send(_writer, DeliveryMethod.ReliableOrdered);

            return new Result();
        }

        private Result _onInteraction(NetPeer peer, NetPacketReader reader)
        {
            var data = reader.Get<Protocol.Packet.InteractionData>();
            
            if (!_world.Colliders.TryGetCollider(data.lhsValue, out Collider lcollider))
                return new Result(Protocol.Code.Error.NullReferenceCollider);

            if (!_world.Colliders.TryGetCollider(data.rhsValue, out Collider rcollider))
                return new Result(Protocol.Code.Error.NullReferenceCollider);

            if (!ReceiverHelper.ValidateInteract(ref lcollider, ref rcollider))
                return new Result(Protocol.Code.Error.IllegalityDataForInteraction);

            var eventManager = Logic.EventManager.Locator;
            eventManager.Insert(eventManager.CurrentFrame, new Logic.Interaction { lhs = data.lhsValue, rhs = data.rhsValue });

            return new Result();
        }
    }
}
