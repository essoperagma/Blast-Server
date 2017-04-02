using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Core.Utilities;

namespace Game_Core.Blocks
{
    class StoneBlock : Block
    {
        private const int skinCount = 16;   // 4 block * 4 rotations
        public StoneBlock()
        {
            skinId = RandomInstance.instance.Next(skinCount);
            movementSpeedCoef = 1f;
        }

        public override void OnPlayerStep(Player player)
        {
            player.walkSpeedCoefficient = movementSpeedCoef;
        }
    }
}
