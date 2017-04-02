using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using GameCore.Messages;
using Game_Core.Blocks;
using Game_Core.Utilities;
using NetWorker.EventArgs;
using NetWorker.Host;
using NetWorker.Utilities;

namespace Game_Core
{
    public static class Game
    {
        public static Dictionary<Client, User> clientUserMap = new Dictionary<Client, User>();
        public static Dictionary<Client, Player> clientPlayerMap = new Dictionary<Client, Player>();
        private static List<Arena> arenas = new List<Arena>(); 

        private static ConcurrentQueue<Client> justArrivedClients = new ConcurrentQueue<Client>(); 
        private static ConcurrentQueue<Client> justLeftClients = new ConcurrentQueue<Client>(); 
        private volatile static ConcurrentQueue<MessageArrivedEventArgs> arrivedMessages = new ConcurrentQueue<MessageArrivedEventArgs>(); 
        private static ConcurrentQueue<MessageArrivedEventArgs> tmpArrivedMessages = new ConcurrentQueue<MessageArrivedEventArgs>(); 

        public static Server server;
        public static ArenaQueue queue;

        public static long frameNumber { get; private set; }

        public static void Initialize(string ip, int port)
        {
            queue = new ArenaQueue();
            server = new Server();
            server.clientArrivedEvent += ServerOnClientArrivedEvent;
            server.clientRemovedEvent += ServerOnClientRemovedEvent;
            server.messageArrivedEvent += ServerOnMessageArrivedEvent;

            server.StartServer(ip,port);
        }
        
        public static void Run()
        {
            Chronos.setInterval(50);      // sunucu dongusunu her 50 milisaniyede tamamlayacak.
            
            while (true)
            {
                try
                {
                    while (true)
                    {
                        Chronos.waitForTheRightMoment();    // fps yi ayarliyor.                 
                        frameNumber++;

                        Client client;
                        
                        while (justLeftClients.TryDequeue(out client))
                        {
                            if(clientUserMap.ContainsKey(client))
                                clientUserMap.Remove(client);
                            if(clientPlayerMap.ContainsKey(client))
                                clientPlayerMap.Remove(client);
                        }

                        //var tmp = arrivedMessages;  // mesajlari alip queue'ya atan koda yeni bir concurrentQueue verelim. o onu doldururken biz bunu bosaltalim.
                        //Interlocked.Exchange<ConcurrentQueue<MessageArrivedEventArgs>>(ref arrivedMessages, tmpArrivedMessages);    // TODO buranin fail olmayacagina emin ol.
                        //tmpArrivedMessages = tmp;
                        tmpArrivedMessages = arrivedMessages;

                        int maxMessagesPerIteration = 50;
                        MessageArrivedEventArgs messageArgs;
                        while(tmpArrivedMessages.TryDequeue(out messageArgs) && maxMessagesPerIteration-->0)
                        {
                            RawMessage message = messageArgs.message;
                            if (message.containsField("messageTypeId") == false)
                                continue;

                            IIncomingMessage incomingMessage;
                            if (TypeIdGenerator.incomingMessageIds.TryGetValue(message.getInt("messageTypeId"),out incomingMessage))
                            {
                                //Console.WriteLine(incomingMessage.GetType().Name);
                                incomingMessage.processMessage(messageArgs);
                            }
                        }

                        foreach(var arena in arenas)
                            arena.Update();
                        arenas.RemoveAll(a => a.awaitsDestruction);

                        queue.Update();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString() + "\nStack Trace:\n" + e.StackTrace);
                }

            }
        }

        private static void ServerOnMessageArrivedEvent(object sender, MessageArrivedEventArgs messageArrivedEventArgs)
        {
            arrivedMessages.Enqueue(messageArrivedEventArgs);
        }

        private static void ServerOnClientRemovedEvent(object sender, ClientEventArgs clientEventArgs)
        {
            justLeftClients.Enqueue(clientEventArgs.client);
            Console.WriteLine("Somebody has disconnected");
        }

        private static void ServerOnClientArrivedEvent(object sender, ClientEventArgs clientEventArgs)
        {
            clientEventArgs.client.ListenForMessages();
            // simdilik bir ise yaramiyor gelen clientlari tutmak. login mesaji alana kadar umursamiyoruz sayilarini
            //justArrivedClients.Enqueue(clientEventArgs.client);
            Console.WriteLine("Somebody is connected");
        }

        public static void StartArena(Arena arena)
        {
            arenas.Add(arena);
        }
    }
}
