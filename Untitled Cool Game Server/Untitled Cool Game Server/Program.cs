

using System;
using Game_Core;

namespace Untitled_Cool_Game_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 0)
                Game.Initialize(args[0], 45998);
            else
                //Game.Initialize("127.0.0.1", 45998);
                Game.Initialize("192.168.1.8", 45998);
            
            Game.Run();
        }
    }
}
