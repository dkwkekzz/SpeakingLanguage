using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server.Networks
{
    internal class PacketReceiver
    {
        private readonly Func<Agent, NetPacketReader, Result>[] _packetActions;

        public PacketReceiver()
        {
            _packetActions = new Func<Agent, NetPacketReader, Result>[(int)Protocol.Code.Packet.__MAX__];
            _packetActions[(int)Protocol.Code.Packet.Authentication] = _onAuthentication;
            _packetActions[(int)Protocol.Code.Packet.Control] = _onControl;
            _packetActions[(int)Protocol.Code.Packet.Interaction] = _onInteraction;
        }

        public void OnEnter(NetPeer peer)
        {
            var agentCol = World.Instance.AgentCollection;

            var id = peer.Id;
            if (agentCol.Contains(id))
                Library.ThrowHelper.ThrowWrongArgument("Duplicate agent id on enter.");

            agentCol.Insert(peer);
        }

        public void OnLeave(NetPeer peer)
        {
            var agentCol = World.Instance.AgentCollection;

            var id = peer.Id;
            if (!agentCol.TryGetAgent(id, out Agent agent))
                Library.ThrowHelper.ThrowWrongArgument("Not found agent id on leave.");
        }

        public void OnReceive(NetPeer peer, NetPacketReader reader)
        {
            var agentCol = World.Instance.AgentCollection;

            var id = peer.Id;
            if (!agentCol.TryGetAgent(id, out Agent agent))
                Library.ThrowHelper.ThrowWrongArgument("Not found agent id on receive.");

            var code = reader.GetInt();
            var res = _packetActions[code](agent, reader);
            if (!res.IsSuccess)
            {
                PacketHelper.CastFailure(agent, (Protocol.Code.Packet)code, res.Error);
                return;
            }

            Library.Tracer.Write($"[CastSuccess] id: {peer.Id.ToString()}, code: {((Protocol.Code.Packet)code).ToString()}");
        }

        private Result _onAuthentication(Agent agent, NetPacketReader reader)
        {
            if (agent.IsAlive)
                return new Result(Protocol.Code.Error.DuplicateAuthentication);

            var data = reader.Get<Protocol.Packet.Authentication>();
            var handle = World.Instance.KeyGenerator.NewHandle;
            agent.Construct(data.id, data.pswd, handle);

            var writer = World.Instance.WriteHolder.NewWriter;
            writer.WriteInt((int)Protocol.Code.Packet.Authentication);
            writer.WriteString(agent.Auth.id);
            writer.WriteString(agent.Auth.pswd);
            Logic.Synchronization.Serialize(agent.Auth.handle, ref writer);

            agent.Send(writer.Buffer, 0, writer.Offset);

            return new Result();
        }

        private Result _onControl(Agent agent, NetPacketReader reader)
        {
            if (!agent.IsAlive)
                return new Result(Protocol.Code.Error.InvaildAuthentication);

            var data = reader.Get<Protocol.Packet.Control>();
            Logic.Receiver.Control(data.handle, data.press, data.key);

            agent.CurrentScene.DataWriter.Put((int)Protocol.Code.Packet.Control);
            agent.CurrentScene.DataWriter.Put(data);
            return new Result();
        }

        private Result _onInteraction(Agent agent, NetPacketReader reader)
        {
            if (!agent.IsAlive)
                return new Result(Protocol.Code.Error.InvaildAuthentication);

            var data = reader.Get<Protocol.Packet.Interaction>();
            Logic.Receiver.Interaction(data.lhsValue, data.rhsValue);

            agent.CurrentScene.DataWriter.Put((int)Protocol.Code.Packet.Interaction);
            agent.CurrentScene.DataWriter.Put(data);
            return new Result();
        }
    }
}
