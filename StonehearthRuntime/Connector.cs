using PegasusShared;
using PegasusUtil;
using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;

namespace StonehearthRuntime
{
    public class Connector : Client, IBattleNet
    {
        private bool mInitialized = false;
        private StreamWriter mLogWriter = null;
        //private DebugConsole mDebugConsole = null;

        private string mHostArgument = string.Empty;
        private ushort mPortArgument = 0;
        private Guid mSessionArgument = Guid.Empty;

        private LobbyClientProtocolState mProtocolState = LobbyClientProtocolState.Handshake;
        private Dictionary<int, PacketHandlerAttribute> mPacketHandlers = new Dictionary<int, PacketHandlerAttribute>();

        private Network.BnetLoginState mBattleNetStatus = Network.BnetLoginState.BATTLE_NET_LOGGED_IN;
        private BattleNet.DllEntityId mAccountID = new BattleNet.DllEntityId();

        private int mGetShutdownMinutes = 0;
        private BattleNet.DllLockouts mGetPlayRestrictions = new BattleNet.DllLockouts() { loaded = true };

        private BnetErrorInfo[] mErrors = new BnetErrorInfo[0];
        private BattleNet.BnetEvent[] mBnetEvents = new BattleNet.BnetEvent[0];
        private BattleNet.DllQueueInfo mQueueInfo = new BattleNet.DllQueueInfo() { position = 0, end = 0, stdev = 100, changed = true };
        private BattleNet.DllFriendsInfo mFriendInfo = new BattleNet.DllFriendsInfo() { friendsSize = 0, updateSize = 0, maxFriends = 50, maxRecvInvites = 50, maxSentInvites = 50 };
        private BattleNet.FriendsUpdate[] mFriendUpdates = new BattleNet.FriendsUpdate[0];
        private BattleNet.DllPartyInfo mPartyInfo = new BattleNet.DllPartyInfo() { size = 0 };
        private BattleNet.PartyEvent[] mPartyEvents = new BattleNet.PartyEvent[0];
        private BattleNet.DllWhisperInfo mWhisperInfo = new BattleNet.DllWhisperInfo() { whisperSize = 0, sendResultsSize = 0 };
        private BnetWhisper[] mWhispers = new BnetWhisper[0];
        private BattleNet.DllChallengeInfo[] mChallenges = new BattleNet.DllChallengeInfo[0];
        private BattleNet.PresenceUpdate[] mPresenceUpdates = new BattleNet.PresenceUpdate[0];

        private LockFreeQueue<BattleNet.QueueEvent> mQueueEvents = new LockFreeQueue<BattleNet.QueueEvent>();
        private LockFreeQueue<PegasusPacket> mUtilPackets = new LockFreeQueue<PegasusPacket>();

        public Connector()
        {
            mLogWriter = new StreamWriter(@"C:\Stonehearth.txt", false);
            mLogWriter.AutoFlush = true;
            Reflector.FindAllMethods<PacketHandlerAttribute, PacketProcessor>(Assembly.GetExecutingAssembly()).ForEach(d => { d.Item1.Processor = d.Item2; mPacketHandlers.Add(d.Item1.Opcode, d.Item1); });
            OnLog += (c, l, f, a) => mLogWriter.WriteLine("<" + l.ToString() + "> " + f, a);
            OnConnect += c =>
            {
                Packet packet = new Packet((int)InternalPacketID.Handshake);
                packet.WriteInt((int)ProtocolVersion.Current);
                packet.WriteBool(true);
                SendPacket(packet);
            };
            OnDisconnect += c => Application.Quit();
            OnPacket += (c, p) =>
            {
                PacketHandlerAttribute handler = null;
                if (!mPacketHandlers.TryGetValue(p.Opcode, out handler))
                {
                    Log(LogManagerLevel.Error, "Invalid Opcode: {0}", p.Opcode);
                    Disconnect();
                }
                else if ((handler.ProtocolState & mProtocolState) == LobbyClientProtocolState.None)
                {
                    Log(LogManagerLevel.Error, "Invalid Protocol State: {0}, {1} requires {2}", mProtocolState, p.Opcode, handler.ProtocolState);
                    Disconnect();
                }
                else handler.Processor(this, p);
            };
            new Thread(() => { while (true) if (Pulse()) Thread.Sleep(1); }).Start();
        }

