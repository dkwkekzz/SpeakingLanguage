using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal struct SceneHandle : IEquatable<SceneHandle>
    {
        public static SceneHandle Lobby { get; } = new SceneHandle { worldIndex = 0, sceneX = 0, sceneY = 0, sceneZ = 0 };

        public int worldIndex;
        public int sceneX;
        public int sceneY;
        public int sceneZ;

        public bool Equals(SceneHandle other)
        {
            return worldIndex == other.worldIndex
                && sceneX == other.sceneX
                && sceneY == other.sceneY
                && sceneZ == other.sceneZ;
        }
    }
}
