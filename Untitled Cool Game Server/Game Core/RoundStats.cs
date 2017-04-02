using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Core.Blocks;

namespace Game_Core
{
    public class RoundStats
    {
        public int winnerCount;
        public RoundStats previousRound;
        public List<Player> deathOrder;
        public int[] playerIds;
        public int[] hits;
        public int[] kills;
        public int[] finalGolds;

        public long lastAddedFrame;

        public RoundStats(List<Player> players, RoundStats previous = null)
        {
            deathOrder = new List<Player>(players.Count);
            playerIds  = new int[players.Count];
            hits  = new int[players.Count];
            kills = new int[players.Count];
            finalGolds = new int[players.Count];
            previousRound = previous;

            for (int i = 0; i < players.Count; i++)
                playerIds[i] = players[i].id;
        }

        public void PlayerDied(Player player)
        {
            deathOrder.Add(player);

            if (lastAddedFrame == Game.frameNumber)
                winnerCount++;
            else
                winnerCount = 1;

            lastAddedFrame = Game.frameNumber;
        }

        public void PlayerHit(Player player, int opponentCount=1)
        {
            int index = Array.IndexOf(playerIds, player.id);

            if (index > -1)
                hits[index] += opponentCount;
        }

        public void PlayerKilledOpponent(Player player, Player opponent)
        {
            int index = Array.IndexOf(playerIds, player.id);

            if (index > -1)
                kills[index]++;
        }

        public void FinalizeStats(Arena arena)
        {
            int remainingPlayers = 0;
            foreach (var p in arena.players.Where(i => i.IsAlive))// olmeyenleri de listeye ekleyelim (muhtemelen sadece 1 kisi)
            {
                deathOrder.Add(p);
                remainingPlayers++;
            }

            if (remainingPlayers != 0)
                winnerCount = remainingPlayers;

            // siralamaya puan vermiyoruz artik.
            //for (int i = 0; i < deathOrder.Count; i++)
                //deathOrder[i].gold += ( i < deathOrder.Count-winnerCount?i+1: deathOrder.Count);

            for (int i = 0, j=0; i < arena.players.Count; i++, j++)
            {
                if (playerIds[j] == arena.players[i].id)
                    finalGolds[j] = arena.players[i].gold;
                else
                    i--;
            }
        }
    }
}
