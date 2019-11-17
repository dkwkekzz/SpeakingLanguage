using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol.Packet
{
    public struct Error : INetSerializable
    {
        public Protocol.Code.Packet code;
        public Protocol.Code.Error error;

        public void Deserialize(NetDataReader reader)
        {
            code = (Protocol.Code.Packet)reader.GetInt();
            error = (Protocol.Code.Error)reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((int)code);
            writer.Put((int)error);
        }
    }
}
