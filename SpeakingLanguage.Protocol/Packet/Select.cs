using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol.Packet
{
    public struct Select : INetSerializable
    {
        public int subjectKey;

        public void Deserialize(NetDataReader reader)
        {
            subjectKey = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(subjectKey);
        }
    }
}
