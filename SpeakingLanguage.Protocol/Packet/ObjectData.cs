using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol.Packet
{
    public struct ObjectData : INetSerializable
    {
        public int handleValue;

        public void Deserialize(NetDataReader reader)
        {
            handleValue = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(handleValue);
        }
    }
}