        [PacketHandler((int)InternalPacketID.Handshake, LobbyClientProtocolState.Handshake)]
        public static void HandshakeHandler(Connector pConnector, Packet pPacket)
        {
            int result = 0;
            if (!pPacket.ReadInt(out result) ||
                result != 0)
            {
                pConnector.Disconnect();
                return;
            }

            pConnector.mProtocolState = LobbyClientProtocolState.Authorize;

            Packet packet = new Packet((int)InternalPacketID.Authorize);
            packet.WriteGuid(pConnector.mSessionArgument);
            pConnector.SendPacket(packet);
        }

        [PacketHandler((int)InternalPacketID.Authorize, LobbyClientProtocolState.Authorize)]
        public static void AuthorizeHandler(Connector pConnector, Packet pPacket)
        {
            int result = 0;
            long accountID = 0;
            if (!pPacket.ReadInt(out result) ||
                result != 0 ||
                !pPacket.ReadLong(out accountID))
            {
                pConnector.Disconnect();
                return;
            }

            pConnector.mAccountID.hi = 144115193835963207; // 144115193835963207;
            pConnector.mAccountID.lo = (ulong)accountID;

            pConnector.mProtocolState = LobbyClientProtocolState.Online;
        }

        [PacketHandler((int)InternalPacketID.UtilPacket)]
        public static void UtilPacketHandler(Connector pConnector, Packet pPacket)
        {
            int packetId = 0;
            int context = 0;
            byte[] data = null;
            if (!pPacket.ReadInt(out packetId) ||
                !pPacket.ReadInt(out context) ||
                !pPacket.ReadBytes(out data))
            {
                pConnector.Disconnect();
                return;
            }

            pConnector.mUtilPackets.Enqueue(new PegasusPacket(packetId, context, data));
        }

        [PacketHandler((int)InternalPacketID.StartScenario)]
        public static void StartScenarioHandler(Connector pConnector, Packet pPacket)
        {
            int result = 0;
            string address = null;
            ushort port = 0;
            string password = null;
            int gameHandle = 0;
            long clientHandle = 0;
            string version = null;
            if (!pPacket.ReadInt(out result) ||
                result != 0 ||
                !pPacket.ReadString(out address) ||
                !pPacket.ReadUShort(out port) ||
                !pPacket.ReadString(out password) ||
                !pPacket.ReadInt(out gameHandle) ||
                !pPacket.ReadLong(out clientHandle) ||
                !pPacket.ReadString(out version))
            {
                pConnector.Disconnect();
                return;
            }

            BattleNet.GameServerInfo gameServerInfo = new BattleNet.GameServerInfo()
            {
                Address = address,
                Port = port,
                AuroraPassword = password,
                GameHandle = gameHandle,
                ClientHandle = clientHandle,
                Version = version,
            };

            pConnector.mQueueEvents.Enqueue(new BattleNet.QueueEvent(BattleNet.QueueEvent.Type.QUEUE_GAME_STARTED, 0, 0, 0, gameServerInfo));
        }


        // IBattleNet

