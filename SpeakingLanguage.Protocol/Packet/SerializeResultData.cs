using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol.Packet
{
    public struct SerializeResultData : INetSerializable
    {
        public bool success;

        public void Deserialize(NetDataReader reader)
        {
            success = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(success);
        }
    }
}
