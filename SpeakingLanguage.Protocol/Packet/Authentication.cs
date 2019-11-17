using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol.Packet
{
    public struct Authentication : INetSerializable
    {
        public string id;
        public string pswd;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(id);
            writer.Put(pswd);
        }

        public void Deserialize(NetDataReader reader)
        {
            id = reader.GetString();
            pswd = reader.GetString();
        }
    }

    namespace S2C
    {
        public struct Authentication : INetSerializable
        {
            public string id;
            public string pswd;
            public int handle;

            public void Serialize(NetDataWriter writer)
            {
                writer.Put(id);
                writer.Put(pswd);
                writer.Put(handle);
            }

            public void Deserialize(NetDataReader reader)
            {
                id = reader.GetString();
                pswd = reader.GetString();
                handle = reader.GetInt();
            }
        }
    }
}
