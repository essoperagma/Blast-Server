using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Core.Messages.OutgoingMessages;
using Game_Core.Missiles;
using Game_Core.Obstacles;
using Game_Core.Utilities;
using NetWorker.Host;

namespace Game_Core.Blocks
{
    public class Arena : LivingObject
    {
        public List<Player> players = new List<Player>();
        public List<Missile> missiles = new List<Missile>();
        public List<Obstacle> obstacles = new List<Obstacle>();
        public Land land;

        public enum ArenaState { WaitingPlayersReady, WaitingPlayersLoading, StartCountdown, Battle, RoundOver, BattleOver }
        public ArenaState state { get; private set; }

        public int totalRounds = 3;
        public int currentRound = 0;
        public RoundStats roundStats;

        private float nextLandShrinkTime = 5f;

        private const float MAX_READY_DURATION = 10f;
        private float remainingReadyTime;

        private const float MAX_LOADING_DURATION = 15f;
        private float remainingLoadingTime;

        private const float START_COUNTDOWN_DURATION = 3f;
        private float remainingCountdownTime;

        private const float ROUND_OVER_DURATION = 8f;
        private float remainingRoundOverTime;

        public Arena()
        {
            remainingReadyTime = MAX_READY_DURATION;
            CreateLandAndObstaclesFromFile("Data/Maps/-57604.arena");
        }

        public Arena(List<User> users) :this()
        {
            foreach (var user in users)
            {
                user.state = User.UserState.Game;
                players.Add( new Player(user,this));
            }
            foreach (var player in players)
                Game.clientPlayerMap.Add(player.user.client,player);

            SetPlayerInitialPositions();
            roundStats = new RoundStats(players);
        }

        public override void Update()
        {
            if (awaitsDestruction)
                return;

            if (players.Any(p => Game.clientUserMap.ContainsKey(p.user.client) == true) == false ) // herkes disconnected ya da players.count == 0
                awaitsDestruction = true;
            else if (state == ArenaState.WaitingPlayersReady)
                CheckForReadyTimeout();
            else if(state == ArenaState.WaitingPlayersLoading)
                CheckForLoadingTimeout();
            else if (state == ArenaState.StartCountdown)
            {
                remainingCountdownTime -= Chronos.deltaTime;
                if (remainingCountdownTime <= 0)
                {
                    state = ArenaState.Battle;
                    OM_BattleStart.SendMessage(this);
                }
            }
            else if (state == ArenaState.RoundOver)
            {
                remainingRoundOverTime -= Chronos.deltaTime;
                if (remainingRoundOverTime <= 0)
                {
                    currentRound++;

                    if (currentRound < totalRounds)
                    {
                        PrepareForNextRound();
                        state = ArenaState.WaitingPlayersLoading;
                        OM_LoadingNextRoundStarted.SendMessage(this);
                        foreach (var p in players)
                            p.state = Player.PlayerState.Loading;
                    }
                    else
                    {
                        state= ArenaState.BattleOver;
                        OM_BattleOver.SendMessage(this);
                    }
                }
            }
            else if (state == ArenaState.Battle)
            {
                foreach (var p in players)
                    p.Update();
                players.RemoveAll(p => p.awaitsDestruction);

                foreach(var m in missiles)
                    m.Update();
                missiles.RemoveAll(m => m.awaitsDestruction);

                foreach(var o in obstacles)
                    o.Update();
                obstacles.RemoveAll(o => o.awaitsDestruction);

                foreach(var p in players)
                    land.OnPlayerStep(p);

                foreach (var o in obstacles)
                    land.OnObstacleStay(o);

                CheckForLandShrink();
                CheckIsGameOver();
            }
        }

        private void CreateLandAndObstacles()
        {
            land = new Land(this, 20, 12, 1);

            obstacles.Clear();
            for (int i = 0; i < 4; i++)
            {
                int wDivisions = (int)(land.width / land.blockSize);
                int hDivisions = (int)(land.height / land.blockSize);

                Vector3 oPos = new Vector3(
                        (RandomInstance.instance.Next(wDivisions) + 0.5f) * land.blockSize + Land.center.x - land.width / 2f, 0,
                        (RandomInstance.instance.Next(hDivisions) + 0.5f) * land.blockSize + Land.center.z - land.height / 2f);

                obstacles.Add(new Obstacle(this, oPos, 40f * (1 + RandomInstance.instance.Next(26) / 100f), 0.17f * (1 + RandomInstance.instance.Next(26) / 100f)));
            }
        }

        private void CreateLandAndObstaclesFromFile(string filepath)
        {
            obstacles.Clear();
            
            using (BinaryReader reader = new BinaryReader(new FileStream(filepath, FileMode.Open)))
            {
                int wC = reader.ReadInt32();
                int hC = reader.ReadInt32();
                int bCount = reader.ReadInt32(); // blockCount 'u okuduk. fuzuli

                land = new Land(this,wC,hC,1);
                
                for (int i = 0; i < hC; i++)
                    for (int j = 0; j < wC; j++)
                    {
                        string typename = reader.ReadString();
                        int skinId = reader.ReadInt32();

                        Type blockType;
                        if ((blockType = Type.GetType("Game_Core.Blocks." + typename)) != null)
                        {
                            land.blocks[i, j] = Activator.CreateInstance(blockType) as Block;
                            land.blocks[i, j].skinId = skinId;
                        }
                        else
                            land.blocks[i, j] = null;
                    }

                int obsCount = reader.ReadInt32();

                for (int i = 0; i < obsCount; i++)
                {
                    string typename = reader.ReadString();
                    int skinId = reader.ReadInt32();
                    Vector3 position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                    Type obsType;
                    if ((obsType = Type.GetType("Game_Core.Obstacles." + typename)) != null)
                        obstacles.Add(Activator.CreateInstance(obsType,this,position,40f,0.15f,skinId) as Obstacle);
                
                }
            }
    
        }

