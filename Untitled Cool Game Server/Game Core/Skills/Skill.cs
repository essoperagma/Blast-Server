using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Core.Utilities;

namespace Game_Core.Skills
{
    public abstract class Skill
    {
        protected Player player;

        public virtual int UnlockPrice{ get { return 1; }}
        public abstract void Update();
        public abstract bool IsActivatable(Vector3 activationLocation);
        public abstract void ActivateAndNotify(Vector3 activationLocation);
    }
}