using BobNetProto;
using PegasusGame;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public static class DumpFrameExternals
    {
        public static void ForEach<T>(this IList<T> pThis, Action<T> pAction) { foreach (T value in pThis) pAction(value); }

        public static void Write(BinaryWriter pWriter, BattleNet.DllEntityId pValue)
        {
            pWriter.Write(pValue.hi);
            pWriter.Write(pValue.lo);
        }

        public static void Read(BinaryReader pReader, ref BattleNet.DllEntityId pValue)
        {
            pValue.hi = pReader.ReadUInt64();
            pValue.lo = pReader.ReadUInt64();
        }

        public static void Dump(IndentedStreamWriter pWriter, BattleNet.DllEntityId pValue)
        {
            pWriter.WriteLine("hi: {0}", pValue.hi);
            pWriter.WriteLine("lo: {0}", pValue.lo);
        }


        public static void Write(BinaryWriter pWriter, BattleNet.DllLockouts pValue)
        {
            pWriter.Write(pValue.CAISactive);
            pWriter.Write(pValue.CAISplayed);
            pWriter.Write(pValue.CAISrested);
            pWriter.Write(pValue.day1.ToInt32());
            pWriter.Write(pValue.day2.ToInt32());
            pWriter.Write(pValue.day3.ToInt32());
            pWriter.Write(pValue.day4.ToInt32());
            pWriter.Write(pValue.day5.ToInt32());
            pWriter.Write(pValue.day6.ToInt32());
            pWriter.Write(pValue.day7.ToInt32());
            pWriter.Write(pValue.loaded);
            pWriter.Write(pValue.loading);
            pWriter.Write(pValue.minutesRemaining);
            pWriter.Write(pValue.parentalControls);
            pWriter.Write(pValue.parentalMinutesRemaining);
            pWriter.Write(pValue.parentalTimedAccount);
            pWriter.Write(pValue.readingCAISI);
            pWriter.Write(pValue.readingGSI);
            pWriter.Write(pValue.readingGTRI);
            pWriter.Write(pValue.readingPCI);
            pWriter.Write(pValue.timedAccount);
        }

        public static void Read(BinaryReader pReader, ref BattleNet.DllLockouts pValue)
        {
            pValue.CAISactive = pReader.ReadBoolean();
            pValue.CAISplayed = pReader.ReadInt32();
            pValue.CAISrested = pReader.ReadInt32();
            pValue.day1 = new IntPtr(pReader.ReadInt32());
            pValue.day2 = new IntPtr(pReader.ReadInt32());
            pValue.day3 = new IntPtr(pReader.ReadInt32());
            pValue.day4 = new IntPtr(pReader.ReadInt32());
            pValue.day5 = new IntPtr(pReader.ReadInt32());
            pValue.day6 = new IntPtr(pReader.ReadInt32());
            pValue.day7 = new IntPtr(pReader.ReadInt32());
            pValue.loaded = pReader.ReadBoolean();
            pValue.loading = pReader.ReadBoolean();
            pValue.minutesRemaining = pReader.ReadInt32();
            pValue.parentalControls = pReader.ReadBoolean();
            pValue.parentalMinutesRemaining = pReader.ReadInt32();
            pValue.parentalTimedAccount = pReader.ReadBoolean();
            pValue.readingCAISI = pReader.ReadBoolean();
            pValue.readingGSI = pReader.ReadBoolean();
            pValue.readingGTRI = pReader.ReadBoolean();
            pValue.readingPCI = pReader.ReadBoolean();
            pValue.timedAccount = pReader.ReadBoolean();
        }

        public static void Dump(IndentedStreamWriter pWriter, BattleNet.DllLockouts pValue)
        {
            pWriter.WriteLine("CAISactive: {0}", pValue.CAISactive);
            pWriter.WriteLine("CAISplayed: {0}", pValue.CAISplayed);
            pWriter.WriteLine("CAISrested: {0}", pValue.CAISrested);
            pWriter.WriteLine("day1: {0}", pValue.day1);
            pWriter.WriteLine("day2: {0}", pValue.day2);
            pWriter.WriteLine("day3: {0}", pValue.day3);
            pWriter.WriteLine("day4: {0}", pValue.day4);
            pWriter.WriteLine("day5: {0}", pValue.day5);
            pWriter.WriteLine("day6: {0}", pValue.day6);
            pWriter.WriteLine("day7: {0}", pValue.day7);
            pWriter.WriteLine("loaded: {0}", pValue.loaded);
            pWriter.WriteLine("loading: {0}", pValue.loading);
            pWriter.WriteLine("minutesRemaining: {0}", pValue.minutesRemaining);
            pWriter.WriteLine("parentalControls: {0}", pValue.parentalControls);
            pWriter.WriteLine("parentalMinutesRemaining: {0}", pValue.parentalMinutesRemaining);
            pWriter.WriteLine("parentalTimedAccount: {0}", pValue.parentalTimedAccount);
            pWriter.WriteLine("readingCAISI: {0}", pValue.readingCAISI);
            pWriter.WriteLine("readingGSI: {0}", pValue.readingGSI);
            pWriter.WriteLine("readingGTRI: {0}", pValue.readingGTRI);
            pWriter.WriteLine("readingPCI: {0}", pValue.readingPCI);
            pWriter.WriteLine("timedAccount: {0}", pValue.timedAccount);
        }


        public static void Write(BinaryWriter pWriter, BattleNet.DllQueueInfo pValue)
        {
            pWriter.Write(pValue.changed);
            pWriter.Write(pValue.end);
            pWriter.Write(pValue.position);
            pWriter.Write(pValue.stdev);
        }

        public static void Read(BinaryReader pReader, ref BattleNet.DllQueueInfo pValue)
        {
            pValue.changed = pReader.ReadBoolean();
            pValue.end = pReader.ReadInt64();
            pValue.position = pReader.ReadInt32();
            pValue.stdev = pReader.ReadInt64();
        }

        public static void Dump(IndentedStreamWriter pWriter, BattleNet.DllQueueInfo pValue)
        {
            pWriter.WriteLine("changed: {0}", pValue.changed);
            pWriter.WriteLine("end: {0}", pValue.end);
            pWriter.WriteLine("position: {0}", pValue.position);
            pWriter.WriteLine("stdev: {0}", pValue.stdev);
        }


        public static void Write(BinaryWriter pWriter, BattleNet.QueueEvent pValue)
        {
            pWriter.Write(pValue.BnetError);
            pWriter.Write((int)pValue.EventType);
            pWriter.Write(pValue.GameServer != null);
            if (pValue.GameServer != null) Write(pWriter, pValue.GameServer);
            pWriter.Write(pValue.MaxSeconds);
            pWriter.Write(pValue.MinSeconds);
        }

        public static void Read(BinaryReader pReader, BattleNet.QueueEvent pValue)
        {
            pValue.BnetError = pReader.ReadInt32();
            pValue.EventType = (BattleNet.QueueEvent.Type)pReader.ReadInt32();
            bool gameServerExists = pReader.ReadBoolean();
            if (gameServerExists)
            {
                pValue.GameServer = new BattleNet.GameServerInfo();
                Read(pReader, pValue.GameServer);
            }
            pValue.MaxSeconds = pReader.ReadInt32();
            pValue.MinSeconds = pReader.ReadInt32();
        }

        public static void Dump(IndentedStreamWriter pWriter, BattleNet.QueueEvent pValue)
        {
            pWriter.WriteLine("BnetError: {0}", pValue.BnetError);
            pWriter.WriteLine("EventType: {0}", pValue.EventType);
            if (pValue.GameServer != null)
            {
                pWriter.WriteLine("GameServer", pValue.EventType);
                pWriter.Indent++;
                Dump(pWriter, pValue.GameServer);
                pWriter.Indent--;
            }
            pWriter.WriteLine("MaxSeconds: {0}", pValue.MaxSeconds);
            pWriter.WriteLine("MinSeconds: {0}", pValue.MinSeconds);
        }


        public static void Write(BinaryWriter pWriter, BattleNet.GameServerInfo pValue)
        {
            pWriter.Write(pValue.Address);
            pWriter.Write(pValue.AuroraPassword);
            pWriter.Write(pValue.ClientHandle);
            pWriter.Write(pValue.GameHandle);
            pWriter.Write(pValue.Port);
            pWriter.Write(pValue.Version);
        }

        public static void Read(BinaryReader pReader, BattleNet.GameServerInfo pValue)
        {
            pValue.Address = pReader.ReadString();
            pValue.AuroraPassword = pReader.ReadString();
            pValue.ClientHandle = pReader.ReadInt64();
            pValue.GameHandle = pReader.ReadInt32();
            pValue.Port = pReader.ReadInt32();
            pValue.Version = pReader.ReadString();
        }

        public static void Dump(IndentedStreamWriter pWriter, BattleNet.GameServerInfo pValue)
        {
            pWriter.WriteLine("Address: {0}", pValue.Address);
            pWriter.WriteLine("AuroraPassword: {0}", pValue.AuroraPassword);
            pWriter.WriteLine("ClientHandle: {0}", pValue.ClientHandle);
            pWriter.WriteLine("GameHandle: {0}", pValue.GameHandle);
            pWriter.WriteLine("Port: {0}", pValue.Port);
            pWriter.WriteLine("Version: {0}", pValue.Version);
        }


        public static void Write(BinaryWriter pWriter, BnetErrorInfo pValue)
        {
            pWriter.Write(pValue.GetCode());
            pWriter.Write((int)pValue.GetError());
            pWriter.Write((int)pValue.GetFeature());
            pWriter.Write((int)pValue.GetFeatureEvent());
        }

        public static void Read(BinaryReader pReader, BnetErrorInfo pValue)
        {
            pValue.SetCode(pReader.ReadUInt32());
            pValue.SetError((BattleNetErrors)pReader.ReadInt32());
            pValue.SetFeature((BnetFeature)pReader.ReadInt32());
            pValue.SetFeatureEvent((BnetFeatureEvent)pReader.ReadInt32());
        }

        public static void Dump(IndentedStreamWriter pWriter, BnetErrorInfo pValue)
        {
            pWriter.WriteLine("Code: {0}", pValue.GetCode());
            pWriter.WriteLine("Error: {0}", pValue.GetError());
            pWriter.WriteLine("Feature: {0}", pValue.GetFeature());
            pWriter.WriteLine("FeatureEvent: {0}", pValue.GetFeatureEvent());
        }


        public static void Write(BinaryWriter pWriter, BattleNet.DllFriendsInfo pValue)
        {
            pWriter.Write(pValue.friendsSize);
            pWriter.Write(pValue.maxFriends);
            pWriter.Write(pValue.maxRecvInvites);
            pWriter.Write(pValue.maxSentInvites);
            pWriter.Write(pValue.updateSize);
        }

        public static void Read(BinaryReader pReader, ref BattleNet.DllFriendsInfo pValue)
        {
            pValue.friendsSize = pReader.ReadInt32();
            pValue.maxFriends = pReader.ReadInt32();
            pValue.maxRecvInvites = pReader.ReadInt32();
            pValue.maxSentInvites = pReader.ReadInt32();
            pValue.updateSize = pReader.ReadInt32();
        }

        public static void Dump(IndentedStreamWriter pWriter, BattleNet.DllFriendsInfo pValue)
        {
            pWriter.WriteLine("friendsSize: {0}", pValue.friendsSize);
            pWriter.WriteLine("maxFriends: {0}", pValue.maxFriends);
            pWriter.WriteLine("maxRecvInvites: {0}", pValue.maxRecvInvites);
            pWriter.WriteLine("maxSentInvites: {0}", pValue.maxSentInvites);
            pWriter.WriteLine("updateSize: {0}", pValue.updateSize);
        }


        public static void Write(BinaryWriter pWriter, BattleNet.FriendsUpdate pValue)
        {
            pWriter.Write(pValue.action);
            pWriter.Write(pValue.bool1);
            pWriter.Write(pValue.entity1 != null);
            if (pValue.entity1 != null) Write(pWriter, pValue.entity1);
            pWriter.Write(pValue.entity2 != null);
            if (pValue.entity2 != null) Write(pWriter, pValue.entity2);
            pWriter.Write(pValue.int1);
            pWriter.Write(pValue.long1);
            pWriter.Write(pValue.long2);
            pWriter.Write(pValue.long3);
            pWriter.Write(pValue.string1 != null);
            if (pValue.string1 != null) pWriter.Write(pValue.string1);
            pWriter.Write(pValue.string2 != null);
            if (pValue.string2 != null) pWriter.Write(pValue.string2);
            pWriter.Write(pValue.string3 != null);
            if (pValue.string3 != null) pWriter.Write(pValue.string3);
        }

        public static void Read(BinaryReader pReader, ref BattleNet.FriendsUpdate pValue)
        {
            pValue.action = pReader.ReadInt32();
            pValue.bool1 = pReader.ReadBoolean();
            bool hasEntity1 = pReader.ReadBoolean();
            if (hasEntity1)
            {
                pValue.entity1 = new BnetEntityId();
                Read(pReader, pValue.entity1);
            }
            bool hasEntity2 = pReader.ReadBoolean();
            if (hasEntity2)
            {
                pValue.entity2 = new BnetEntityId();
                Read(pReader, pValue.entity2);
            }
            pValue.int1 = pReader.ReadInt32();
            pValue.long1 = pReader.ReadUInt64();
            pValue.long2 = pReader.ReadUInt64();
            pValue.long3 = pReader.ReadUInt64();
            bool hasString1 = pReader.ReadBoolean();
            if (hasString1) pValue.string1 = pReader.ReadString();
            bool hasString2 = pReader.ReadBoolean();
            if (hasString2) pValue.string2 = pReader.ReadString();
            bool hasString3 = pReader.ReadBoolean();
            if (hasString3) pValue.string3 = pReader.ReadString();
        }

        public static void Dump(IndentedStreamWriter pWriter, BattleNet.FriendsUpdate pValue)
        {
            pWriter.WriteLine("action: {0}", pValue.action);
            pWriter.WriteLine("bool1: {0}", pValue.bool1);
            if (pValue.entity1 != null)
            {
                pWriter.WriteLine("entity1");
                pWriter.Indent++;
                Dump(pWriter, pValue.entity1);
                pWriter.Indent--;
            }
            if (pValue.entity2 != null)
            {
                pWriter.WriteLine("entity2");
                pWriter.Indent++;
                Dump(pWriter, pValue.entity2);
                pWriter.Indent--;
            }
            pWriter.WriteLine("int1: {0}", pValue.int1);
            pWriter.WriteLine("long1: {0}", pValue.long1);
            pWriter.WriteLine("long2: {0}", pValue.long2);
            pWriter.WriteLine("long3: {0}", pValue.long3);
            if (pValue.string1 != null) pWriter.WriteLine("string1: {0}", pValue.string1);
            if (pValue.string2 != null) pWriter.WriteLine("string2: {0}", pValue.string2);
            if (pValue.string3 != null) pWriter.WriteLine("string3: {0}", pValue.string3);
        }


        public static void Write(BinaryWriter pWriter, BnetEntityId pValue)
        {
            pWriter.Write(pValue.GetHi());
            pWriter.Write(pValue.GetLo());
        }

        public static void Read(BinaryReader pReader, BnetEntityId pValue)
        {
            pValue.SetHi(pReader.ReadUInt64());
            pValue.SetLo(pReader.ReadUInt64());
        }

        public static void Dump(IndentedStreamWriter pWriter, BnetEntityId pValue)
        {
            pWriter.WriteLine("Hi: {0}", pValue.GetHi());
            pWriter.WriteLine("Lo: {0}", pValue.GetLo());
        }


        public static void Write(BinaryWriter pWriter, BattleNet.DllWhisperInfo pValue)
        {
            pWriter.Write(pValue.sendResultsSize);
            pWriter.Write(pValue.whisperSize);
        }

        public static void Read(BinaryReader pReader, ref BattleNet.DllWhisperInfo pValue)
        {
            pValue.sendResultsSize = pReader.ReadInt32();
            pValue.whisperSize = pReader.ReadInt32();
        }

        public static void Dump(IndentedStreamWriter pWriter, BattleNet.DllWhisperInfo pValue)
        {
            pWriter.WriteLine("sendResultsSize: {0}", pValue.sendResultsSize);
            pWriter.WriteLine("whisperSize: {0}", pValue.whisperSize);
        }


        public static void Write(BinaryWriter pWriter, BnetWhisper pValue)
        {
            pWriter.Write(pValue.GetErrorInfo() != null);
            if (pValue.GetErrorInfo() != null) Write(pWriter, pValue.GetErrorInfo());
            pWriter.Write(pValue.GetMessage() != null);
            if (pValue.GetMessage() != null) pWriter.Write(pValue.GetMessage());
            pWriter.Write(pValue.GetReceiverId() != null);
            if (pValue.GetReceiverId() != null) Write(pWriter, pValue.GetReceiverId());
            pWriter.Write(pValue.GetSpeakerId() != null);
            if (pValue.GetSpeakerId() != null) Write(pWriter, pValue.GetSpeakerId());
            pWriter.Write(pValue.GetTimestampMicrosec());
        }

        public static void Read(BinaryReader pReader, BnetWhisper pValue)
        {
            if (pReader.ReadBoolean())
            {
                BnetErrorInfo error = new BnetErrorInfo(BnetFeature.Auth, BnetFeatureEvent.Auth_Logon, BattleNetErrors.ERROR_OK);
                Read(pReader, error);
                pValue.SetErrorInfo(error);
            }
            if (pReader.ReadBoolean()) pValue.SetMessage(pReader.ReadString());
            if (pReader.ReadBoolean())
            {
                BnetGameAccountId receiverId = new BnetGameAccountId();
                Read(pReader, receiverId);
                pValue.SetReceiverId(receiverId);
            }
            if (pReader.ReadBoolean())
            {
                BnetGameAccountId speakerId = new BnetGameAccountId();
                Read(pReader, speakerId);
                pValue.SetSpeakerId(speakerId);
            }
            pValue.SetTimestampMicrosec(pReader.ReadUInt64());
        }

        public static void Dump(IndentedStreamWriter pWriter, BnetWhisper pValue)
        {
            if (pValue.GetErrorInfo() != null)
            {
                pWriter.WriteLine("ErrorInfo");
                pWriter.Indent++;
                Dump(pWriter, pValue.GetErrorInfo());
                pWriter.Indent--;
            }
            if (pValue.GetMessage() != null) pWriter.WriteLine("Message: {0}", pValue.GetMessage());
            if (pValue.GetReceiverId() != null)
            {
                pWriter.WriteLine("ReceiverId");
                pWriter.Indent++;
                Dump(pWriter, pValue.GetReceiverId());
                pWriter.Indent--;
            }
            if (pValue.GetSpeakerId() != null)
            {
                pWriter.WriteLine("SpeakerId");
                pWriter.Indent++;
                Dump(pWriter, pValue.GetSpeakerId());
                pWriter.Indent--;
            }
            pWriter.WriteLine("TimestampMicrosec: {0}", pValue.GetTimestampMicrosec());
        }


        public static void Write(BinaryWriter pWriter, BattleNet.DllChallengeInfo pValue)
        {
            pWriter.Write(pValue.challengeId);
            pWriter.Write(pValue.isRetry);
            pWriter.Write(pValue.type);
        }

        public static void Read(BinaryReader pReader, ref BattleNet.DllChallengeInfo pValue)
        {
            pValue.challengeId = pReader.ReadUInt64();
            pValue.isRetry = pReader.ReadBoolean();
            pValue.type = pReader.ReadInt32();
        }

        public static void Dump(IndentedStreamWriter pWriter, BattleNet.DllChallengeInfo pValue)
        {
            pWriter.WriteLine("challengeId: {0}", pValue.challengeId);
            pWriter.WriteLine("isRetry: {0}", pValue.isRetry);
            pWriter.WriteLine("type: {0}", pValue.type);
        }


        public static void Write(BinaryWriter pWriter, BattleNet.DllPartyInfo pValue)
        {
            pWriter.Write(pValue.size);
        }

        public static void Read(BinaryReader pReader, ref BattleNet.DllPartyInfo pValue)
        {
            pValue.size = pReader.ReadInt32();
        }

        public static void Dump(IndentedStreamWriter pWriter, BattleNet.DllPartyInfo pValue)
        {
            pWriter.WriteLine("size: {0}", pValue.size);
        }


        public static void Write(BinaryWriter pWriter, BattleNet.PartyEvent pValue)
        {
            pWriter.Write(pValue.errorInfo != null);
            if (pValue.errorInfo != null) Write(pWriter, pValue.errorInfo);
            pWriter.Write(pValue.eventData != null);
            if (pValue.eventData != null) pWriter.Write(pValue.eventData);
            pWriter.Write(pValue.eventName != null);
            if (pValue.eventName != null) pWriter.Write(pValue.eventName);
            Write(pWriter, pValue.otherMemberId);
            Write(pWriter, pValue.partyId);
        }

        public static void Read(BinaryReader pReader, ref BattleNet.PartyEvent pValue)
        {
            if (pReader.ReadBoolean())
            {
                pValue.errorInfo = new BnetErrorInfo(BnetFeature.Auth, BnetFeatureEvent.Auth_Logon, BattleNetErrors.ERROR_OK);
                Read(pReader, pValue.errorInfo);
            }
            if (pReader.ReadBoolean()) pValue.eventData = pReader.ReadString();
            if (pReader.ReadBoolean()) pValue.eventName = pReader.ReadString();
            Read(pReader, ref pValue.otherMemberId);
            Read(pReader, ref pValue.partyId);
        }

        public static void Dump(IndentedStreamWriter pWriter, BattleNet.PartyEvent pValue)
        {
            if (pValue.errorInfo != null)
            {
                pWriter.WriteLine("errorInfo");
                pWriter.Indent++;
                Dump(pWriter, pValue.errorInfo);
                pWriter.Indent--;
            }
            if (pValue.eventData != null) pWriter.WriteLine("eventData: {0}", pValue.eventData);
            if (pValue.eventName != null) pWriter.WriteLine("eventName: {0}", pValue.eventName);
            Dump(pWriter, pValue.otherMemberId);
            Dump(pWriter, pValue.partyId);
        }


        public static void Write(BinaryWriter pWriter, BattleNet.PresenceUpdate pValue)
        {
            pWriter.Write(pValue.blobVal != null);
            if (pValue.blobVal != null)
            {
                pWriter.Write(pValue.blobVal.Length);
                pWriter.Write(pValue.blobVal);
            }
            pWriter.Write(pValue.boolVal);
            Write(pWriter, pValue.entityId);
            Write(pWriter, pValue.entityIdVal);
            pWriter.Write(pValue.fieldId);
            pWriter.Write(pValue.groupId);
            pWriter.Write(pValue.index);
            pWriter.Write(pValue.intVal);
            pWriter.Write(pValue.programId);
            pWriter.Write(pValue.stringVal != null);
            if (pValue.stringVal != null) pWriter.Write(pValue.stringVal);
            pWriter.Write(pValue.valCleared);
        }

        public static void Read(BinaryReader pReader, ref BattleNet.PresenceUpdate pValue)
        {
            if (pReader.ReadBoolean())
            {
                int blobValLength = pReader.ReadInt32();
                pValue.blobVal = pReader.ReadBytes(blobValLength);
            }
            pValue.boolVal = pReader.ReadBoolean();
            Read(pReader, ref pValue.entityId);
            Read(pReader, ref pValue.entityIdVal);
            pValue.fieldId = pReader.ReadUInt32();
            pValue.groupId = pReader.ReadUInt32();
            pValue.index = pReader.ReadUInt64();
            pValue.intVal = pReader.ReadInt64();
            pValue.programId = pReader.ReadUInt32();
            if (pReader.ReadBoolean()) pValue.stringVal = pReader.ReadString();
            pValue.valCleared = pReader.ReadBoolean();
        }

        public static void Dump(IndentedStreamWriter pWriter, BattleNet.PresenceUpdate pValue)
        {
            if (pValue.blobVal != null) pWriter.WriteLine("blobVal: {0}", BitConverter.ToString(pValue.blobVal).Replace('-', ' '));
            pWriter.WriteLine("boolVal: {0}", pValue.boolVal);
            pWriter.WriteLine("entityId");
            {
                pWriter.Indent++;
                Dump(pWriter, pValue.entityId);
                pWriter.Indent--;
            }
            pWriter.WriteLine("entityIdVal");
            {
                pWriter.Indent++;
                Dump(pWriter, pValue.entityIdVal);
                pWriter.Indent--;
            }
            pWriter.WriteLine("fieldId: {0}", pValue.fieldId);
            pWriter.WriteLine("groupId: {0}", pValue.groupId);
            pWriter.WriteLine("index: {0}", pValue.index);
            pWriter.WriteLine("intVal: {0}", pValue.intVal);
            pWriter.WriteLine("programId: {0}", pValue.programId);
            if (pValue.stringVal != null) pWriter.WriteLine("stringVal: {0}", pValue.stringVal);
            pWriter.WriteLine("valCleared: {0}", pValue.valCleared);
        }


        public static void Write(BinaryWriter pWriter, BattleNet.DllRichPresenceUpdate pValue)
        {
            pWriter.Write(pValue.index);
            pWriter.Write(pValue.presenceFieldIndex);
            pWriter.Write(pValue.programId);
            pWriter.Write(pValue.streamId);
        }

        public static void Read(BinaryReader pReader, ref BattleNet.DllRichPresenceUpdate pValue)
        {
            pValue.index = pReader.ReadUInt32();
            pValue.presenceFieldIndex = pReader.ReadUInt64();
            pValue.programId = pReader.ReadUInt32();
            pValue.streamId = pReader.ReadUInt32();
        }

        public static void Dump(IndentedStreamWriter pWriter, BattleNet.DllRichPresenceUpdate pValue)
        {
            pWriter.WriteLine("index: {0}", pValue.index);
            pWriter.WriteLine("presenceFieldIndex: {0}", pValue.presenceFieldIndex);
            pWriter.WriteLine("programId: {0}", pValue.programId);
            pWriter.WriteLine("streamId: {0}", pValue.streamId);
        }





        public static void DumpUtilPacket(IndentedStreamWriter pWriter, int pPacketID, byte[] pBody)
        {
            switch (pPacketID)
            {
                case (int)NOP.Types.PacketID.ID: Dump(pWriter, NOP.ParseFrom(pBody)); break;
                case (int)GetAccountInfo.Types.PacketID.ID: Dump(pWriter, GetAccountInfo.ParseFrom(pBody)); break;
                case (int)BoosterList.Types.PacketID.ID: Dump(pWriter, BoosterList.ParseFrom(pBody)); break;
                case (int)ProfileProgress.Types.PacketID.ID: Dump(pWriter, ProfileProgress.ParseFrom(pBody)); break;
                case (int)GuardianVars.Types.PacketID.ID: Dump(pWriter, GuardianVars.ParseFrom(pBody)); break;
                case (int)MedalInfo.Types.PacketID.ID: Dump(pWriter, MedalInfo.ParseFrom(pBody)); break;
                case (int)UpdateLogin.Types.PacketID.ID: Dump(pWriter, UpdateLogin.ParseFrom(pBody)); break;
                case (int)CheckAccountLicenses.Types.PacketID.ID: Dump(pWriter, CheckAccountLicenses.ParseFrom(pBody)); break;
                case (int)CheckGameLicenses.Types.PacketID.ID: Dump(pWriter, CheckGameLicenses.ParseFrom(pBody)); break;
                case (int)CheckLicensesResponse.Types.PacketID.ID: Dump(pWriter, CheckLicensesResponse.ParseFrom(pBody)); break;
                case (int)GetBattlePayConfig.Types.PacketID.ID: Dump(pWriter, GetBattlePayConfig.ParseFrom(pBody)); break;
                case (int)GetBattlePayStatus.Types.PacketID.ID: Dump(pWriter, GetBattlePayStatus.ParseFrom(pBody)); break;
                case (int)BattlePayConfigResponse.Types.PacketID.ID: Dump(pWriter, BattlePayConfigResponse.ParseFrom(pBody)); break;
                case (int)BattlePayStatusResponse.Types.PacketID.ID: Dump(pWriter, BattlePayStatusResponse.ParseFrom(pBody)); break;
                case (int)GetOptions.Types.PacketID.ID: Dump(pWriter, GetOptions.ParseFrom(pBody)); break;
                case (int)ClientOptions.Types.PacketID.ID: Dump(pWriter, ClientOptions.ParseFrom(pBody)); break;
                case (int)DeckList.Types.PacketID.ID: Dump(pWriter, DeckList.ParseFrom(pBody)); break;
                case (int)CardValues.Types.PacketID.ID: Dump(pWriter, CardValues.ParseFrom(pBody)); break;
                case (int)Collection.Types.PacketID.ID: Dump(pWriter, Collection.ParseFrom(pBody)); break;
                case (int)PlayerRecords.Types.PacketID.ID: Dump(pWriter, PlayerRecords.ParseFrom(pBody)); break;
                case (int)ProfileDeckLimit.Types.PacketID.ID: Dump(pWriter, ProfileDeckLimit.ParseFrom(pBody)); break;
                case (int)ArcaneDustBalance.Types.PacketID.ID: Dump(pWriter, ArcaneDustBalance.ParseFrom(pBody)); break;
                case (int)GoldBalance.Types.PacketID.ID: Dump(pWriter, GoldBalance.ParseFrom(pBody)); break;
                case (int)ProfileNotices.Types.PacketID.ID: Dump(pWriter, ProfileNotices.ParseFrom(pBody)); break;
                case (int)RewardProgress.Types.PacketID.ID: Dump(pWriter, RewardProgress.ParseFrom(pBody)); break;
                case (int)HeroXP.Types.PacketID.ID: Dump(pWriter, HeroXP.ParseFrom(pBody)); break;
                case (int)Disconnected.Types.PacketID.ID: Dump(pWriter, Disconnected.ParseFrom(pBody)); break;
                case (int)GetAchieves.Types.PacketID.ID: Dump(pWriter, GetAchieves.ParseFrom(pBody)); break;
                case (int)Achieves.Types.PacketID.ID: Dump(pWriter, Achieves.ParseFrom(pBody)); break;
                case (int)GuardianTrack.Types.PacketID.ID: Dump(pWriter, GuardianTrack.ParseFrom(pBody)); break;
                case (int)SetOptions.Types.PacketID.ID: Dump(pWriter, SetOptions.ParseFrom(pBody)); break;
                case (int)MedalHistory.Types.PacketID.ID: Dump(pWriter, MedalHistory.ParseFrom(pBody)); break;
                case (int)GamesInfo.Types.PacketID.ID: Dump(pWriter, GamesInfo.ParseFrom(pBody)); break;
                case (int)AckNotice.Types.PacketID.ID: Dump(pWriter, AckNotice.ParseFrom(pBody)); break;
                default: pWriter.WriteLine("UnhandledBody"); break;
            }
        }


        public static void Dump(IndentedStreamWriter pWriter, NOP pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
        }

        public static void Dump(IndentedStreamWriter pWriter, GetAccountInfo pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasRequest) pWriter.WriteLine("Request: {0}", pValue.Request.ToSafeString());
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, BoosterList pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("List: {0}", pValue.ListCount);
            if (pValue.ListCount > 0)
            {
                pWriter.Indent++;
                pValue.ListList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, BoosterInfo pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasType) pWriter.WriteLine("Type: {0}", (BoosterType)pValue.Type);
            if (pValue.HasCount) pWriter.WriteLine("Count: {0}", pValue.Count);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, ProfileProgress pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasProgress) pWriter.WriteLine("Progress: {0}", (MissionProgress)pValue.Progress);
            if (pValue.HasBestForge) pWriter.WriteLine("BestForge: {0}", pValue.BestForge);
            if (pValue.HasLastForge) pWriter.WriteLine("LastForge: {0}", pValue.LastForge.ToString());
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, GuardianVars pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasTourney) pWriter.WriteLine("Tourney: {0}", pValue.Tourney);
            if (pValue.HasPractice) pWriter.WriteLine("Practice: {0}", pValue.Practice);
            if (pValue.HasCasual) pWriter.WriteLine("Casual: {0}", pValue.Casual);
            if (pValue.HasForge) pWriter.WriteLine("Forge: {0}", pValue.Forge);
            if (pValue.HasFriendly) pWriter.WriteLine("Friendly: {0}", pValue.Friendly);
            if (pValue.HasManager) pWriter.WriteLine("Manager: {0}", pValue.Manager);
            if (pValue.HasCrafting) pWriter.WriteLine("Crafting: {0}", pValue.Crafting);
            if (pValue.HasHunter) pWriter.WriteLine("Hunter: {0}", pValue.Hunter);
            if (pValue.HasMage) pWriter.WriteLine("Mage: {0}", pValue.Mage);
            if (pValue.HasPaladin) pWriter.WriteLine("Paladin: {0}", pValue.Paladin);
            if (pValue.HasPriest) pWriter.WriteLine("Priest: {0}", pValue.Priest);
            if (pValue.HasRogue) pWriter.WriteLine("Rogue: {0}", pValue.Rogue);
            if (pValue.HasShaman) pWriter.WriteLine("Shaman: {0}", pValue.Shaman);
            if (pValue.HasWarlock) pWriter.WriteLine("Warlock: {0}", pValue.Warlock);
            if (pValue.HasWarrior) pWriter.WriteLine("Warrior: {0}", pValue.Warrior);
            if (pValue.HasShowUserUI) pWriter.WriteLine("ShowUserUI: {0}", pValue.ShowUserUI);
            if (pValue.HasStore) pWriter.WriteLine("Store: {0}", pValue.Store);
            if (pValue.HasBattlePay) pWriter.WriteLine("BattlePay: {0}", pValue.BattlePay);
            if (pValue.HasBuyWithGold) pWriter.WriteLine("BuyWithGold: {0}", pValue.BuyWithGold);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, MedalInfo pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasCurrMedal) pWriter.WriteLine("CurrMedal: {0}", (Medal.Type)pValue.CurrMedal);
            if (pValue.HasCurrXp) pWriter.WriteLine("CurrXp: {0}", pValue.CurrXp);
            if (pValue.HasSeasonWins) pWriter.WriteLine("SeasonWins: {0}", pValue.SeasonWins);
            if (pValue.HasPrevMedal) pWriter.WriteLine("PrevMedal: {0}", (Medal.Type)pValue.PrevMedal);
            if (pValue.HasPrevXp) pWriter.WriteLine("PrevXp: {0}", pValue.PrevXp);
            if (pValue.HasStars) pWriter.WriteLine("Stars: {0}", pValue.Stars);
            if (pValue.HasStreak) pWriter.WriteLine("Streak: {0}", pValue.Streak);
            if (pValue.HasStarLevel) pWriter.WriteLine("StarLevel: {0}", pValue.StarLevel);
            if (pValue.HasLevelStart) pWriter.WriteLine("LevelStart: {0}", pValue.LevelStart);
            if (pValue.HasLevelEnd) pWriter.WriteLine("LevelEnd: {0}", pValue.LevelEnd);
            if (pValue.HasCanLose) pWriter.WriteLine("CanLose: {0}", pValue.CanLose);
            if (pValue.HasMidLevel) pWriter.WriteLine("MidLevel: {0}", pValue.MidLevel);
            if (pValue.HasLegendRank) pWriter.WriteLine("LegendRank: {0}", pValue.LegendRank);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, UpdateLogin pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
        }

        public static void Dump(IndentedStreamWriter pWriter, CheckAccountLicenses pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
        }

        public static void Dump(IndentedStreamWriter pWriter, CheckGameLicenses pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
        }

        public static void Dump(IndentedStreamWriter pWriter, CheckLicensesResponse pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasAccountLevel) pWriter.WriteLine("AccountLevel: {0}", pValue.AccountLevel);
            if (pValue.HasSuccess) pWriter.WriteLine("Success: {0}", pValue.Success);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, GetBattlePayConfig pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
        }

        public static void Dump(IndentedStreamWriter pWriter, GetBattlePayStatus pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
        }

        public static void Dump(IndentedStreamWriter pWriter, BattlePayConfigResponse pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("Bundles: {0}", pValue.BundlesCount);
            if (pValue.BundlesCount > 0)
            {
                pWriter.Indent++;
                pValue.BundlesList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            if (pValue.HasCurrency) pWriter.WriteLine("Currency: {0}", (Currency)pValue.Currency);
            if (pValue.HasUnavailable) pWriter.WriteLine("Unavailable: {0}", pValue.Unavailable);
            if (pValue.HasSecsBeforeAutoCancel) pWriter.WriteLine("SecsBeforeAutoCancel: {0}", pValue.SecsBeforeAutoCancel);
            pWriter.WriteLine("GoldPrices: {0}", pValue.GoldPricesCount);
            if (pValue.GoldPricesCount > 0)
            {
                pWriter.Indent++;
                pValue.GoldPricesList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, Bundle pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasType) pWriter.WriteLine("Type: {0}", pValue.Type);
            if (pValue.HasProduct) pWriter.WriteLine("Product: {0}", (ProductType)pValue.Product);
            if (pValue.HasData) pWriter.WriteLine("Data: {0}", pValue.Data);
            if (pValue.HasQuantity) pWriter.WriteLine("Quantity: {0}", pValue.Quantity);
            if (pValue.HasCost) pWriter.WriteLine("Cost: {0}", pValue.Cost);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, GoldPrice pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasCost) pWriter.WriteLine("Cost: {0}", pValue.Cost);
            if (pValue.HasProduct) pWriter.WriteLine("Product: {0}", (ProductType)pValue.Product);
            if (pValue.HasData) pWriter.WriteLine("Data: {0}", pValue.Data);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, BattlePayStatusResponse pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasStatus) pWriter.WriteLine("Status: {0}", pValue.Status.ToSafeString());
            if (pValue.HasProductType) pWriter.WriteLine("ProductType: {0}", (ProductType)pValue.ProductType);
            if (pValue.HasPurchaseError) Dump(pWriter, pValue.PurchaseError);
            if (pValue.HasBattlePayAvailable) pWriter.WriteLine("BattlePayAvailable: {0}", pValue.BattlePayAvailable);
            if (pValue.HasTransactionId) pWriter.WriteLine("TransactionId: {0}", pValue.TransactionId);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PurchaseError pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasError) pWriter.WriteLine("Error: {0}", pValue.Error);
            if (pValue.HasPurchaseInProgress) pWriter.WriteLine("PurchaseInProgress: {0}", pValue.PurchaseInProgress);
            if (pValue.HasErrorCode) pWriter.WriteLine("ErrorCode: {0}", pValue.ErrorCode);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, GetOptions pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
        }

        public static void Dump(IndentedStreamWriter pWriter, ClientOptions pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("Options: {0}", pValue.OptionsCount);
            if (pValue.OptionsCount > 0)
            {
                pWriter.Indent++;
                pValue.OptionsList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            if (pValue.HasFailed) pWriter.WriteLine("Failed: {0}", pValue.Failed);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PegasusUtil.ClientOption pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasIndex) pWriter.WriteLine("Index: {0}", (ServerOption)pValue.Index);
            if (pValue.HasAsBool) pWriter.WriteLine("AsBool: {0}", pValue.AsBool);
            if (pValue.HasAsInt32) pWriter.WriteLine("AsInt32: {0}", pValue.AsInt32);
            if (pValue.HasAsInt64) pWriter.WriteLine("AsInt64: {0}", pValue.AsInt64);
            if (pValue.HasAsFloat) pWriter.WriteLine("AsFloat: {0}", pValue.AsFloat);
            if (pValue.HasAsUint64) pWriter.WriteLine("AsUint64: {0}", pValue.AsUint64);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, DeckList pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("Decks: {0}", pValue.DecksCount);
            if (pValue.DecksCount > 0)
            {
                pWriter.Indent++;
                pValue.DecksList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, DeckInfo pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasId) pWriter.WriteLine("Id: {0}", pValue.Id);
            if (pValue.HasName) pWriter.WriteLine("Name: {0}", pValue.Name);
            if (pValue.HasBox) pWriter.WriteLine("Box: {0}", pValue.Box);
            if (pValue.HasHero) pWriter.WriteLine("Hero: {0}", (TAG_CLASS)pValue.Hero);
            if (pValue.HasDeckType) pWriter.WriteLine("DeckType: {0}", pValue.DeckType.ToSafeString());
            if (pValue.HasValidity) pWriter.WriteLine("Validity: {0}", (NetCache.DeckFlags)pValue.Validity);
            if (pValue.HasHeroPremium) pWriter.WriteLine("HeroPremium: {0}", (CardFlair.PremiumType)pValue.HeroPremium);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, CardValues pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("Cards: {0}", pValue.CardsCount);
            if (pValue.CardsCount > 0)
            {
                pWriter.Indent++;
                pValue.CardsList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, CardValue pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasCard) Dump(pWriter, pValue.Card);
            if (pValue.HasBuy) pWriter.WriteLine("Buy: {0}", pValue.Buy);
            if (pValue.HasSell) pWriter.WriteLine("Sell: {0}", pValue.Sell);
            if (pValue.HasNerfed) pWriter.WriteLine("Nerfed: {0}", pValue.Nerfed);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PegasusShared.CardDef pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasAsset) pWriter.WriteLine("Asset: {0}", pValue.Asset);
            if (pValue.HasPremium) pWriter.WriteLine("Premium: {0}", (CardFlair.PremiumType)pValue.Premium);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, Collection pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("Stacks: {0}", pValue.StacksCount);
            if (pValue.StacksCount > 0)
            {
                pWriter.Indent++;
                pValue.StacksList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PegasusShared.CardStack pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasCardDef) Dump(pWriter, pValue.CardDef);
            if (pValue.HasLatestInsertDate) pWriter.WriteLine("LatestInsertDate: {0}", pValue.LatestInsertDate.ToString());
            if (pValue.HasCount) pWriter.WriteLine("Count: {0}", pValue.Count);
            if (pValue.HasNumSeen) pWriter.WriteLine("NumSeen: {0}", pValue.NumSeen);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PlayerRecords pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("Records: {0}", pValue.RecordsCount);
            if (pValue.RecordsCount > 0)
            {
                pWriter.Indent++;
                pValue.RecordsList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PlayerRecord pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasType) pWriter.WriteLine("Type: {0}", (NetCache.PlayerRecord.Type)pValue.Type);
            if (pValue.HasData) pWriter.WriteLine("Data: {0}", pValue.Data);
            if (pValue.HasWins) pWriter.WriteLine("Wins: {0}", pValue.Wins);
            if (pValue.HasLosses) pWriter.WriteLine("Losses: {0}", pValue.Losses);
            if (pValue.HasTies) pWriter.WriteLine("Ties: {0}", pValue.Ties);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, ProfileDeckLimit pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasDeckLimit) pWriter.WriteLine("DeckLimit: {0}", pValue.DeckLimit);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, ArcaneDustBalance pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasBalance) pWriter.WriteLine("Balance: {0}", pValue.Balance);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, GoldBalance pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasCappedBalance) pWriter.WriteLine("CappedBalance: {0}", pValue.CappedBalance);
            if (pValue.HasBonusBalance) pWriter.WriteLine("BonusBalance: {0}", pValue.BonusBalance);
            if (pValue.HasCap) pWriter.WriteLine("Cap: {0}", pValue.Cap);
            if (pValue.HasCapWarning) pWriter.WriteLine("CapWarning: {0}", pValue.CapWarning);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, ProfileNotices pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("List: {0}", pValue.ListCount);
            if (pValue.ListCount > 0)
            {
                pWriter.Indent++;
                pValue.ListList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, ProfileNotice pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasEntry) pWriter.WriteLine("Entry: {0}", (NetCache.ProfileNotice.NoticeType)pValue.Entry);
            if (pValue.HasMedal) Dump(pWriter, pValue.Medal);
            if (pValue.HasRewardBooster) Dump(pWriter, pValue.RewardBooster);
            if (pValue.HasRewardCard) Dump(pWriter, pValue.RewardCard);
            if (pValue.HasRewardCard) Dump(pWriter, pValue.PreconDeck);
            if (pValue.HasRewardDust) Dump(pWriter, pValue.RewardDust);
            if (pValue.HasRewardGold) Dump(pWriter, pValue.RewardGold);
            if (pValue.HasRewardMount) Dump(pWriter, pValue.RewardMount);
            if (pValue.HasRewardForge) Dump(pWriter, pValue.RewardForge);
            if (pValue.HasOrigin) pWriter.WriteLine("Origin: {0}", (NetCache.ProfileNotice.NoticeOrigin)pValue.Origin);
            if (pValue.HasOriginData) pWriter.WriteLine("OriginData: {0}", pValue.OriginData);
            if (pValue.HasWhen) pWriter.WriteLine("When: {0}", pValue.When.ToString());
            if (pValue.HasPurchase) Dump(pWriter, pValue.Purchase);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, ProfileNoticeMedal pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasMedal) pWriter.WriteLine("Medal: {0}", (Medal.Type)pValue.Medal);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PegasusShared.ProfileNoticeRewardBooster pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasBoosterType) pWriter.WriteLine("BoosterType: {0}", (BoosterType)pValue.BoosterType);
            if (pValue.HasBoosterCount) pWriter.WriteLine("BoosterCount: {0}", pValue.BoosterCount);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PegasusShared.ProfileNoticeRewardCard pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasCard) Dump(pWriter, pValue.Card);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PegasusShared.ProfileNoticePreconDeck pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasDeck) pWriter.WriteLine("Deck: {0}", pValue.Deck);
            if (pValue.HasHero) pWriter.WriteLine("Hero: {0}", (TAG_CLASS)pValue.Hero);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PegasusShared.ProfileNoticeRewardDust pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasAmount) pWriter.WriteLine("Amount: {0}", pValue.Amount);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PegasusShared.ProfileNoticeRewardGold pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasAmount) pWriter.WriteLine("Amount: {0}", pValue.Amount);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PegasusShared.ProfileNoticeRewardMount pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasMountId) pWriter.WriteLine("MountId: {0}", (MountRewardData.MountType)pValue.MountId);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PegasusShared.ProfileNoticeRewardForge pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasQuantity) pWriter.WriteLine("Quantity: {0}", pValue.Quantity);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PegasusShared.ProfileNoticePurchase pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasTransactionType) pWriter.WriteLine("TransactionType: {0}", pValue.TransactionType);
            if (pValue.HasData) pWriter.WriteLine("Data: {0}", pValue.Data);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, RewardProgress pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasSeasonEnd) pWriter.WriteLine("SeasonEnd: {0}", pValue.SeasonEnd.ToString());
            if (pValue.HasWinsPerGold) pWriter.WriteLine("WinsPerGold: {0}", pValue.WinsPerGold);
            if (pValue.HasGoldPerReward) pWriter.WriteLine("GoldPerReward: {0}", pValue.GoldPerReward);
            if (pValue.HasMaxGoldPerDay) pWriter.WriteLine("MaxGoldPerDay: {0}", pValue.MaxGoldPerDay);
            if (pValue.HasPackId) pWriter.WriteLine("PackId: {0}", pValue.PackId);
            if (pValue.HasXpSoloLimit) pWriter.WriteLine("XpSoloLimit: {0}", pValue.XpSoloLimit);
            if (pValue.HasMaxHeroLevel) pWriter.WriteLine("MaxHeroLevel: {0}", pValue.MaxHeroLevel);
            if (pValue.HasNextQuestCancel) pWriter.WriteLine("NextQuestCancel: {0}", pValue.NextQuestCancel.ToString());
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, HeroXP pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("XpInfos: {0}", pValue.XpInfosCount);
            if (pValue.XpInfosCount > 0)
            {
                pWriter.Indent++;
                pValue.XpInfosList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, HeroXPInfo pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasClassId) pWriter.WriteLine("ClassId: {0}", (TAG_CLASS)pValue.ClassId);
            if (pValue.HasLevel) pWriter.WriteLine("Level: {0}", pValue.Level);
            if (pValue.HasCurrXp) pWriter.WriteLine("CurrXp: {0}", pValue.CurrXp);
            if (pValue.HasMaxXp) pWriter.WriteLine("MaxXp: {0}", pValue.MaxXp);
            if (pValue.HasNextReward) Dump(pWriter, pValue.NextReward);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, NextHeroLevelReward pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasLevel) pWriter.WriteLine("Level: {0}", pValue.Level);
            if (pValue.HasRewardBooster) Dump(pWriter, pValue.RewardBooster);
            if (pValue.HasRewardCard) Dump(pWriter, pValue.RewardCard);
            if (pValue.HasRewardDust) Dump(pWriter, pValue.RewardDust);
            if (pValue.HasRewardGold) Dump(pWriter, pValue.RewardGold);
            if (pValue.HasRewardMount) Dump(pWriter, pValue.RewardMount);
            if (pValue.HasRewardForge) Dump(pWriter, pValue.RewardForge);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, Disconnected pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasAddress) pWriter.WriteLine("Address: {0}", pValue.Address);
            if (pValue.HasGameHandle) pWriter.WriteLine("GameHandle: {0}", pValue.GameHandle);
            if (pValue.HasClientHandle) pWriter.WriteLine("ClientHandle: {0}", pValue.ClientHandle);
            if (pValue.HasPort) pWriter.WriteLine("Port: {0}", pValue.Port);
            if (pValue.HasVersion) pWriter.WriteLine("Version: {0}", pValue.Version);
            if (pValue.HasAuroraPassword) pWriter.WriteLine("AuroraPassword: {0}", pValue.AuroraPassword);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, GetAchieves pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasOnlyActiveOrNewComplete) pWriter.WriteLine("OnlyActiveOrNewComplete: {0}", pValue.OnlyActiveOrNewComplete);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, Achieves pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("List: {0}", pValue.ListCount);
            if (pValue.ListCount > 0)
            {
                pWriter.Indent++;
                pValue.ListList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, Achieve pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasId) pWriter.WriteLine("Id: {0}", pValue.Id);
            if (pValue.HasProgress) pWriter.WriteLine("Progress: {0}", pValue.Progress);
            if (pValue.HasAckProgress) pWriter.WriteLine("AckProgress: {0}", pValue.AckProgress);
            if (pValue.HasCompletionCount) pWriter.WriteLine("CompletionCount: {0}", pValue.CompletionCount);
            if (pValue.HasActive) pWriter.WriteLine("Active: {0}", pValue.Active);
            if (pValue.HasStartedCount) pWriter.WriteLine("StartedCount: {0}", pValue.StartedCount);
            if (pValue.HasDateGiven) pWriter.WriteLine("DateGiven: {0}", pValue.DateGiven.ToString());
            if (pValue.HasDateCompleted) pWriter.WriteLine("DateCompleted: {0}", pValue.DateCompleted.ToString());
            if (pValue.HasDoNotAck) pWriter.WriteLine("DoNotAck: {0}", pValue.DoNotAck);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, GuardianTrack pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasWhat) pWriter.WriteLine("What: {0}", (Network.TrackWhat)pValue.What);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, SetOptions pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("Options: {0}", pValue.OptionsCount);
            if (pValue.OptionsCount > 0)
            {
                pWriter.Indent++;
                pValue.OptionsList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, MedalHistory pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("Medals: {0}", pValue.MedalsCount);
            if (pValue.MedalsCount > 0)
            {
                pWriter.Indent++;
                pValue.MedalsList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, MedalHistoryInfo pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasMedal) pWriter.WriteLine("Medal: {0}", (Medal.Type)pValue.Medal);
            if (pValue.HasWhen) pWriter.WriteLine("When: {0}", pValue.When.ToString());
            if (pValue.HasRank) pWriter.WriteLine("Rank: {0}", pValue.Rank);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, GamesInfo pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasGamesStarted) pWriter.WriteLine("GamesStarted: {0}", pValue.GamesStarted);
            if (pValue.HasGamesWon) pWriter.WriteLine("GamesWon: {0}", pValue.GamesWon);
            if (pValue.HasGamesLost) pWriter.WriteLine("GamesLost: {0}", pValue.GamesLost);
            if (pValue.HasFreeRewardProgress) pWriter.WriteLine("FreeRewardProgress: {0}", pValue.FreeRewardProgress);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, AckNotice pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasEntry) pWriter.WriteLine("Entry: {0}", pValue.Entry);
            pWriter.Indent--;
        }






        public static void DumpGamePacket(IndentedStreamWriter pWriter, int pPacketID, byte[] pBody)
        {
            switch (pPacketID)
            {
                case (int)AuroraHandshake.Types.PacketID.ID: Dump(pWriter, AuroraHandshake.ParseFrom(pBody)); break;
                case (int)BeginPlaying.Types.PacketID.ID: Dump(pWriter, BeginPlaying.ParseFrom(pBody)); break;
                case (int)GameStarting.Types.PacketID.ID: Dump(pWriter, GameStarting.ParseFrom(pBody)); break;
                case (int)GameSetup.Types.PacketID.ID: Dump(pWriter, GameSetup.ParseFrom(pBody)); break;
                case (int)StartGameState.Types.PacketID.ID: Dump(pWriter, StartGameState.ParseFrom(pBody)); break;
                case (int)FinishGameState.Types.PacketID.ID: Dump(pWriter, FinishGameState.ParseFrom(pBody)); break;
                case (int)PowerHistory.Types.PacketID.ID: Dump(pWriter, PowerHistory.ParseFrom(pBody)); break;
                case (int)GetGameState.Types.PacketID.ID: Dump(pWriter, GetGameState.ParseFrom(pBody)); break;
                case (int)EntityChoice.Types.PacketID.ID: Dump(pWriter, EntityChoice.ParseFrom(pBody)); break;
                case (int)ChooseEntities.Types.PacketID.ID: Dump(pWriter, ChooseEntities.ParseFrom(pBody)); break;
                case (int)PreLoad.Types.PacketID.ID: Dump(pWriter, PreLoad.ParseFrom(pBody)); break;
                case (int)AllOptions.Types.PacketID.ID: Dump(pWriter, AllOptions.ParseFrom(pBody)); break;
                case (int)ChooseOption.Types.PacketID.ID: Dump(pWriter, ChooseOption.ParseFrom(pBody)); break;
                default: pWriter.WriteLine("UnhandledBody"); break;
            }
        }

        public static void Dump(IndentedStreamWriter pWriter, AuroraHandshake pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasClientHandle) pWriter.WriteLine("ClientHandle: {0}", pValue.ClientHandle);
            if (pValue.HasGameHandle) pWriter.WriteLine("GameHandle: {0}", pValue.GameHandle);
            if (pValue.HasPassword) pWriter.WriteLine("Password: {0}", pValue.Password);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, BeginPlaying pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasMode) pWriter.WriteLine("Mode: {0}", pValue.Mode.ToSafeString());
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, GameStarting pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasGameHandle) pWriter.WriteLine("GameHandle: {0}", pValue.GameHandle);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, GameSetup pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasBoard) pWriter.WriteLine("Board: {0}", pValue.Board);
            pWriter.WriteLine("Clients: {0}", pValue.ClientsCount);
            if (pValue.ClientsCount > 0)
            {
                pWriter.Indent++;
                pValue.ClientsList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            if (pValue.HasMaxSecretsPerPlayer) pWriter.WriteLine("MaxSecretsPerPlayer: {0}", pValue.MaxSecretsPerPlayer);
            if (pValue.HasMaxFriendlyMinionsPerPlayer) pWriter.WriteLine("MaxFriendlyMinionsPerPlayer: {0}", pValue.MaxFriendlyMinionsPerPlayer);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, ClientInfo pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("Pieces: {0}", pValue.PiecesCount);
            if (pValue.PiecesCount > 0)
            {
                pWriter.Indent++;
                pValue.PiecesList.ForEach(v => pWriter.WriteLine("Piece: {0}", v));
                pWriter.Indent--;
            }
            if (pValue.HasCardBack) pWriter.WriteLine("CardBack: {0}", pValue.CardBack);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, StartGameState pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasGameEntity)
            {
                pWriter.WriteLine("GameEntity");
                pWriter.Indent++;
                Dump(pWriter, pValue.GameEntity);
                pWriter.Indent--;
            }
            pWriter.WriteLine("Players: {0}", pValue.PlayersCount);
            if (pValue.PlayersCount > 0)
            {
                pWriter.Indent++;
                pValue.PlayersList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PegasusGame.Entity pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasId) pWriter.WriteLine("Id: {0}", pValue.Id);
            pWriter.WriteLine("Tags: {0}", pValue.TagsCount);
            if (pValue.TagsCount > 0)
            {
                pWriter.Indent++;
                pValue.TagsList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, Tag pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasName) pWriter.WriteLine("Name: {0}", (GAME_TAG)pValue.Name);
            if (pValue.HasValue) pWriter.WriteLine("Value: {0}", pValue.Value);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PegasusGame.Player pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasId) pWriter.WriteLine("Id: {0}", pValue.Id);
            if (pValue.HasGameAccountId)
            {
                pWriter.WriteLine("GameAccountId");
                pWriter.Indent++;
                Dump(pWriter, pValue.GameAccountId);
                pWriter.Indent--;
            }
            if (pValue.HasEntity)
            {
                pWriter.WriteLine("Entity");
                pWriter.Indent++;
                Dump(pWriter, pValue.Entity);
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, BnetId pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasHi) pWriter.WriteLine("Hi: {0}", pValue.Hi);
            if (pValue.HasLo) pWriter.WriteLine("Lo: {0}", pValue.Lo);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, FinishGameState pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
        }

        public static void Dump(IndentedStreamWriter pWriter, PowerHistory pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("List: {0}", pValue.ListCount);
            if (pValue.ListCount > 0)
            {
                pWriter.Indent++;
                pValue.ListList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PowerHistoryData pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasFullEntity)
            {
                pWriter.WriteLine("FullEntity");
                pWriter.Indent++;
                Dump(pWriter, pValue.FullEntity);
                pWriter.Indent--;
            }
            if (pValue.HasShowEntity)
            {
                pWriter.WriteLine("ShowEntity");
                pWriter.Indent++;
                Dump(pWriter, pValue.ShowEntity);
                pWriter.Indent--;
            }
            if (pValue.HasHideEntity)
            {
                pWriter.WriteLine("HideEntity");
                pWriter.Indent++;
                Dump(pWriter, pValue.HideEntity);
                pWriter.Indent--;
            }
            if (pValue.HasTagChange)
            {
                pWriter.WriteLine("TagChange");
                pWriter.Indent++;
                Dump(pWriter, pValue.TagChange);
                pWriter.Indent--;
            }
            if (pValue.HasCreateGame)
            {
                pWriter.WriteLine("CreateGame");
                pWriter.Indent++;
                Dump(pWriter, pValue.CreateGame);
                pWriter.Indent--;
            }
            if (pValue.HasPowerStart)
            {
                pWriter.WriteLine("PowerStart");
                pWriter.Indent++;
                Dump(pWriter, pValue.PowerStart);
                pWriter.Indent--;
            }
            if (pValue.HasPowerEnd)
            {
                pWriter.WriteLine("PowerEnd");
                pWriter.Indent++;
                Dump(pWriter, pValue.PowerEnd);
                pWriter.Indent--;
            }
            if (pValue.HasMetaData)
            {
                pWriter.WriteLine("MetaData");
                pWriter.Indent++;
                Dump(pWriter, pValue.MetaData);
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PowerHistoryEntity pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasEntity) pWriter.WriteLine("Entity: {0}", pValue.Entity);
            if (pValue.HasName) pWriter.WriteLine("Name: {0}", pValue.Name);
            pWriter.WriteLine("Tags: {0}", pValue.TagsCount);
            if (pValue.TagsCount > 0)
            {
                pWriter.Indent++;
                pValue.TagsList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PowerHistoryHide pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasEntity) pWriter.WriteLine("Entity: {0}", pValue.Entity);
            if (pValue.HasZone) pWriter.WriteLine("Zone: {0}", pValue.Zone);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PowerHistoryTagChange pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasEntity) pWriter.WriteLine("Entity: {0}", pValue.Entity);
            if (pValue.HasTag) pWriter.WriteLine("Tag: {0}", (GAME_TAG)pValue.Tag);
            if (pValue.HasValue) pWriter.WriteLine("Value: {0}", pValue.Value);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PowerHistoryCreateGame pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasGameEntity)
            {
                pWriter.WriteLine("GameEntity");
                pWriter.Indent++;
                Dump(pWriter, pValue.GameEntity);
                pWriter.Indent--;
            }
            pWriter.WriteLine("Players: {0}", pValue.PlayersCount);
            if (pValue.PlayersCount > 0)
            {
                pWriter.Indent++;
                pValue.PlayersList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PowerHistoryStart pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasType) pWriter.WriteLine("Type: {0}", pValue.Type.ToSafeString());
            if (pValue.HasIndex) pWriter.WriteLine("Index: {0}", pValue.Index);
            if (pValue.HasSource) pWriter.WriteLine("Source: {0}", pValue.Source);
            if (pValue.HasTarget) pWriter.WriteLine("Target: {0}", pValue.Target);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PowerHistoryEnd pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
        }

        public static void Dump(IndentedStreamWriter pWriter, PowerHistoryMetaData pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("Info: {0}", pValue.InfoCount);
            if (pValue.InfoCount > 0)
            {
                pWriter.Indent++;
                pValue.InfoList.ForEach(v => pWriter.WriteLine("Info: {0}", v));
                pWriter.Indent--;
            }
            if (pValue.HasMetaType) pWriter.WriteLine("MetaType: {0}", pValue.MetaType.ToSafeString());
            if (pValue.HasData) pWriter.WriteLine("Data: {0}", pValue.Data);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, GetGameState pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
        }

        public static void Dump(IndentedStreamWriter pWriter, EntityChoice pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasId) pWriter.WriteLine("Id: {0}", pValue.Id);
            if (pValue.HasChoiceType) pWriter.WriteLine("ChoiceType: {0}", pValue.ChoiceType);
            if (pValue.HasCancelable) pWriter.WriteLine("Cancelable: {0}", pValue.Cancelable);
            if (pValue.HasCountMin) pWriter.WriteLine("CountMin: {0}", pValue.CountMin);
            if (pValue.HasCountMax) pWriter.WriteLine("CountMax: {0}", pValue.CountMax);
            pWriter.WriteLine("Entities: {0}", pValue.EntitiesCount);
            if (pValue.EntitiesCount > 0)
            {
                pWriter.Indent++;
                pValue.EntitiesList.ForEach(v => pWriter.WriteLine("Entity: {0}", v));
                pWriter.Indent--;
            }
            if (pValue.HasSource) pWriter.WriteLine("Source: {0}", pValue.Source);
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, ChooseEntities pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasId) pWriter.WriteLine("Id: {0}", pValue.Id);
            pWriter.WriteLine("Entities: {0}", pValue.EntitiesCount);
            if (pValue.EntitiesCount > 0)
            {
                pWriter.Indent++;
                pValue.EntitiesList.ForEach(v => pWriter.WriteLine("Entity: {0}", v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PreLoad pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            pWriter.WriteLine("Cards: {0}", pValue.CardsCount);
            if (pValue.CardsCount > 0)
            {
                pWriter.Indent++;
                pValue.CardsList.ForEach(v => pWriter.WriteLine("Card: {0}", v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, AllOptions pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasId) pWriter.WriteLine("Id: {0}", pValue.Id);
            pWriter.WriteLine("Options: {0}", pValue.OptionsCount);
            if (pValue.OptionsCount > 0)
            {
                pWriter.Indent++;
                pValue.OptionsList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, PegasusGame.Option pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasType) pWriter.WriteLine("Type: {0}", pValue.Type.ToSafeString());
            if (pValue.HasMainOption)
            {
                pWriter.WriteLine("MainOption");
                pWriter.Indent++;
                Dump(pWriter, pValue.MainOption);
                pWriter.Indent--;
            }
            pWriter.WriteLine("SubOptions: {0}", pValue.SubOptionsCount);
            if (pValue.SubOptionsCount > 0)
            {
                pWriter.Indent++;
                pValue.SubOptionsList.ForEach(v => Dump(pWriter, v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, SubOption pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasId) pWriter.WriteLine("Id: {0}", pValue.Id);
            pWriter.WriteLine("Targets: {0}", pValue.TargetsCount);
            if (pValue.TargetsCount > 0)
            {
                pWriter.Indent++;
                pValue.TargetsList.ForEach(v => pWriter.WriteLine("Target: {0}", v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }

        public static void Dump(IndentedStreamWriter pWriter, ChooseOption pValue)
        {
            pWriter.WriteLine(pValue.GetType().Name);
            pWriter.Indent++;
            if (pValue.HasId) pWriter.WriteLine("Id: {0}", pValue.Id);
            if (pValue.HasIndex) pWriter.WriteLine("Index: {0}", pValue.Index);
            if (pValue.HasTarget) pWriter.WriteLine("Target: {0}", pValue.Target);
            if (pValue.HasSubOption) pWriter.WriteLine("SubOption: {0}", pValue.SubOption);
            if (pValue.HasPosition) pWriter.WriteLine("Position: {0}", pValue.Position);
            pWriter.Indent--;
        }
    }
}
