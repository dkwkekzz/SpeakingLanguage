using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol.Packet
{
    public struct SceneData : INetSerializable
    {
        public int sceneX;
        public int sceneY;
        public int sceneZ;

        public void Deserialize(NetDataReader reader)
        {
            sceneX = reader.GetInt();
            sceneY = reader.GetInt();
            sceneZ = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(sceneX);
            writer.Put(sceneY);
            writer.Put(sceneZ);
        }
    }
}
