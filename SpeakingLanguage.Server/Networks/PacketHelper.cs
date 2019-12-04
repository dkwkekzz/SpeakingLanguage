using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server.Networks
{
    internal struct Result
    {
        public Protocol.Code.Error Error { get; }
        public bool IsSuccess => Error == Protocol.Code.Error.None;

        public Result(Protocol.Code.Error error)
        {
            this.Error = error;
        }
    }

    internal static class PacketHelper
    {
        public static void CastFailure(Agent agent, Protocol.Code.Packet code, Protocol.Code.Error err)
        {
            Library.Tracer.Error($"[CastFailure] id: {agent.Id.ToString()}, " +
                   $"code: {code.ToString()}, " +
                   $"err: {err.ToString()}");

            var writer = World.Instance.WriteHolder.NewWriter;
            writer.WriteInt((int)Protocol.Code.Packet.Error);
            writer.WriteInt((int)code);
            writer.WriteInt((int)err);
            agent.Send(writer.Buffer, 0, writer.Offset);
        }
    }
}
