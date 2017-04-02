using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Core.Blocks
{
    public interface IBlock
    {
        void OnPlayerStep(Player player);
    }
}
