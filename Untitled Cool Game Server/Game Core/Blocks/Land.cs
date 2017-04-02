using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Core.Messages.OutgoingMessages;
using Game_Core.Obstacles;
using Game_Core.Utilities;

namespace Game_Core.Blocks
{
    public class Land
    {
        public static readonly Vector3 center = Vector3.zero;
        public float width;
        public float height;
        public float blockSize;
        public Block[,] blocks;
        private Arena arena;

        public Land(Arena arena, float width, float height, float blockSize)
        {
            this.arena = arena;
            this.width = width;
            this.height = height;
            this.blockSize = blockSize;

            int wBlocks = (int) (width/blockSize), hBlocks = (int) (height/blockSize);
            blocks = new Block[hBlocks,wBlocks];

            for(int i=0; i<hBlocks; i++)
                for(int j=0; j<wBlocks; j++)
                    blocks[i,j] = new StoneBlock();
            
        }

        public void ShrinkSingleFromBorders()
        {
            if (RandomInstance.instance.Next(2) == 0)
            {
                int indexI = RandomInstance.instance.Next(2)*(blocks.GetLength(0) - 1); // ya ilk ya son element
                int indexJ = RandomInstance.instance.Next(blocks.GetLength(1));

                int direction = indexI == 0 ? 1 : -1;

                for(int i=indexI; i!=blocks.GetLength(0)-indexI - (indexI==0?0:1); i+=direction)
                    if (blocks[i,indexJ] != null && blocks[i, indexJ].isDestroyed == false)
                    {
                        indexI = i;
                        break;
                    }

                if(blocks[indexI,indexJ]!=null && blocks[indexI,indexJ].isDestroyed == false)
                {
                    blocks[indexI, indexJ].isDestroyed = true;
                    OM_BlockDestroyed.SendMessage(this.arena, indexI, indexJ);
                }
            }
            else
            {
                int indexI = RandomInstance.instance.Next(blocks.GetLength(0));
                int indexJ = RandomInstance.instance.Next(2) * (blocks.GetLength(1) - 1); // ya ilk ya son element

                int direction = indexJ == 0 ? 1 : -1;

                for (int j = indexJ; j != blocks.GetLength(1) - indexJ - (indexJ == 0 ? 0 : 1); j += direction)
                    if (blocks[indexI, j] != null && blocks[indexI, j].isDestroyed == false)
                    {
                        indexJ = j;
                        break;
                    }

                if (blocks[indexI, indexJ]!=null && blocks[indexI, indexJ].isDestroyed == false)
                {
                    blocks[indexI, indexJ].isDestroyed = true;
                    OM_BlockDestroyed.SendMessage(this.arena, indexI, indexJ);
                }
            }
        }

        /// <summary>
        /// player'in hangi tas uzerinde yurudugunu bulup ona gore block'a ozgu islemi yapan metod.
        /// </summary>
        /// <param name="player"></param>
        public void OnPlayerStep(Player player)
        {
            Tuple<int, int> blockIndex = GetIndexForBlockUnder(player.position);
            Block blockUnder = null;

            if (blockIndex != null)
            {
                blockUnder = blocks[blockIndex.Item1, blockIndex.Item2];
                
                if(blockUnder != null && blockUnder.isDestroyed == false)
                    blockUnder.OnPlayerStep(player);
            }

            if (blockIndex == null || (blockUnder==null || blockUnder.isDestroyed))
                OnPlayerStepOutsideOfMap(player);
        }

        public void OnObstacleStay(Obstacle obstacle)
        {
            Tuple<int, int> blockIndex = GetIndexForBlockUnder(obstacle.position);
            Block blockUnder = null;

            if (blockIndex != null)
            {
                blockUnder = blocks[blockIndex.Item1, blockIndex.Item2];

                if(blockUnder != null && blockUnder.isDestroyed == false)
                    return;
            }

            obstacle.DealDamage(obstacle.health);
        }

        private Tuple<int, int> GetIndexForBlockUnder(Vector3 position)
        {
            float xDistance = position.x - center.x;
            float zDistance = position.z - center.z;

            if( Math.Abs(xDistance) >= width / 2 || Math.Abs(zDistance) >= height / 2)
                return null;

            return new Tuple<int, int>((int)(blocks.GetLength(0)/2f + zDistance / blockSize), (int)(blocks.GetLength(1)/2f + xDistance / blockSize));
        }

        protected virtual void OnPlayerStepOutsideOfMap(Player player)
        {
            LavaBlock lavaBlock = new LavaBlock();
            lavaBlock.OnPlayerStep(player);
        }
    }
}
