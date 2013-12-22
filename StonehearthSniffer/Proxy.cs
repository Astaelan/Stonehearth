using PacketDotNet;
using PegasusUtil;
using SharpPcap;
using StonehearthCommon;
using StonehearthCommon.DumpFrames;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthSniffer
{
    public class Proxy : IBattleNet
    {
        private BattleNetDll DLL = new BattleNetDll();
        private BinaryWriter DumpWriter = null;
        private ICaptureDevice CaptureDevice = null;
        private TCPStack CaptureStack = new TCPStack();

        public Proxy()
        {
            string capturePath = Path.GetDirectoryName((new Uri(GetType().Assembly.CodeBase)).AbsolutePath) + Path.DirectorySeparatorChar + "Captures";
            if (!Directory.Exists(capturePath)) Directory.CreateDirectory(capturePath);
            capturePath += Path.DirectorySeparatorChar + Process.GetCurrentProcess().StartTime.ToFileTime().ToString();
            DumpWriter = new BinaryWriter(new FileStream(capturePath + ".bin", FileMode.Create, FileAccess.Write), Encoding.UTF8);
            CaptureDevice = CaptureDeviceList.Instance.FirstOrDefault();
            CaptureDevice.Open();
            CaptureDevice.OnCaptureStopped += CaptureDevice_OnCaptureStopped;
            CaptureDevice.OnPacketArrival += CaptureDevice_OnPacketArrival;
        }

        private void CaptureDevice_OnCaptureStopped(object sender, CaptureStoppedEventStatus status)
        {
            DumpWriter.Write((int)DumpFrameType.GameFinished);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new GameFinishedFrame()
            {
                Filter = CaptureDevice.Filter,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);
        }

        private void CaptureDevice_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            TcpPacket packet = (TcpPacket)PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data).Extract(typeof(TcpPacket));
            TCPStream stream = CaptureStack.Push(packet);
            bool done = false;
            while (!done)
            {
                int packetID = 0;
                int size = 0;
                if (!stream.PeekInt32(true, 0, out packetID) ||
                    !stream.PeekInt32(true, 4, out size) ||
                    stream.Length < (size + 8))
                {
                    done = true;
                    break;
                }

                DumpWriter.Write(stream.Owner == TCPStream.TCPStreamOwner.Client ? (int)DumpFrameType.ClientGamePacket : (int)DumpFrameType.ServerGamePacket);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                DumpWriter.Write(packetID);
                DumpWriter.Write(size);
                DumpWriter.Write(stream.Data, 8, size);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);

                stream.Consume(size + 8);
            }
            if (packet.Fin || packet.Rst) CaptureDevice.StopCapture();
        }


        // IBattleNet

        private bool mInit = false;
        public bool Init(bool fromEditor)
        {
            bool result = DLL.Init(fromEditor);

            if (mInit != result)
            {
                DumpWriter.Write((int)DumpFrameType.Init);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new InitFrame()
                {
                    Return = result,
                    FromEditor = fromEditor,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);

                mInit = result;
            }

            return result;
        }

        private bool mIsInitialized = false;
        public bool IsInitialized()
        {
            bool result = DLL.IsInitialized();

            if (mIsInitialized != result)
            {
                DumpWriter.Write((int)DumpFrameType.IsInitialized);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new IsInitializedFrame()
                {
                    Return = result,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);

                mIsInitialized = result;
            }

            return result;
        }

        public void AppQuit()
        {
            DumpWriter.Write((int)DumpFrameType.AppQuit);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new AppQuitFrame()
            {
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            if (CaptureDevice.Started) CaptureDevice.StopCapture();
            CaptureDevice.Close();
            DumpWriter.Close();
            DLL.AppQuit();
        }

        public void ApplicationWasPaused()
        {
            DumpWriter.Write((int)DumpFrameType.ApplicationWasPaused);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new ApplicationWasPausedFrame()
            {
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.ApplicationWasPaused();
        }

        public void ApplicationWasUnpaused()
        {
            DumpWriter.Write((int)DumpFrameType.ApplicationWasUnpaused);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new ApplicationWasUnpausedFrame()
            {
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.ApplicationWasUnpaused();
        }

        public string GetLaunchOption(string key)
        {
            string result = DLL.GetLaunchOption(key);

            DumpWriter.Write((int)DumpFrameType.GetLaunchOption);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new GetLaunchOptionFrame()
            {
                Return = result,
                Key = key,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            return result;
        }

        public BattleNetLogSource GetLogSource() { return DLL.GetLogSource(); }

        private string mStoredBNetIPAddress = null;
        public string GetStoredBNetIPAddress()
        {
            string result = DLL.GetStoredBNetIPAddress();
            if (mStoredBNetIPAddress != result)
            {
                DumpWriter.Write((int)DumpFrameType.GetStoredBNetIPAddress);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetStoredBNetIPAddressFrame()
                {
                    Return = result,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);

                mStoredBNetIPAddress = result;
            }
            return result;
        }

        public void RequestCloseAurora()
        {
            DumpWriter.Write((int)DumpFrameType.RequestCloseAurora);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new RequestCloseAuroraFrame()
            {
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.RequestCloseAurora();
        }

        public void CloseAurora()
        {
            DumpWriter.Write((int)DumpFrameType.CloseAurora);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new CloseAuroraFrame()
            {
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.CloseAurora();
        }

        public void QueryAurora()
        {
            DumpWriter.Write((int)DumpFrameType.QueryAurora);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new QueryAuroraFrame()
            {
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.QueryAurora();
        }

        public void ProcessAurora() { DLL.ProcessAurora(); }

        public void ProvideWebAuthToken(string token)
        {
            DumpWriter.Write((int)DumpFrameType.ProvideWebAuthToken);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new ProvideWebAuthTokenFrame()
            {
                Token = token,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.ProvideWebAuthToken(token);
        }

        public bool CheckWebAuth(out string url)
        {
            bool result = DLL.CheckWebAuth(out url);

            DumpWriter.Write((int)DumpFrameType.CheckWebAuth);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new CheckWebAuthFrame()
            {
                Return = result,
                Url = url,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            return result;
        }

        private int mShutdownMinutes = 0;
        public int GetShutdownMinutes()
        {
            int result = DLL.GetShutdownMinutes();
            if (mShutdownMinutes != result)
            {
                DumpWriter.Write((int)DumpFrameType.GetShutdownMinutes);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetShutdownMinutesFrame()
                {
                    Return = result,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);

                mShutdownMinutes = result;
            }
            return result;
        }

        private int mBattleNetStatus = 0;
        public int BattleNetStatus()
        {
            int result = DLL.BattleNetStatus();
            if (mBattleNetStatus != result)
            {
                DumpWriter.Write((int)DumpFrameType.BattleNetStatus);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetBattleNetStatusFrame()
                {
                    Return = result,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);

                mBattleNetStatus = result;
            }
            return result;
        }

        private BattleNet.DllEntityId mGameAccountId = new BattleNet.DllEntityId();
        public BattleNet.DllEntityId GetMyGameAccountId()
        {
            BattleNet.DllEntityId result = DLL.GetMyGameAccountId();
            if (!mGameAccountId.Equals(result))
            {
                DumpWriter.Write((int)DumpFrameType.GetMyGameAccountId);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetMyGameAccountIdFrame()
                {
                    Return = result,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);

                mGameAccountId = result;
            }
            return result;
        }

        public string GetAccountCountry()
        {
            string result = DLL.GetAccountCountry();

            DumpWriter.Write((int)DumpFrameType.GetAccountCountry);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new GetAccountCountryFrame()
            {
                Return = result,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            return result;
        }

        private int mAccountRegion = 0;
        public int GetAccountRegion()
        {
            int result = DLL.GetAccountRegion();

            if (mAccountRegion != result)
            {
                DumpWriter.Write((int)DumpFrameType.GetAccountRegion);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetAccountRegionFrame()
                {
                    Return = result,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);

                mAccountRegion = result;
            }

            return result;
        }

        public int GetCurrentRegion()
        {
            int result = DLL.GetCurrentRegion();

            DumpWriter.Write((int)DumpFrameType.GetCurrentRegion);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new GetCurrentRegionFrame()
            {
                Return = result,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            return result;
        }

        private BattleNet.DllLockouts mPlayRestrictions = new BattleNet.DllLockouts();
        public void GetPlayRestrictions(ref BattleNet.DllLockouts restrictions, bool reload)
        {
            DLL.GetPlayRestrictions(ref restrictions, reload);

            if (!mPlayRestrictions.Equals(restrictions) || reload)
            {
                DumpWriter.Write((int)DumpFrameType.GetPlayRestrictions);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetPlayRestrictionsFrame()
                {
                    Reload = reload,
                    Restrictions = restrictions,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);

                mPlayRestrictions = restrictions;
            }
        }

        public void SendUtilPacket(int packetId, int systemId, byte[] bytes, int size, int subID, int context)
        {
            DumpWriter.Write((int)DumpFrameType.SendUtilPacket);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new SendUtilPacketFrame()
            {
                PacketID = packetId,
                SystemID = systemId,
                SubID = subID,
                Context = context,
                Body = bytes,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.SendUtilPacket(packetId, systemId, bytes, size, subID, context);
        }

        public PegasusPacket NextUtilPacket()
        {
            PegasusPacket packet = DLL.NextUtilPacket();
            if (packet == null) return null;
            byte[] packetBody = (byte[])packet.Body;

            DumpWriter.Write((int)DumpFrameType.NextUtilPacket);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new NextUtilPacketFrame()
            {
                PacketID = packet.Type,
                Context = packet.Context,
                Body = packetBody,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            return packet;
        }

        private BattleNet.DllQueueInfo mQueueInfo = new BattleNet.DllQueueInfo();
        public void GetQueueInfo(ref BattleNet.DllQueueInfo queueInfo)
        {
            DLL.GetQueueInfo(ref queueInfo);
            if (!mQueueInfo.Equals(queueInfo))
            {
                DumpWriter.Write((int)DumpFrameType.GetQueueInfo);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetQueueInfoFrame()
                {
                    QueueInfo = queueInfo,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);

                mQueueInfo = queueInfo;
            }
        }

        public BattleNet.QueueEvent GetQueueEvent()
        {
            BattleNet.QueueEvent queueEvent = DLL.GetQueueEvent();
            if (queueEvent != null)
            {
                DumpWriter.Write((int)DumpFrameType.GetQueueEvent);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetQueueEventFrame()
                {
                    QueueEvent = queueEvent,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);

                if (queueEvent.EventType == BattleNet.QueueEvent.Type.QUEUE_GAME_STARTED && queueEvent.GameServer != null)
                {
                    if (CaptureDevice.Started) CaptureDevice.StopCapture();
                    string filter = string.Format("host {0} and tcp port {1}", queueEvent.GameServer.Address, queueEvent.GameServer.Port);
                    DumpWriter.Write((int)DumpFrameType.GameStarted);
                    DumpWriter.Write(DateTime.Now.ToFileTime());
                    new GameStartedFrame()
                    {
                        Filter = filter,
                    }.Write(DumpWriter);
                    DumpWriter.Write((int)DumpFrameType.FrameTerminator);

                    CaptureStack.Reset();
                    CaptureDevice.Filter = filter;
                    CaptureDevice.StartCapture();
                }
            }
            return queueEvent;
        }

        public int GetBnetEventsSize() { return DLL.GetBnetEventsSize(); }

        public void GetBnetEvents(BattleNet.BnetEvent[] events)
        {
            DLL.GetBnetEvents(events);
            if (events.Length > 0)
            {
                DumpWriter.Write((int)DumpFrameType.GetBnetEvents);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetBnetEventsFrame()
                {
                    Events = events,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);
            }
        }

        public void ClearBnetEvents()
        {
            DumpWriter.Write((int)DumpFrameType.ClearBnetEvents);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new ClearBnetEventsFrame()
            {
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.ClearBnetEvents();
        }


        public int GetErrorsCount() { return DLL.GetErrorsCount(); }

        public void GetErrors(BnetErrorInfo[] errors)
        {
            DLL.GetErrors(errors);
            if (errors.Length > 0)
            {
                DumpWriter.Write((int)DumpFrameType.GetErrors);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetErrorsFrame()
                {
                    Errors = errors,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);
            }
        }

        public void ClearErrors()
        {
            DumpWriter.Write((int)DumpFrameType.ClearErrors);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new ClearErrorsFrame()
            {
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.ClearErrors();
        }

        private BattleNet.DllFriendsInfo mFriendsInfo = new BattleNet.DllFriendsInfo();
        public void GetFriendsInfo(ref BattleNet.DllFriendsInfo info)
        {
            DLL.GetFriendsInfo(ref info);
            if (!mFriendsInfo.Equals(info))
            {
                DumpWriter.Write((int)DumpFrameType.GetFriendsInfo);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetFriendsInfoFrame()
                {
                    Info = info,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);

                mFriendsInfo = info;
            }
        }

        public void GetFriendsUpdates(BattleNet.FriendsUpdate[] updates)
        {
            DLL.GetFriendsUpdates(updates);
            if (mFriendsInfo.updateSize > 0)
            {
                BattleNet.FriendsUpdate[] actualUpdates = new BattleNet.FriendsUpdate[mFriendsInfo.updateSize];
                Array.Copy(updates, actualUpdates, mFriendsInfo.updateSize);
                DumpWriter.Write((int)DumpFrameType.GetFriendsUpdates);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetFriendsUpdatesFrame()
                {
                    Updates = actualUpdates,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);
            }
        }

        public void ClearFriendsUpdates()
        {
            DumpWriter.Write((int)DumpFrameType.ClearFriendsUpdates);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new ClearFriendsUpdatesFrame()
            {
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.ClearFriendsUpdates();
        }

        public void SendFriendInvite(string inviter, string invitee, bool byEmail)
        {
            DumpWriter.Write((int)DumpFrameType.SendFriendInvite);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new SendFriendInviteFrame()
            {
                Inviter = inviter,
                Invitee = invitee,
                ByEmail = byEmail,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.SendFriendInvite(inviter, invitee, byEmail);
        }

        public void ManageFriendInvite(int action, ulong inviteId)
        {
            DumpWriter.Write((int)DumpFrameType.ManageFriendInvite);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new ManageFriendInviteFrame()
            {
                Action = action,
                InviteID = inviteId,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.ManageFriendInvite(action, inviteId);
        }

        public void RemoveFriend(ref BnetAccountId account)
        {
            DLL.RemoveFriend(ref account);
            DumpWriter.Write((int)DumpFrameType.RemoveFriend);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new RemoveFriendFrame()
            {
                Account = BnetAccountId.CreateFromBnetEntityId(account),
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);
        }

        private BattleNet.DllWhisperInfo mWhisperInfo = new BattleNet.DllWhisperInfo();
        public void GetWhisperInfo(ref BattleNet.DllWhisperInfo info)
        {
            DLL.GetWhisperInfo(ref info);
            if (!mWhisperInfo.Equals(info))
            {
                DumpWriter.Write((int)DumpFrameType.GetWhisperInfo);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetWhisperInfoFrame()
                {
                    Info = info,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);

                mWhisperInfo = info;
            }
        }

        public void GetWhispers(BnetWhisper[] whispers)
        {
            DLL.GetWhispers(whispers);
            if (whispers.Length > 0)
            {
                DumpWriter.Write((int)DumpFrameType.GetWhispers);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetWhispersFrame()
                {
                    Whispers = whispers,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);
            }
        }

        public void ClearWhispers()
        {
            DumpWriter.Write((int)DumpFrameType.ClearWhispers);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new ClearWhispersFrame()
            {
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.ClearWhispers();
        }

        public void SendWhisper(BnetGameAccountId gameAccount, string message)
        {
            DumpWriter.Write((int)DumpFrameType.SendWhisper);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new SendWhisperFrame()
            {
                GameAccount = gameAccount,
                Message = message,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.SendWhisper(gameAccount, message);
        }

        public string FilterProfanity(string unfiltered)
        {
            string result = DLL.FilterProfanity(unfiltered);

            DumpWriter.Write((int)DumpFrameType.FilterProfanity);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new FilterProfanityFrame()
            {
                Return = result,
                Unfiltered = unfiltered,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            return result;
        }

        public int NumChallenges() { return DLL.NumChallenges(); }

        public void GetChallenges(BattleNet.DllChallengeInfo[] challenges)
        {
            DLL.GetChallenges(challenges);
            if (challenges.Length > 0)
            {
                DumpWriter.Write((int)DumpFrameType.GetChallenges);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetChallengesFrame()
                {
                    Challenges = challenges,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);
            }
        }

        public void ClearChallenges()
        {
            DumpWriter.Write((int)DumpFrameType.ClearChallenges);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new ClearChallengesFrame()
            {
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.ClearChallenges();
        }

        public void AnswerChallenge(ulong challengeID, string answer)
        {
            DumpWriter.Write((int)DumpFrameType.AnswerChallenge);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new AnswerChallengeFrame()
            {
                ChallengeID = challengeID,
                Answer = answer,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.AnswerChallenge(challengeID, answer);
        }

        public void CancelChallenge(ulong challengeID)
        {
            DumpWriter.Write((int)DumpFrameType.CancelChallenge);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new CancelChallengeFrame()
            {
                ChallengeID = challengeID,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.CancelChallenge(challengeID);
        }

        private BattleNet.DllPartyInfo mPartyInfo = new BattleNet.DllPartyInfo();
        public void GetPartyUpdatesInfo(ref BattleNet.DllPartyInfo info)
        {
            DLL.GetPartyUpdatesInfo(ref info);
            if (!mPartyInfo.Equals(info))
            {
                DumpWriter.Write((int)DumpFrameType.GetPartyUpdatesInfo);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetPartyUpdatesInfoFrame()
                {
                    Info = info,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);

                mPartyInfo = info;
            }
        }

        public void GetPartyUpdates(BattleNet.PartyEvent[] updates)
        {
            DLL.GetPartyUpdates(updates);
            if (updates.Length > 0)
            {
                DumpWriter.Write((int)DumpFrameType.GetPartyUpdates);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetPartyUpdatesFrame()
                {
                    Updates = updates,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);
            }
        }

        public void ClearPartyUpdates()
        {
            DumpWriter.Write((int)DumpFrameType.ClearPartyUpdates);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new ClearPartyUpdatesFrame()
            {
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.ClearPartyUpdates();
        }

        public void AcceptPartyInvite(ref BattleNet.DllEntityId partyId)
        {
            DLL.AcceptPartyInvite(ref partyId);
            DumpWriter.Write((int)DumpFrameType.AcceptPartyInvite);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new AcceptPartyInviteFrame()
            {
                PartyID = partyId,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);
        }

        public void DeclinePartyInvite(ref BattleNet.DllEntityId partyId)
        {
            DLL.DeclinePartyInvite(ref partyId);
            DumpWriter.Write((int)DumpFrameType.DeclinePartyInvite);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new DeclinePartyInviteFrame()
            {
                PartyID = partyId,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);
        }

        public void SendPartyInvite(ref BattleNet.DllEntityId gameAccount)
        {
            DLL.SendPartyInvite(ref gameAccount);
            DumpWriter.Write((int)DumpFrameType.SendPartyInvite);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new SendPartyInviteFrame()
            {
                GameAccount = gameAccount,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);
        }

        public void RescindPartyInvite(ref BattleNet.DllEntityId partyId)
        {
            DLL.RescindPartyInvite(ref partyId);
            DumpWriter.Write((int)DumpFrameType.RescindPartyInvite);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new RescindPartyInviteFrame()
            {
                PartyID = partyId,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);
        }

        public void SetPartyDeck(ref BattleNet.DllEntityId partyId, long deckID)
        {
            DLL.SetPartyDeck(ref partyId, deckID);
            DumpWriter.Write((int)DumpFrameType.SetPartyDeck);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new SetPartyDeckFrame()
            {
                PartyID = partyId,
                DeckID = deckID,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);
        }

        public int PresenceSize() { return DLL.PresenceSize(); }

        public void GetPresence(BattleNet.PresenceUpdate[] updates)
        {
            DLL.GetPresence(updates);
            if (updates.Length > 0)
            {
                DumpWriter.Write((int)DumpFrameType.GetPresence);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new GetPresenceFrame()
                {
                    Updates = updates,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);
            }
        }

        public void ClearPresence()
        {
            DumpWriter.Write((int)DumpFrameType.ClearPresence);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new ClearPresenceFrame()
            {
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.ClearPresence();
        }

        public void SetPresenceBlob(uint field, byte[] val)
        {
            DumpWriter.Write((int)DumpFrameType.SetPresenceBlob);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new SetPresenceBlobFrame()
            {
                Field = field,
                Val = val,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.SetPresenceBlob(field, val);
        }

        public void SetPresenceBool(uint field, bool val)
        {
            DumpWriter.Write((int)DumpFrameType.SetPresenceBool);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new SetPresenceBoolFrame()
            {
                Field = field,
                Val = val,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.SetPresenceBool(field, val);
        }

        public void SetPresenceInt(uint field, long val)
        {
            DumpWriter.Write((int)DumpFrameType.SetPresenceInt);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new SetPresenceIntFrame()
            {
                Field = field,
                Val = val,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.SetPresenceInt(field, val);
        }

        public void SetPresenceString(uint field, string val)
        {
            DumpWriter.Write((int)DumpFrameType.SetPresenceString);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new SetPresenceStringFrame()
            {
                Field = field,
                Val = val,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.SetPresenceString(field, val);
        }

        public void SetRichPresence(BattleNet.DllRichPresenceUpdate[] updates)
        {
            if (updates.Length > 0)
            {
                DumpWriter.Write((int)DumpFrameType.SetRichPresence);
                DumpWriter.Write(DateTime.Now.ToFileTime());
                new SetRichPresenceFrame()
                {
                    Updates = updates,
                }.Write(DumpWriter);
                DumpWriter.Write((int)DumpFrameType.FrameTerminator);
            }
            DLL.SetRichPresence(updates);
        }

        public void DraftQueue(bool join)
        {
            DumpWriter.Write((int)DumpFrameType.DraftQueue);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new DraftQueueFrame()
            {
                Join = join,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.DraftQueue(join);
        }

        public void RankedMatch(long deckID)
        {
            DumpWriter.Write((int)DumpFrameType.RankedMatch);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new RankedMatchFrame()
            {
                DeckID = deckID,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.RankedMatch(deckID);
        }

        public void UnrankedMatch(long deckID, bool newbie)
        {
            DumpWriter.Write((int)DumpFrameType.UnrankedMatch);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new UnrankedMatchFrame()
            {
                DeckID = deckID,
                Newbie = newbie,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.UnrankedMatch(deckID, newbie);
        }

        public void StartScenario(int scenario, long deckID)
        {
            DumpWriter.Write((int)DumpFrameType.StartScenario);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new StartScenarioFrame()
            {
                Scenario = scenario,
                DeckID = deckID,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.StartScenario(scenario, deckID);
        }

        public void StartScenarioAI(int scenario, long deckID, long aiDeckID)
        {
            DumpWriter.Write((int)DumpFrameType.StartScenarioAI);
            DumpWriter.Write(DateTime.Now.ToFileTime());
            new StartScenarioAIFrame()
            {
                Scenario = scenario,
                DeckID = deckID,
                AIDeckID = aiDeckID,
            }.Write(DumpWriter);
            DumpWriter.Write((int)DumpFrameType.FrameTerminator);

            DLL.StartScenarioAI(scenario, deckID, aiDeckID);
        }
    }
}
