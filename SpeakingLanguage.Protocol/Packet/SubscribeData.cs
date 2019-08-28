using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol.Packet
{
    public struct SubscribeData : INetSerializable
    {
        public int worldIndex;
        public int count;

        public void Deserialize(NetDataReader reader)
        {
            worldIndex = reader.GetInt();
            count = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(worldIndex);
            writer.Put(count);
        }
    }
}
