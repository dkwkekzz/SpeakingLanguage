using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol.Packet
{
    public struct Success : INetSerializable
    {
        public Protocol.Code.Packet code;

        public void Deserialize(NetDataReader reader)
        {
            code = (Protocol.Code.Packet)reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((int)code);
        }
    }
}
