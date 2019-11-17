using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol.Packet
{
    public struct Synchronization : INetSerializable
    {
        public int handle;

        public void Deserialize(NetDataReader reader)
        {
            handle = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(handle);
        }
    }

    namespace S2C
    {
        public struct Synchronization : INetSerializable
        {
            public int length;
            public byte[] rawData;

            public void Deserialize(NetDataReader reader)
            {
                length = reader.GetInt();
                reader.GetBytes(rawData, 0, length);
            }

            public void Serialize(NetDataWriter writer)
            {
                writer.Put(length);
                writer.Put(rawData, 0, length);
            }
        }
    }
}