        private void CheckForReadyTimeout()
        {
            if (remainingReadyTime > 0)
                remainingReadyTime -= Chronos.deltaTime;

            if (remainingReadyTime <= 0)
            {
                Player notReady = players.FirstOrDefault(p => p.state == Player.PlayerState.AnsweringReady);
                PlayerRejectedReady(notReady);
            }
        }

        private void CheckForLoadingTimeout()
        {
            if(remainingLoadingTime>0)
                remainingLoadingTime -= Chronos.deltaTime;

            if (remainingLoadingTime <= 0)
            {
                List<Player> calculatorUsers = new List<Player>(players.Count);
                calculatorUsers.AddRange(players.Where(p => p.state == Player.PlayerState.Loading));
                calculatorUsers.ForEach( PlayerKick );

                PlayerDoneLoading(null);
            }
        }

        private void CheckForLandShrink()
        {
            nextLandShrinkTime -= Chronos.deltaTime;
            if (nextLandShrinkTime <= 0)
            {
                land.ShrinkSingleFromBorders();
                nextLandShrinkTime = RandomInstance.instance.Next( 60, 100)/100f;// 1 ile 2 sn arasi
            }
        }

        private void CheckIsGameOver()
        {
            if (players.Count(p => p.IsAlive) <= 1) // herkes ölü
            {
                state = ArenaState.RoundOver;
                remainingRoundOverTime = (currentRound != totalRounds-1 ? ROUND_OVER_DURATION : 2);
                roundStats.FinalizeStats(this);
                OM_RoundOver.SendMessage(this);
            }
        }

        public void PlayerAcceptedReady(Player player)
        {
            player.state = Player.PlayerState.WaitingOthersReady;

            bool allReady = true;
            foreach (var p in players)
            {
                if (p.state == Player.PlayerState.AnsweringReady)
                    allReady = false;
            }

            if (allReady)
            {
                remainingLoadingTime = MAX_LOADING_DURATION;
                state = ArenaState.WaitingPlayersLoading;
                OM_LoadingStateStarted.SendMessage(this);
                foreach(var p in players)
                    p.state = Player.PlayerState.Loading;
            }
        }

        public void PlayerRejectedReady(Player player)
        {
            awaitsDestruction = true;

            foreach (var p in players)
            {
                p.user.state = User.UserState.Queue;
                Game.clientPlayerMap.Remove(p.user.client);
            }

            if(player!=null)
            {
                player.user.state = User.UserState.Idle;
                OM_GameReadyRejected.SendMessage(player);
            }

            foreach(var p in players.Where(i=>i!=player))
                Game.queue.AddUser(p.user);
            OM_ReturnToQueue.SendMessage(players,player);
        }

        public void PlayerDoneLoading(Player player)
        {
            if(player != null)
                player.state = Player.PlayerState.WaitingOthersLoading;

            bool allLoaded = true;

            foreach (var p in players)
                if (p.state == Player.PlayerState.Loading)
                    allLoaded = false;

            if (allLoaded && state == ArenaState.WaitingPlayersLoading)
            {
                state = ArenaState.StartCountdown;
                remainingCountdownTime = START_COUNTDOWN_DURATION;
                foreach(var p in players)
                    p.state = Player.PlayerState.InGame;

                OM_StartCountdown.SendMessage(this, START_COUNTDOWN_DURATION);
            }
        }

        private void SetPlayerInitialPositions()
        {
            for (int i = 0; i < players.Count; i++)
            {
                double angle = Math.PI*(360f/players.Count)/180.0;

                float x = (float) Math.Cos(angle*i)*land.width/2*0.5f;
                float z = (float) Math.Sin(angle*i)*land.height/2*0.5f;

                players[i].position = new Vector3(x,0,z);
            }
        }

        public void SpawnMissile(Missile missile)
        {
            missiles.Add(missile);
        }

        public void PlayerKick(Player player)
        {
            if(!players.Contains(player))
                return;

            if ((state == ArenaState.Battle || state == ArenaState.WaitingPlayersLoading) && player.IsAlive)
            {
                player.DealDamage(player.health);
                OM_PlayerHealthInfo.SendMessage(this,player);
            }

            player.user.state = User.UserState.Idle;
            player.Destroy();
            players.Remove(player);
            Game.clientPlayerMap.Remove(player.user.client);
            OM_PlayerExitGame.SendMessage(player);
        }

        private void PrepareForNextRound()
        {
            roundStats = new RoundStats(players,roundStats);
            CreateLandAndObstaclesFromFile("Data/Maps/-57604.arena");
            
            foreach (var player in players) 
                player.PrepareForRound();
            SetPlayerInitialPositions();

            remainingLoadingTime = MAX_LOADING_DURATION;
        }
    }
}
