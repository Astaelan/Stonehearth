using StonehearthCommon;
using StonehearthCommon.DumpFrames;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthDumper
{
    internal static class Program
    {
        private static BinaryReader DumpReader = null;
        private static IndentedStreamWriter DumpWriter = null;

        private static void Main(string[] pArgs)
        {
            string binaryPath = null;
            if (pArgs.Length > 0) binaryPath = pArgs[0];
            else
            {
                binaryPath = Path.GetFullPath("Captures");
                string[] binaryFiles = Directory.GetFiles(binaryPath, "*.bin", SearchOption.TopDirectoryOnly);
                if (binaryFiles.Length == 0) return;
                Array.Sort(binaryFiles);
                binaryPath = binaryFiles[binaryFiles.Length - 1];
            }
            if (!File.Exists(binaryPath)) return;

            DumpReader = new BinaryReader(new FileStream(binaryPath, FileMode.Open, FileAccess.Read), Encoding.UTF8);
            DumpWriter = new IndentedStreamWriter(Path.ChangeExtension(binaryPath, ".txt"));

            while (DumpReader.BaseStream.Position < DumpReader.BaseStream.Length)
            {
                DumpFrameType dumpFrameType = (DumpFrameType)DumpReader.ReadInt32();
                DateTime dumpFrameTimestamp = DateTime.FromFileTime(DumpReader.ReadInt64());
                switch (dumpFrameType)
                {
                    case DumpFrameType.GameStarted: new GameStartedFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GameFinished: new GameFinishedFrame().Read(DumpReader).Dump(DumpWriter); break;

                    case DumpFrameType.Init: new InitFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.IsInitialized: new IsInitializedFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.AppQuit: new AppQuitFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.ApplicationWasPaused: new ApplicationWasPausedFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.ApplicationWasUnpaused: new ApplicationWasUnpausedFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetLaunchOption: new GetLaunchOptionFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetStoredBNetIPAddress: new GetStoredBNetIPAddressFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.RequestCloseAurora: new RequestCloseAuroraFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.CloseAurora: new CloseAuroraFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.QueryAurora: new QueryAuroraFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.ProvideWebAuthToken: new ProvideWebAuthTokenFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.CheckWebAuth: new CheckWebAuthFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetShutdownMinutes: new GetShutdownMinutesFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.BattleNetStatus: new GetBattleNetStatusFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetMyGameAccountId: new GetMyGameAccountIdFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetAccountCountry: new GetAccountCountryFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetAccountRegion: new GetAccountRegionFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetCurrentRegion: new GetCurrentRegionFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetPlayRestrictions: new GetPlayRestrictionsFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.SendUtilPacket: new SendUtilPacketFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.NextUtilPacket: new NextUtilPacketFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetQueueInfo: new GetQueueInfoFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetQueueEvent: new GetQueueEventFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetBnetEvents: new GetBnetEventsFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.ClearBnetEvents: new ClearBnetEventsFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetErrors: new GetErrorsFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.ClearErrors: new ClearErrorsFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetFriendsInfo: new GetFriendsInfoFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetFriendsUpdates: new GetFriendsUpdatesFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.ClearFriendsUpdates: new ClearFriendsUpdatesFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.SendFriendInvite: new SendFriendInviteFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.ManageFriendInvite: new ManageFriendInviteFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.RemoveFriend: new RemoveFriendFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetWhisperInfo: new GetWhisperInfoFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetWhispers: new GetWhispersFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.ClearWhispers: new ClearWhispersFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.SendWhisper: new SendWhisperFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.FilterProfanity: new FilterProfanityFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetChallenges: new GetChallengesFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.ClearChallenges: new ClearChallengesFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.AnswerChallenge: new AnswerChallengeFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.CancelChallenge: new CancelChallengeFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetPartyUpdatesInfo: new GetPartyUpdatesInfoFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetPartyUpdates: new GetPartyUpdatesFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.ClearPartyUpdates: new ClearPartyUpdatesFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.AcceptPartyInvite: new AcceptPartyInviteFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.DeclinePartyInvite: new DeclinePartyInviteFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.SendPartyInvite: new SendPartyInviteFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.RescindPartyInvite: new RescindPartyInviteFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.SetPartyDeck: new SetPartyDeckFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.GetPresence: new GetPresenceFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.ClearPresence: new ClearPresenceFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.SetPresenceBlob: new SetPresenceBlobFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.SetPresenceBool: new SetPresenceBoolFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.SetPresenceInt: new SetPresenceIntFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.SetPresenceString: new SetPresenceStringFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.SetRichPresence: new SetRichPresenceFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.DraftQueue: new DraftQueueFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.RankedMatch: new RankedMatchFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.UnrankedMatch: new UnrankedMatchFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.StartScenario: new StartScenarioFrame().Read(DumpReader).Dump(DumpWriter); break;
                    case DumpFrameType.StartScenarioAI: new StartScenarioAIFrame().Read(DumpReader).Dump(DumpWriter); break;

                    case DumpFrameType.ClientGamePacket:
                    case DumpFrameType.ServerGamePacket:
                        {
                            bool fromClient = dumpFrameType == DumpFrameType.ClientGamePacket;
                            int packetID = DumpReader.ReadInt32();
                            int bodyLength = DumpReader.ReadInt32();
                            byte[] bodyData = DumpReader.ReadBytes(bodyLength);

                            DumpWriter.WriteLine(dumpFrameType.ToString());
                            DumpWriter.Indent++;
                            DumpWriter.WriteLine("PacketID: {0}", packetID);
                            DumpWriter.WriteLine("BodyLength: {0}", bodyData.Length);
                            DumpFrameExternals.DumpGamePacket(DumpWriter, packetID, bodyData);
                            DumpWriter.Indent--;
                            break;
                        }

                    default: throw new DataMisalignedException(string.Format("Unknown DumpFrameType: {0}", dumpFrameType));
                }
                if (DumpReader.ReadInt32() != (int)DumpFrameType.FrameTerminator)
                    throw new DataMisalignedException(string.Format("DumpFrameType: {0}", dumpFrameType));
                DumpWriter.WriteLine(new string('=', 100));
            }
        }
    }
}
