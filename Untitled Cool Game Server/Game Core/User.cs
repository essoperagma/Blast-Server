using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetWorker.Host;

namespace Game_Core
{
    public class User
    {
        private static int idGenerator;
        public int id { get; private set; }
        public string username { get; private set; }

        public enum UserState { Idle=0, Queue=1, Game=2 }
        public UserState state;

        public Client client;

        private User(){id = idGenerator++;}

        public User(Client client, string username) : this()   // this() cagirdik, id otomatik gelecek.
        {
            this.client = client;
            this.username = username;
            state = UserState.Idle;
        }
    }
}