        public bool Init(bool fromEditor)
        {
            if (mInitialized) return true;
            //mDebugConsole = ApplicationMgr.Get().gameObject.AddComponent<DebugConsole>();

            mLogWriter.WriteLine("-- Init: {0}", fromEditor);

            string commandLine = Win32API.GetCommandLineA();
            int startOfArg = 0;
            char terminatorOfArg = ' ';
            if (commandLine[0] == '"')
            {
                terminatorOfArg = '"';
                startOfArg = 1;
            }
            int endOfArg = commandLine.IndexOf(terminatorOfArg, startOfArg) + 1;
            commandLine = commandLine.Substring(endOfArg).Trim();
            string[] commandLineArgs = commandLine.Split(' ');
            foreach (string commandLineArg in commandLineArgs)
            {
                if (commandLineArg.StartsWith("-host=")) mHostArgument = commandLineArg.Remove(0, "-host=".Length);
                else if (commandLineArg.StartsWith("-port=")) mPortArgument = Convert.ToUInt16(commandLineArg.Remove(0, "-port=".Length));
                else if (commandLineArg.StartsWith("-session=")) mSessionArgument = new Guid(commandLineArg.Remove(0, "-session=".Length));
            }

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(mHostArgument, mPortArgument);
            Connect(socket);

            mLogWriter.WriteLine("-- Initialized: {0}:{1}, {2}", mHostArgument, mPortArgument, mSessionArgument);

            return (mInitialized = ConnectAPI.ConnectInit());
        }

        public bool IsInitialized() { return mInitialized; }
        public void AppQuit() { mLogWriter.Close(); }
        public void ApplicationWasPaused() { }
        public void ApplicationWasUnpaused() { }
        public int BattleNetStatus() { return (int)mBattleNetStatus; }
        public BattleNet.DllEntityId GetMyGameAccountId() { return mAccountID; }
        public string GetAccountCountry() { return "USA"; }
        public int GetAccountRegion() { return (int)Network.BnetRegion.REGION_US; }
        public int GetCurrentRegion() { return (int)Network.BnetRegion.REGION_US; }
        public BattleNetLogSource GetLogSource() { mLogWriter.WriteLine("** GetLogSource"); return null; }
        public string GetLaunchOption(string key) { mLogWriter.WriteLine("** GetLaunchOption: {0}", key); return null; }
        public int GetShutdownMinutes() { return mGetShutdownMinutes; }
        public void ProvideWebAuthToken(string token) { mLogWriter.WriteLine("** ProvideWebAuthToken: {0}", token); }
        public bool CheckWebAuth(out string url) { mLogWriter.WriteLine("** CheckWebAuth"); url = null; return false; }
        public string GetStoredBNetIPAddress() { mLogWriter.WriteLine("** GetStoredBNetIPAddress"); return null; }
        public void RequestCloseAurora() { mLogWriter.WriteLine("** RequestCloseAurora"); }
        public void CloseAurora() { mLogWriter.WriteLine("** CloseAurora"); }
        public void QueryAurora() { mLogWriter.WriteLine("** QueryAurora"); }
        public void ProcessAurora() { }
        public PegasusPacket NextUtilPacket() { return mUtilPackets.Dequeue(); }
        public void SendUtilPacket(int packetId, int systemId, byte[] bytes, int size, int subID, int context)
        {
            //mLogWriter.WriteLine("-- SendUtilPacket: {0}, {1}, {2}, {3}, {4}", packetId, systemId, size, subID, context);
            Packet packet = new Packet((int)InternalPacketID.UtilPacket);
            packet.WriteInt(packetId);
            packet.WriteInt(context);
            packet.WriteBytes(bytes);
            SendPacket(packet);
        }


        public void ClearErrors() { Array.Resize(ref mErrors, 0); }
        public int GetErrorsCount() { return mErrors.Length; }
        public void GetErrors(BnetErrorInfo[] errors) { for (int index = 0; index < errors.Length; ++index) errors[index] = mErrors[index]; }


        public void ClearBnetEvents() { Array.Resize(ref mBnetEvents, 0); }
        public int GetBnetEventsSize() { return mBnetEvents.Length; }
        public void GetBnetEvents(BattleNet.BnetEvent[] events) { for (int index = 0; index < events.Length; ++index) events[index] = mBnetEvents[index]; }


