using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol.Packet
{
    public struct KeyboardData : INetSerializable
    {
        public bool press;
        public int key;

        public void Deserialize(NetDataReader reader)
        {
            press = reader.GetBool();
            key = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(press);
            writer.Put(key);
        }
    }
}
