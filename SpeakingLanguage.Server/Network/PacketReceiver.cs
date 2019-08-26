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

        public PacketReceiver(ref Logic.StartInfo info)
        {
            _world = new WorldManager(ref info);
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
            var agent = new Agent(peer);
            var scene = _world.GetScene(SceneHandle.Lobby);
            _world.EnterScene(ref agent, scene);
        }

        public void OnLeave(NetPeer peer)
        {
            _world.LeaveScene(peer.Id);
        }

        public void OnReceive(NetPeer peer, NetPacketReader reader)
        {
            var code = reader.GetInt();
            var res = _packetActions[code](peer, reader);
            if (!res.Success)
            {
                Library.Tracer.Error($"[CastError] id: {peer.Id.ToString()}, code: {((Protocol.Code.Packet)code).ToString()}, err: {res.Error.ToString()}");

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
            var scene = _world.FindScene(id);
            if (scene == null)
                return new Result(Protocol.Code.Error.NullReferenceScene);
           
            var keyData = reader.Get<Protocol.Packet.KeyboardData>();

            _writer.Reset();
            _writer.Put(keyData.press);
            _writer.Put(keyData.key);

            var sceneIter = scene.GetEnumerator();
            while (sceneIter.MoveNext())
            {
                var agent = sceneIter.Current;
                var nearPeer = agent.Peer;
                if (nearPeer != null)
                {
                    nearPeer.Send(_writer, DeliveryMethod.ReliableOrdered);
                }
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
            var exist = _world.LeaveScene(id);
            if (!exist)
                return new Result(Protocol.Code.Error.NullReferenceScene);

            var selectData = reader.Get<Protocol.Packet.SceneData>();
            var scene = _world.GetScene(new SceneHandle(selectData));
            if (null == scene)
                return new Result(Protocol.Code.Error.NullReferenceScene);

            var agent = new Agent(peer);
            _world.EnterScene(ref agent, scene);

            return new Result();
        }

        private Result _onSubscribeScene(NetPeer peer, NetPacketReader reader)
        {
            var selectData = reader.Get<Protocol.Packet.SceneData>();
            var agent = new Agent(peer);
            var scene = _world.SubscribeScene(ref agent, new SceneHandle(selectData));
            if (null == scene)
                return new Result(Protocol.Code.Error.NullReferenceScene);

            return new Result();
        }

        private Result _onUnsubscribeScene(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var selectData = reader.Get<Protocol.Packet.SceneData>();
            var scene = _world.UnsubscribeScene(id, new SceneHandle(selectData));
            if (null == scene)
                return new Result(Protocol.Code.Error.NullReferenceScene);

            return new Result();
        }

        private Result _onInteraction(NetPeer peer, NetPacketReader reader)
        {
            var interactionData = reader.Get<Protocol.Packet.InteractionData>();

            // interaction 검증

            return new Result();
        }
    }
}
