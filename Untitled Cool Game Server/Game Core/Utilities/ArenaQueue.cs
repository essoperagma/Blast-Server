using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Core.Blocks;
using Game_Core.Messages.OutgoingMessages;

namespace Game_Core.Utilities
{
    public class ArenaQueue
    {
        private List<User> users = new List<User>();
        public const int MINIMUM_REQUIRED_PLAYERS = 2;
        public int maxUserCount = 4;

        private const float MAX_QUEUE_WAIT = 7;
        private float remainingWaitTime;

        public void AddUser(User user)
        {
            remainingWaitTime = MAX_QUEUE_WAIT;

            if (users.Contains(user) == false)
            {
                user.state = User.UserState.Queue;
                users.Add(user);
            }

            if (users.Count >= maxUserCount)
            {
                RemoveDisconnected();

                if (users.Count >= maxUserCount)
                {
                    var arena = new Arena(users);
                    Game.StartArena(arena);
                    OM_GameFound.SendMessage(arena);
                    users = new List<User>();
                }
            }
        }

        public bool RemoveUser(User user)
        {
            user.state = User.UserState.Idle;
            return users.Remove(user);
        }

        private void RemoveDisconnected()
        {
            for (int i = users.Count - 1; i >= 0; i--)
                if (Game.clientUserMap.ContainsKey(users[i].client) == false)// eger queue'da beklerken oyundan dusen varsa silelim
                {
                    users[i] = users[users.Count - 1];
                    users.RemoveAt(users.Count-1);
                }
        }

        public void Update()
        {
            if (remainingWaitTime > 0)
                remainingWaitTime -= Chronos.deltaTime;

            if (remainingWaitTime <= 0 && users.Count >= MINIMUM_REQUIRED_PLAYERS)
            {
                RemoveDisconnected();

                if (users.Count >= MINIMUM_REQUIRED_PLAYERS)
                {
                    var arena = new Arena(users);
                    Game.StartArena(arena);
                    OM_GameFound.SendMessage(arena);
                    users = new List<User>();
                }
            }
        }
    }
}
