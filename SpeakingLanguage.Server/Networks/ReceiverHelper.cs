using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server.Networks
{
    internal static class ReceiverHelper
    {
        public static IScene GetCurrentScene(WorldManager world, User agent)
        {
            var subjectHandle = agent.SubjectHandle;

            SceneHandle selectedSceneHandle;
            if (!world.Colliders.TryGetCollider(subjectHandle.value, out Collider collider))
            {
                selectedSceneHandle = SceneHandle.Lobby;
            }
            else
            {
                var pos = collider.position;
                selectedSceneHandle = new SceneHandle
                {
                    worldIndex = pos.world,
                    xValue = pos.x > 0 ? pos.x / Protocol.Define.SCENE_WIDTH : 0,
                    yValue = pos.y > 0 ? pos.y / Protocol.Define.SCENE_HEIGHT : 0,
                    zValue = pos.z,
                };
            }
            
            return world.Scenes.Get(selectedSceneHandle);
        }

        public static bool ValidateScene(ref Protocol.Packet.SceneData sceneData, ref Collider collider)
        {
            var worldX = sceneData.sceneX * Protocol.Define.SCENE_WIDTH;
            var worldY = sceneData.sceneY * Protocol.Define.SCENE_HEIGHT;
            var pos = collider.position;
            if (worldX > pos.x || worldX + Protocol.Define.SCENE_WIDTH < pos.x)
                return false;

            if (worldY > pos.y || worldY + Protocol.Define.SCENE_HEIGHT < pos.y)
                return false;

            return true;
        }

        public static bool ValidateInteract(ref Collider lcollider, ref Collider rcollider)
        {
            var diffX = Math.Abs(lcollider.position.x - rcollider.position.x);
            var diffY = Math.Abs(lcollider.position.y - rcollider.position.y);
            var distance = lcollider.detection.radius + rcollider.detection.radius;
            return diffX * diffX + diffY * diffY >= distance * distance;
        }

        public static bool ValidateAuthenticate(ref Protocol.Packet.AuthenticationData data)
        {
            return true;
        }

        public static bool CanCaptureSubject(User agent, ref Protocol.Packet.ObjectData data)
        {
            return true;
        }
    }
}
