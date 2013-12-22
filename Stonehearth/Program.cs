using BobNetProto;
using PegasusGame;
using PegasusShared;
using PegasusUtil;
using Stonehearth.Game;
using Stonehearth.Lobby;
using Stonehearth.Properties;
using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
//using UnityEngine;

namespace Stonehearth
{
    internal static class Program
    {
        public static bool Running = true;

        //private static Dictionary<TAG_CLASS, PegasusShared.CardDef.Builder[]> sHeroLevelRewardCard = new Dictionary<TAG_CLASS, PegasusShared.CardDef.Builder[]>();

        private static LobbyServer sLobbyServer = null;
        private static GameServer sGameServer = null;

        private static void Main()
        {
            if (Process.GetCurrentProcess().MainWindowHandle != IntPtr.Zero)
            {
                Console.SetWindowSize(120, 50);
                Console.BufferHeight = 4000;
            }
            LogManager.OnOutput += OnLogManagerOutput;

            CardManager.Initialize();
            //LoadHeroLevelRewards();
            MatchManager.Initialize();
            PacketHandlerManager.Initialize();


            sLobbyServer = new LobbyServer(Settings.Default.LobbyPort);
            sLobbyServer.OnStarted += s => s.Log(LogManagerLevel.Info, "Started");
            sLobbyServer.OnStopped += s => s.Log(LogManagerLevel.Info, "Stopped");
            sLobbyServer.OnLog += (s, l, f, a) => LogManager.WriteLine(l, f, a);
            sLobbyServer.OnConnect += OnLobbyServerConnected;
            sLobbyServer.OnDisconnect += OnLobbyServerDisconnected;
            sLobbyServer.OnPacket += OnLobbyServerPacket;
            sLobbyServer.Start();

            sGameServer = new GameServer(Settings.Default.GamePort);
            sGameServer.OnStarted += s => s.Log(LogManagerLevel.Info, "Started");
            sGameServer.OnStopped += s => s.Log(LogManagerLevel.Info, "Stopped");
            sGameServer.OnLog += (s, l, f, a) => LogManager.WriteLine(l, f, a);
            sGameServer.OnConnect += OnGameServerConnected;
            sGameServer.OnDisconnect += OnGameServerDisconnected;
            sGameServer.OnPacket += OnGameServerPacket;
            sGameServer.Start();

            LogManager.WriteLine(LogManagerLevel.Info, "Ready");
            while (Running)
            {
                bool sleep = true;
                if (!sLobbyServer.Pulse()) sleep = false;
                if (!sGameServer.Pulse()) sleep = false;
                if (sleep) Thread.Sleep(1);
            }
            sGameServer.Stop();
            sLobbyServer.Stop();
        }

        private static void OnLogManagerOutput(LogManagerLevel pLogLevel, string pOutput)
        {
            switch (pLogLevel)
            {
                case LogManagerLevel.Info: Console.ForegroundColor = ConsoleColor.DarkGreen; break;
                case LogManagerLevel.Warn: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case LogManagerLevel.Error: Console.ForegroundColor = ConsoleColor.DarkRed; break;
                case LogManagerLevel.Exception: Console.ForegroundColor = ConsoleColor.Red; break;
                case LogManagerLevel.Debug: Console.ForegroundColor = ConsoleColor.DarkGray; break;
                default: Console.ForegroundColor = ConsoleColor.White; break;
            }
            Console.WriteLine(pOutput);
        }

