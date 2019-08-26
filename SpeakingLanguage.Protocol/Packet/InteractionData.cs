using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol.Packet
{
    public struct InteractionData : INetSerializable
    {
        public struct ObjectHandle
        {
            public int value;
        }

        public ObjectHandle lhs;
        public ObjectHandle rhs;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(lhs.value);
            writer.Put(rhs.value);
        }

        public void Deserialize(NetDataReader reader)
        {
            lhs.value = reader.GetInt();
            rhs.value = reader.GetInt();
        }
    }
}
