using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
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
            _packetActions[(int)Protocol.Code.Packet.SelectScene] = _onSelectScene;
            _packetActions[(int)Protocol.Code.Packet.SubscribeScene] = _onSubscribeScene;
            _packetActions[(int)Protocol.Code.Packet.UnsubscribeScene] = _onUnsubscribeScene;
            _packetActions[(int)Protocol.Code.Packet.Interaction] = _onInteraction;
        }

        public void OnEnter(NetPeer peer)
        {
            var id = peer.Id;
            var agent = _world.Agents.CreateEmpty(id);
            if (null == agent)
            {
                // cast failure
                return; //new Result(Protocol.Code.Error.AlreadyExistAgent);
            }

            agent.ConstructUser(peer);
            agent.CaptureScene(_world.Scenes.Get(SceneHandle.Lobby));
        }

        public void OnLeave(NetPeer peer)
        {
            var id = peer.Id;
            var agent = _world.Agents.Get(id);
            if (agent == null)
                return; //new Result(Protocol.Code.Error.NullReferenceAgent);

            agent.ClearSubscribe();

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

            var scene = agent.CurrentScene;
            Library.Tracer.Assert(null != scene);

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

            return new Result();
        }

        private Result _onTouch(NetPeer peer, NetPacketReader reader)
        {
            return new Result();
        }

        private Result _onSelectScene(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.Get(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);
            
            var data = reader.Get<Protocol.Packet.SceneData>();
            var newScene = _world.Scenes.Get(new SceneHandle(data));
            var lastScene = agent.CaptureScene(newScene);

            // validate: 내가 선택한 subjectHandle의 position이 scene에 적합해야 한다.

            return new Result();
        }

        private Result _onSubscribeScene(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.Get(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            // scenedata 여러개 한번에 처리하도록...
            var data = reader.Get<Protocol.Packet.SceneData>();
            var scene = _world.Scenes.Get(new SceneHandle(data));
            if (!scene.TryAddSubscribe(agent))
            {
                scene = _world.Scenes.ExpandScene(scene);
                scene.TryAddSubscribe(agent);
            }

            scene = agent.SubscribeScene(scene);
            if (null == scene)
                return new Result(Protocol.Code.Error.OverflowSubscribe);

            _writer.Reset();
            _writer.Put((int)Protocol.Code.Packet.SubscribeScene);

            var writer = new Library.Writer(1 << 10);
            var sceneIter = scene.GetEnumerator();
            while (sceneIter.MoveNext())
            {
                var subjectHandle = sceneIter.Current.SubjectHandle;
                WorldManager.Locator.Service.SerializeObject(subjectHandle, ref writer);
            }
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

            var data = reader.Get<Protocol.Packet.SceneData>();
            var scene = _world.Scenes.Get(new SceneHandle(data));
            scene.CancelSubscribe(id);

            if (!agent.UnsubscribeScene(scene))
                return new Result(Protocol.Code.Error.NullReferenceScene);

            //_writer.Reset();
            //_writer.Put((int)Protocol.Code.Packet.UnsubscribeScene);
            //
            //var sceneIter = scene.GetEnumerator();
            //while (sceneIter.MoveNext())
            //{
            //    var subjectHandle = sceneIter.Current.SubjectHandle;
            //    _writer.Put(subjectHandle.value);
            //}
            //
            //peer.Send(_writer, DeliveryMethod.ReliableOrdered);

            return new Result();
        }

        private Result _onInteraction(NetPeer peer, NetPacketReader reader)
        {
            var data = reader.Get<Protocol.Packet.InteractionData>();

            // interaction 검증

            var eventManager = Logic.EventManager.Locator;
            eventManager.Insert(eventManager.CurrentFrame, new Logic.Interaction { lhs = data.lhsValue, rhs = data.rhsValue });

            return new Result();
        }
    }
}
