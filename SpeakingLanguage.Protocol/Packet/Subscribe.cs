using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol.Packet
{
    public struct Subscribe : INetSerializable
    {
        public int sceneIndex;

        public void Deserialize(NetDataReader reader)
        {
            sceneIndex = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(sceneIndex);
        }
    }
}
