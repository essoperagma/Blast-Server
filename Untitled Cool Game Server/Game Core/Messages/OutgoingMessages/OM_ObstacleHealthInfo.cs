using GameCore.Messages;
using Game_Core.Blocks;
using Game_Core.Obstacles;
using Game_Core.Utilities;
using NetWorker.Utilities;

namespace Game_Core.Messages.OutgoingMessages
{
    class OM_ObstacleHealthInfo : OutgoingMessageImp
    {
        public static void SendMessage(Arena arena, Obstacle obstacle, Vector3 impactPosition)
        {
            RawMessage message = PrepareMessageFor( typeof(OM_ObstacleHealthInfo));
            message.putInt("obstacleId",obstacle.id);
            message.putFloat("health",obstacle.health);
            message.PutVector3("impactPosition", impactPosition);

            foreach(var p in arena.players)
                p.user.client.SendMessage(message);
        }
    }
}