        //private static void LoadHeroLevelRewards()
        //{
        //    int heroLevelRewards = 0;
        //    using (SqlConnection db = OpenDB())
        //    {
        //        using (SqlDataReader dr = db.ExecuteReader(null, "SELECT * FROM [HeroLevelReward] ORDER BY [ClassID] ASC, [Level] ASC"))
        //        {
        //            while (dr.Read())
        //            {
        //                PegasusShared.CardDef.Builder[] levelRewardCards = null;
        //                if (!sHeroLevelRewardCard.TryGetValue((TAG_CLASS)(int)dr["ClassID"], out levelRewardCards))
        //                {
        //                    levelRewardCards = new PegasusShared.CardDef.Builder[60];
        //                    sHeroLevelRewardCard.Add((TAG_CLASS)(int)dr["ClassID"], levelRewardCards);
        //                }
        //                PegasusShared.CardDef.Builder builder = PegasusShared.CardDef.CreateBuilder();
        //                builder.SetAsset(CardManager.AllCardsByCardID[(string)dr["CardID"]].AssetID);
        //                builder.SetPremium((int)dr["CardPremium"]);
        //                levelRewardCards[(int)dr["Level"]] = builder;
        //                ++heroLevelRewards;
        //            }
        //        }
        //    }
        //    LogManager.WriteLine(LogManagerLevel.Info, "Loaded {0} Hero Level Rewards", heroLevelRewards);
        //}

        private static void OnLobbyServerConnected(Server<LobbyClient> pServer, LobbyClient pClient)
        {
            pClient.Log(LogManagerLevel.Info, "Connected");

            Random random = new Random();
            pClient.ClientHandle = random.Next();
            //while (!MatchManager.LobbyClientByClientHandle.TryAdd(pClient.ClientHandle, pClient)) pClient.ClientHandle = random.Next();
            while (MatchManager.LobbyClientByClientHandle.ContainsKey(pClient.ClientHandle)) pClient.ClientHandle = random.Next();
            MatchManager.LobbyClientByClientHandle.Add(pClient.ClientHandle, pClient);
        }

        private static void OnLobbyServerDisconnected(Server<LobbyClient> pServer, LobbyClient pClient)
        {
            pClient.Log(LogManagerLevel.Info, "Disconnected");

            //LobbyClient removed = null;
            //MatchManager.LobbyClientByClientHandle.TryRemove(pClient.ClientHandle, out removed);
            MatchManager.LobbyClientByClientHandle.Remove(pClient.ClientHandle);
        }

        private static void OnLobbyServerPacket(Server<LobbyClient> pServer, LobbyClient pClient, Packet pPacket)
        {
            LobbyPacketHandlerAttribute handler = null;
            if (!PacketHandlerManager.LobbyPacketHandlers.TryGetValue(pPacket.Opcode, out handler))
            {
                pClient.Log(LogManagerLevel.Warn, "Unhandled Opcode: {0}", pPacket.Opcode);
                //pClient.Disconnect();
            }
            else if ((handler.ProtocolState & pClient.ProtocolState) == LobbyClientProtocolState.None)
            {
                pClient.Log(LogManagerLevel.Error, "Invalid Protocol State: {0}, {1} requires {2}", pClient.ProtocolState, pPacket.Opcode, handler.ProtocolState);
                pClient.Disconnect();
            }
            else handler.Processor(pClient, pPacket);
        }

        private static void OnGameServerConnected(Server<GameClient> pServer, GameClient pClient)
        {
            pClient.Log(LogManagerLevel.Info, "Connected");
        }

        private static void OnGameServerDisconnected(Server<GameClient> pServer, GameClient pClient)
        {
            pClient.Log(LogManagerLevel.Info, "Disconnected");
        }

        private static void OnGameServerPacket(Server<GameClient> pServer, GameClient pClient, Packet pPacket)
        {
            GamePacketHandlerAttribute handler = null;
            if (!PacketHandlerManager.GamePacketHandlers.TryGetValue(pPacket.Opcode, out handler))
            {
                pClient.Log(LogManagerLevel.Warn, "Unhandled Opcode: {0}", pPacket.Opcode);
                //pClient.Disconnect();
            }
            else if ((handler.ProtocolState & pClient.ProtocolState) == GameClientProtocolState.None)
            {
                pClient.Log(LogManagerLevel.Error, "Invalid Protocol State: {0}, {1} requires {2}", pClient.ProtocolState, pPacket.Opcode, handler.ProtocolState);
                pClient.Disconnect();
            }
            else handler.Processor(pClient, pPacket);
        }
    }
}