        public void GetQueueInfo(ref BattleNet.DllQueueInfo queueInfo) { queueInfo = mQueueInfo; mQueueInfo.changed = false; }
        public BattleNet.QueueEvent GetQueueEvent() { return mQueueEvents.Dequeue(); }


        public void ClearPartyUpdates() { Array.Resize(ref mPartyEvents, 0); mPartyInfo.size = 0; }
        public void GetPartyUpdatesInfo(ref BattleNet.DllPartyInfo info) { mPartyInfo.size = mPartyEvents.Length; info = mPartyInfo; }
        public void GetPartyUpdates(BattleNet.PartyEvent[] updates) { for (int index = 0; index < updates.Length; ++index) updates[index] = mPartyEvents[index]; }
        public void SendPartyInvite(ref BattleNet.DllEntityId gameAccount)
        {
            mLogWriter.WriteLine("** SendPartyInvite");
            throw new NotImplementedException();
        }
        public void RescindPartyInvite(ref BattleNet.DllEntityId partyId)
        {
            mLogWriter.WriteLine("** RescindPartyInvite");
            throw new NotImplementedException();
        }
        public void AcceptPartyInvite(ref BattleNet.DllEntityId partyId)
        {
            mLogWriter.WriteLine("** AcceptPartyInvite");
            throw new NotImplementedException();
        }
        public void DeclinePartyInvite(ref BattleNet.DllEntityId partyId)
        {
            mLogWriter.WriteLine("** DeclinePartyInvite");
            throw new NotImplementedException();
        }
        public void SetPartyDeck(ref BattleNet.DllEntityId partyId, long deckID)
        {
            mLogWriter.WriteLine("** SetPartyDeck");
            throw new NotImplementedException();
        }


        public void ClearFriendsUpdates() { Array.Resize(ref mFriendUpdates, 0); mFriendInfo.friendsSize = 0; mFriendInfo.updateSize = 0; }
        public void GetFriendsInfo(ref BattleNet.DllFriendsInfo info) { mFriendInfo.friendsSize = mFriendUpdates.Length; mFriendInfo.updateSize = mFriendUpdates.Length; info = mFriendInfo; }
        public void GetFriendsUpdates(BattleNet.FriendsUpdate[] updates) { for (int index = 0; index < updates.Length; ++index) updates[index] = mFriendUpdates[index]; }
        public void SendFriendInvite(string inviter, string invitee, bool byEmail) { mLogWriter.WriteLine("** SendFriendInvite: {0}, {1}, {2}", inviter, invitee, byEmail); }
        public void ManageFriendInvite(int action, ulong inviteId) { mLogWriter.WriteLine("** ManageFriendInvite: {0}, {1}", action, inviteId); }
        public void RemoveFriend(ref BnetAccountId account) { mLogWriter.WriteLine("** RemoveFriend: {0}", account); }


        public void ClearWhispers() { Array.Resize(ref mWhispers, 0); mWhisperInfo.whisperSize = 0; }
        public void GetWhisperInfo(ref BattleNet.DllWhisperInfo info) { mWhisperInfo.whisperSize = mWhispers.Length; info = mWhisperInfo; }
        public void GetWhispers(BnetWhisper[] whispers) { for (int index = 0; index < whispers.Length; ++index) whispers[index] = mWhispers[index]; }
        public string FilterProfanity(string unfiltered) { return unfiltered; }
        public void SendWhisper(BnetGameAccountId gameAccount, string message) { mLogWriter.WriteLine("** SendWhisper: {0}, {1}", gameAccount, message); }


        public void ClearChallenges() { Array.Resize(ref mChallenges, 0); }
        public int NumChallenges() { return mChallenges.Length; }
        public void GetChallenges(BattleNet.DllChallengeInfo[] challenges) { for (int index = 0; index < challenges.Length; ++index) challenges[index] = mChallenges[index]; }
        public void AnswerChallenge(ulong challengeID, string answer) { mLogWriter.WriteLine("** AnswerChallenge: {0}, {1}", challengeID, answer); }
        public void CancelChallenge(ulong challengeID) { mLogWriter.WriteLine("** CancelChallenge: {0}", challengeID); }


