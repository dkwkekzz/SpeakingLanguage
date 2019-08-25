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

        public PacketReceiver(WorldManager world)
        {
            _world = world;
            _writer = new NetDataWriter();
            _packetActions = new Func<NetPeer, NetPacketReader, Result>[(int)Protocol.Code.Packet.__MAX__];
            _packetActions[(int)Protocol.Code.Packet.Keyboard] = _onKeyboard;
        }

        public void OnEnter(NetPeer peer)
        {
            _world.EnterScene(SceneHandle.Lobby, new Agent(peer));
        }

        public void OnLeave(NetPeer peer)
        {
            _world.LeaveScene(peer.Id);
        }

        public void OnReceive(int code, NetPeer peer, NetPacketReader reader)
        {
            var res = _packetActions[code](peer, reader);
            if (!res.Success)
            {
                Library.Tracer.Error($"[CastError] id: {peer.Id.ToString()}, err: {res.Error.ToString()}");

                _writer.Reset();
                _writer.Put((int)res.Error);
                peer.Disconnect(_writer);
            }
        }
        
        private Result _onKeyboard(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var scene = _world.FindScene(id);
            if (scene == null)
                return new Result(Protocol.Code.Error.NullReferenceScene);
           
            var key = reader.GetInt();

            _writer.Reset();
            _writer.Put(key);

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

        private Result _onSelectScene(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var scene = _world.FindScene(id);
            if (scene == null)
                return new Result(Protocol.Code.Error.NullReferenceScene);

            var key = reader.GetInt();

            _writer.Reset();
            _writer.Put(key);

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
    }
}
