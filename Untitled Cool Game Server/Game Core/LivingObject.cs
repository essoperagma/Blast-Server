using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Core
{
    public abstract class LivingObject
    {
        private static int idGenerator = 0;
        public int id { get; private set; }

        public LivingObject()
        {
            id = idGenerator++;
        }

        public bool awaitsDestruction { get; protected set; }

        public abstract void Update();
    }
}
