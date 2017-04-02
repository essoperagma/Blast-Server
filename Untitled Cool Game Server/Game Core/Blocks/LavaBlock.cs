using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Core.Utilities;

namespace Game_Core.Blocks
{
    class LavaBlock : Block
    {
        public LavaBlock()
        {
            skinId = 0;
            movementSpeedCoef = 0.7f;
        }

        public override void OnPlayerStep(Player player)
        {
            player.walkSpeedCoefficient = movementSpeedCoef;

            // her saniye caninin yuzde 10 unu goturur
            player.DealDamage(player.maxHealth/10f*Chronos.deltaTime);
        }
    }
}
