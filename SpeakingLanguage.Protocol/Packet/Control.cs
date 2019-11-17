using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol.Packet
{
    public struct Control : INetSerializable
    {
        public int handle;
        public bool press;
        public int key;

        public void Deserialize(NetDataReader reader)
        {
            handle = reader.GetInt();
            press = reader.GetBool();
            key = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(handle);
            writer.Put(press);
            writer.Put(key);
        }
    }
}
