using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Core.Blocks
{
    public abstract class Block : IBlock
    {
        public int skinId;
        public bool isDestroyed;
        protected float movementSpeedCoef;

        /// <summary>
        /// Called when a player is walking on this thing
        /// </summary>
        public abstract void OnPlayerStep(Player player);
    }
}
