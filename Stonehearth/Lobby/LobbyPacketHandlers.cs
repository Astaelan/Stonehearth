﻿using Dapper;
using Stonehearth.Properties;
using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Stonehearth.Lobby
{
    public static class LobbyPacketHandlers
    {
        [LobbyPacketHandler((int)InternalPacketID.Handshake, LobbyClientProtocolState.Handshake)]
        public static void HandshakeHandler(LobbyClient pClient, Packet pPacket)
        {
            int version = 0;
            bool authorize = false;
            if (!pPacket.ReadInt(out version) ||
                version != (int)ProtocolVersion.Current ||
                !pPacket.ReadBool(out authorize))
            {
                pClient.Disconnect();
                return;
            }

            pClient.ProtocolState = LobbyClientProtocolState.Authenticate;
            if (authorize) pClient.ProtocolState = LobbyClientProtocolState.Authorize;

            Packet packet = new Packet((int)InternalPacketID.Handshake);
            packet.WriteInt(0);
            if (!authorize) packet.WriteGuid(pClient.AuthenticateSalt);
            pClient.SendPacket(packet);
        }

        [LobbyPacketHandler((int)InternalPacketID.Authenticate, LobbyClientProtocolState.Authenticate)]
        public static void AuthenticateHandler(LobbyClient pClient, Packet pPacket)
        {
            string email = null;
            string passwordHash = null;
            if (!pPacket.ReadString(out email) ||
                !pPacket.ReadString(out passwordHash))
            {
                pClient.Disconnect();
                return;
            }
            email = email.ToLower();

            Packet packet = new Packet((int)InternalPacketID.Authenticate);
            using (SqlConnection db = DB.Open(Settings.Default.Database))
            {
                using (SqlDataReader dr = db.ExecuteReader(null, "SELECT [Password] FROM [Account] WHERE [Email]=@0", email))
                {
                    if (!dr.Read())
                    {
                        packet.WriteInt(1);
                        pClient.SendPacket(packet, true);
                        return;
                    }
                    if (passwordHash != PasswordHasher.HashStrings(pClient.AuthenticateSalt.ToString(), (string)dr["Password"]))
                    {
                        packet.WriteInt(2);
                        pClient.SendPacket(packet, true);
                        return;
                    }
                }
                db.Execute(null,
                           "UPDATE [Account] SET [SessionID]=@0,[SessionHost]=@1,[SessionExpiration]=@2 WHERE [Email]=@3",
                           pClient.AuthenticateSession, pClient.Host, DateTime.UtcNow.AddMinutes(1), email);
            }

            pClient.ProtocolState = LobbyClientProtocolState.None;

            pClient.Log(LogManagerLevel.Info, "Authenticated: {0}", pClient.AuthenticateSession);

            packet.WriteInt(0);
            packet.WriteGuid(pClient.AuthenticateSession);
            pClient.SendPacket(packet, true);
        }

        [LobbyPacketHandler((int)InternalPacketID.Authorize, LobbyClientProtocolState.Authorize)]
        public static void AuthorizeHandler(LobbyClient pClient, Packet pPacket)
        {
            Guid session = Guid.Empty;
            if (!pPacket.ReadGuid(out session))
            {
                pClient.Disconnect();
                return;
            }

            Packet packet = new Packet((int)InternalPacketID.Authorize);
            using (SqlConnection db = DB.Open(Settings.Default.Database))
            {
                long accountID = 0;
                using (SqlDataReader dr = db.ExecuteReader(null, "SELECT [AccountID],[SessionHost] FROM [Account] WHERE [SessionID]=@0", session))
                {
                    if (!dr.Read())
                    {
                        packet.WriteInt(1);
                        pClient.SendPacket(packet, true);
                        return;
                    }
                    if (pClient.Host != (string)dr["SessionHost"])
                    {
                        packet.WriteInt(2);
                        pClient.SendPacket(packet, true);
                        return;
                    }
                    accountID = (long)dr["AccountID"];
                }
                db.Execute(null, "UPDATE [Account] SET [SessionID]=NULL,[SessionHost]=NULL,[SessionExpiration]=NULL WHERE [AccountID]=@0", accountID);
                pClient.Account = Data.Account.Load(db, accountID);
            }


            pClient.ProtocolState = LobbyClientProtocolState.Online;

            pClient.Log(LogManagerLevel.Info, "Authorized: {0}", session);

            packet.WriteInt(0);
            packet.WriteLong(pClient.Account.AccountID);
            pClient.SendPacket(packet);
        }

        [LobbyPacketHandler((int)InternalPacketID.UtilPacket, LobbyClientProtocolState.Online)]
        public static void UtilPacketHandler(LobbyClient pClient, Packet pPacket)
        {
            int packetId = 0;
            int context = 0;
            byte[] data = null;
            if (!pPacket.ReadInt(out packetId) ||
                !pPacket.ReadInt(out context) ||
                !pPacket.ReadBytes(out data))
            {
                pClient.Disconnect();
                return;
            }

            LobbyUtilPacketHandlerAttribute handler = null;
            if (!PacketHandlerManager.LobbyUtilPacketHandlers.TryGetValue(packetId, out handler))
            {
                pClient.Log(LogManagerLevel.Warn, "Unhandled UtilPacket Opcode: {0}", packetId);
                //pClient.Disconnect();
            }
            else if (pClient.ProtocolState != LobbyClientProtocolState.Online)
            {
                pClient.Log(LogManagerLevel.Error, "Invalid UtilPacket Protocol State: {0}, {1}", pClient.ProtocolState, packetId);
                pClient.Disconnect();
            }
            else handler.Processor(pClient, context, data);
        }

        [LobbyPacketHandler((int)InternalPacketID.StartScenario, LobbyClientProtocolState.Online)]
        public static void StartScenarioHandler(LobbyClient pClient, Packet pPacket)
        {
            int scenario = 0;
            long deckID = 0;
            if (!pPacket.ReadInt(out scenario) ||
                !pPacket.ReadLong(out deckID))
            {
                pClient.Disconnect();
                return;
            }

            Match match = MatchManager.CreateMatch();
            MatchPlayer matchPlayer1 = null;
            List<Data.Card> player1Cards = new List<Data.Card>();

            Data.AccountDeck accountDeck = pClient.Account.Decks.Find(d => d.AccountDeckID == deckID);
            if (accountDeck == null)
            {
                MatchManager.MatchByGameHandle.Remove(match.GameHandle);
                pClient.Disconnect();
                return;
            }
            TAG_CLASS player1Class = (TAG_CLASS)accountDeck.Hero;
            foreach (Data.AccountDeckCard accountDeckCard in accountDeck.Cards)
            {
                for (int index = 0; index < accountDeckCard.Quantity; ++index)
                    player1Cards.Add(CardManager.CardsByCardID[accountDeckCard.CardID]);
            }

            matchPlayer1 = match.CreatePlayer(false,
                                                CardManager.CoreHeroCardsByClassID[player1Class],
                                                CardManager.CoreHeroPowerCardsByClassID[player1Class],
                                                player1Cards,
                                                pClient.ClientHandle,
                                                pClient.Account.AccountID);

            // TODO: Temporary for now, until decks are in for AI's
            MatchPlayer matchPlayer2 = match.CreatePlayer(true,
                                                          matchPlayer1.HeroCardData,
                                                          matchPlayer1.HeroPowerCardData,
                                                          player1Cards);

            Packet packet = new Packet((int)InternalPacketID.StartScenario);
            packet.WriteInt(0);
            packet.WriteString("127.0.0.1");
            packet.WriteUShort(Settings.Default.GamePort);
            packet.WriteString(match.Password);
            packet.WriteInt(match.GameHandle);
            packet.WriteLong(pClient.ClientHandle);
            packet.WriteString("Version");
            pClient.SendPacket(packet);
        }
    }
}
