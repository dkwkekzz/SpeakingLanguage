using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol.Packet
{
    public struct Interaction : INetSerializable
    {
        public int lhsValue;
        public int rhsValue;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(lhsValue);
            writer.Put(rhsValue);
        }

        public void Deserialize(NetDataReader reader)
        {
            lhsValue = reader.GetInt();
            rhsValue = reader.GetInt();
        }
    }
}
