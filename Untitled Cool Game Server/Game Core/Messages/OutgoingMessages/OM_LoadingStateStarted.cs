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
    class OM_LoadingStateStarted : OutgoingMessageImp
    {
        public static void SendMessage(Arena arena)
        {
            RawMessage message = PrepareMessageFor(typeof (OM_LoadingStateStarted));

            int[] userIds = new int[arena.players.Count];
            string[] usernames = new string[userIds.Length];
            int[] playerIds = new int[userIds.Length];
            Vector3[] positions = new Vector3[playerIds.Length];

            for(int i=0; i<arena.players.Count; i++)
            {
                userIds[i] = arena.players[i].user.id;
                usernames[i] = arena.players[i].user.username;
                playerIds[i] = arena.players[i].id;
                positions[i] = arena.players[i].position;
            }

            int height = arena.land.blocks.GetLength(0), width = arena.land.blocks.GetLength(1);

            int[] blockIds = new int[width * height];
            int[] blockSkinIds = new int[width * height];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    blockIds[i * width + j] = (arena.land.blocks[i, j] ==null?-1:TypeIdGenerator.idsOfBlocks[arena.land.blocks[i, j].GetType()]);
                    blockSkinIds[i * width + j] = (arena.land.blocks[i, j] ==null?0:arena.land.blocks[i, j].skinId);
                }

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

            message.putIntArray("userIds",userIds);
            message.putUTF8StringArray("usernames", usernames);
            message.putIntArray("playerIds", playerIds);
            message.PutVector3Array("positions",positions);
            message.putFloat("landWidth", arena.land.width);
            message.putFloat("landHeight", arena.land.height);
            message.putFloat("blockSize", arena.land.blockSize);
            message.putIntArray("blockTypeIds",blockIds);
            message.putIntArray("blockSkinIds",blockSkinIds);
            message.putIntArray("obsTypes", obstacleTypeIds);
            message.putIntArray("obsIds", obstacleIds);
            message.PutVector3Array("obsPositions",obstaclePositions);
            message.putFloatArray("obsHealths",obstacleHealths);
            message.putFloatArray("obsRadiuses",obstacleRadiuses);
            message.putIntArray("obsSkins",obstacleSkinIds);

            foreach(var p in arena.players)
                p.user.client.SendMessage(message);
        }
    }
}