        public void GetPlayRestrictions(ref BattleNet.DllLockouts restrictions, bool reload) { mLogWriter.WriteLine("-- GetPlayRestrictions: {0}", reload); restrictions = mGetPlayRestrictions; }
        public void DraftQueue(bool join) { mLogWriter.WriteLine("** DraftQueue: {0}", join); }
        public void RankedMatch(long deckID) { mLogWriter.WriteLine("** RankedMatch: {0}", deckID); }
        public void UnrankedMatch(long deckID, bool newbie) { mLogWriter.WriteLine("** UnrankedMatch: {0}, {1}", deckID, newbie); }
        public void StartScenarioAI(int scenario, long deckID, long aiDeckID) { mLogWriter.WriteLine("** StartScenarioAI: {0}, {1}, {2}", scenario, deckID, aiDeckID); }
        public void StartScenario(int scenario, long deckID)
        {
            mLogWriter.WriteLine("** StartScenario: {0}, {1}", scenario, deckID);

            mQueueEvents.Enqueue(new BattleNet.QueueEvent(BattleNet.QueueEvent.Type.QUEUE_ENTER));

            Packet startScenario = new Packet((int)InternalPacketID.StartScenario);
            startScenario.WriteInt(scenario);
            startScenario.WriteLong(deckID);
            SendPacket(startScenario);
        }


        public void ClearPresence() { Array.Resize(ref mPresenceUpdates, 0); }
        public int PresenceSize() { return mPresenceUpdates.Length; }
        public void GetPresence(BattleNet.PresenceUpdate[] updates) { for (int index = 0; index < updates.Length; ++index) updates[index] = mPresenceUpdates[index]; }
        public void SetPresenceBlob(uint field, byte[] val) { }
        public void SetPresenceBool(uint field, bool val) { }
        public void SetPresenceInt(uint field, long val) { }
        public void SetPresenceString(uint field, string val) { }
        public void SetRichPresence(BattleNet.DllRichPresenceUpdate[] updates)
        {
            //mLogWriter.WriteLine("?? SetRichPresence: {0}", updates.Length);
            //foreach (BattleNet.DllRichPresenceUpdate update in updates)
            //{
            //    if (update.presenceFieldIndex == 0 && update.index == 9)
            //    {
            //        int start = mGetPresence.Length;
            //        Array.Resize(ref mGetPresence, mGetPresence.Length + 2);
            //        mGetPresence[start].entityId = AccountID; 
            //        mGetPresence[start].programId = 16974;
            //        mGetPresence[start].groupId = 2;
            //        mGetPresence[start].fieldId = 1000;
            //        mGetPresence[start].stringVal = "Making a deck";
            //        mGetPresence[start + 1].entityId = AccountID; 
            //        mGetPresence[start + 1].programId = 16974;
            //        mGetPresence[start + 1].groupId = 2;
            //        mGetPresence[start + 1].fieldId = 9;
            //        mGetPresence[start + 1].intVal = 1386103683688662;
            //    }
            //    else if (update.presenceFieldIndex == 0 && update.index == 8)
            //    {
            //        int start = mGetPresence.Length;
            //        Array.Resize(ref mGetPresence, mGetPresence.Length + 2);
            //        mGetPresence[start].entityId = AccountID; 
            //        mGetPresence[start].programId = 16974;
            //        mGetPresence[start].groupId = 2;
            //        mGetPresence[start].fieldId = 1000;
            //        mGetPresence[start].stringVal = "Browsing the collection";
            //        mGetPresence[start + 1].entityId = AccountID; 
            //        mGetPresence[start + 1].programId = 16974;
            //        mGetPresence[start + 1].groupId = 2;
            //        mGetPresence[start + 1].fieldId = 9;
            //        mGetPresence[start + 1].intVal = 1386103678684067;

            //    }
            //}
        }
    }
}
