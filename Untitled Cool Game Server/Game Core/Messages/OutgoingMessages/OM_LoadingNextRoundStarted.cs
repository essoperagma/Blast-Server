using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using Game_Core.Blocks;
using Game_Core.Obstacles;
using Game_Core.Utilities;
using NetWorker.Utilities;

namespace Game_Core.Messages.OutgoingMessages
{
    class OM_LoadingNextRoundStarted : OutgoingMessageImp
    {
        public static void SendMessage(Arena arena)
        {
            RawMessage message = PrepareMessageFor(typeof(OM_LoadingNextRoundStarted));

            AddLandInfo(message,arena);
            AddObstacleInfo(message,arena);

            List<RawMessage> playerInfos = new List<RawMessage>(arena.players.Count);
            foreach (var p in arena.players)
            {
                RawMessage pMes = new RawMessage();

                pMes.putInt("pId",p.id);
                pMes.PutVector3("pos",p.position);

                int[] skillTypeIds = new int[p.skills.Count];
                for (int i = 0; i < skillTypeIds.Length; i++)
                    skillTypeIds[i] = TypeIdGenerator.idsOfSkills[p.skills[i].GetType()];

                pMes.putIntArray("skillTypeIds",skillTypeIds);

                playerInfos.Add(pMes);
            }
            message.putRawMessageArray("playerInfos",playerInfos.ToArray());

            foreach (var player in arena.players)
                player.user.client.SendMessage(message);
        }

        private static void AddLandInfo(RawMessage message, Arena arena)
        {
            int height = arena.land.blocks.GetLength(0), width = arena.land.blocks.GetLength(1);

            int[] blockIds = new int[width * height];
            int[] blockSkinIds = new int[width * height];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    blockIds[i * width + j] = (arena.land.blocks[i, j] == null ? -1 : TypeIdGenerator.idsOfBlocks[arena.land.blocks[i, j].GetType()]);
                    blockSkinIds[i * width + j] = (arena.land.blocks[i, j] == null ? 0 : arena.land.blocks[i, j].skinId);
                }

            message.putFloat("landWidth", arena.land.width);
            message.putFloat("landHeight", arena.land.height);
            message.putFloat("blockSize", arena.land.blockSize);
            message.putIntArray("blockTypeIds", blockIds);
            message.putIntArray("blockSkinIds", blockSkinIds);
            
        }

        private static void AddObstacleInfo(RawMessage message, Arena arena)
        {

            int obstacleCount = arena.obstacles.Count; 
            int[] obstacleTypeIds = new int[obstacleCount];
            int[] obstacleIds = new int[obstacleCount];
            Vector3[] obstaclePositions = new Vector3[obstacleCount];
            float[] obstacleHealths = new float[obstacleCount];
            float[] obstacleRadiuses = new float[obstacleCount];
            int[] obstacleSkinIds = new int[obstacleCount];

            for (int i = 0; i < obstacleCount; i++)
            {
                Obstacle o = arena.obstacles[i];
                obstacleTypeIds[i] = TypeIdGenerator.idsOfObstacles[o.GetType()];
                obstacleIds[i] = o.id;
                obstaclePositions[i] = o.position;
                obstacleHealths[i] = o.health;
                obstacleRadiuses[i] = o.radius;
                obstacleSkinIds[i] = o.skinId;
            }

            message.putIntArray("obsTypes", obstacleTypeIds);
            message.putIntArray("obsIds", obstacleIds);
            message.PutVector3Array("obsPositions",obstaclePositions);
            message.putFloatArray("obsHealths",obstacleHealths);
            message.putFloatArray("obsRadiuses",obstacleRadiuses);
            message.putIntArray("obsSkins",obstacleSkinIds);
        }
    }
}
