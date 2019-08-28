using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal struct SceneHandle : IEquatable<SceneHandle>
    {
        public static SceneHandle Lobby { get; } = new SceneHandle { worldIndex = 0, xValue = 0, yValue = 0, zValue = 0 };

        public int worldIndex;
        public int xValue;
        public int yValue;
        public int zValue;

        public SceneHandle(int world, Protocol.Packet.SceneData data)
        {
            worldIndex = world;
            xValue = data.sceneX;
            yValue = data.sceneY;
            zValue = data.sceneZ;
        }

        public bool Equals(SceneHandle other)
        {
            return worldIndex == other.worldIndex
                && xValue == other.xValue
                && yValue == other.yValue
                && zValue == other.zValue;
        }
    }
}
