using BobNetProto;
using PegasusGame;
using Stonehearth.Lobby;
using Stonehearth.Properties;
using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;

namespace Stonehearth.Game
{
    public static class GamePacketHandlers
    {
        [GamePacketHandler((int)AuroraHandshake.Types.PacketID.ID, GameClientProtocolState.AuroraHandshake)]
        public static void AuroraHandshakeHandler(GameClient pClient, Packet pPacket)
        {
            AuroraHandshake auroraHandshake = AuroraHandshake.ParseFrom(pPacket.ReadAll());
            pClient.Log(LogManagerLevel.Debug, "AuroraHandshake: {0}, {1}, {2}", auroraHandshake.GameHandle, auroraHandshake.ClientHandle, auroraHandshake.Password);

            if (auroraHandshake.GameHandle == 0 || auroraHandshake.ClientHandle == 0)
            {
                pClient.Disconnect();
                return;
            }

            Match match = null;
            if (!MatchManager.MatchByGameHandle.TryGetValue(auroraHandshake.GameHandle, out match))
            {
                pClient.Disconnect();
                return;
            }
            LobbyClient lobbyClient = null;
            if (!MatchManager.LobbyClientByClientHandle.TryGetValue(auroraHandshake.ClientHandle, out lobbyClient))
            {
                pClient.Disconnect();
                return;
            }
            MatchPlayer matchPlayer = match.Players.Find(p => p.ClientHandle == auroraHandshake.ClientHandle);
            if (matchPlayer == null)
            {
                pClient.Disconnect();
                return;
            }
            if (pClient.Host != lobbyClient.Host)
            {
                pClient.Disconnect();
                return;
            }
            if (auroraHandshake.Password != match.Password)
            {
                pClient.Disconnect();
                return;
            }

            matchPlayer.Client = pClient;

            pClient.ProtocolState = GameClientProtocolState.GetGameState;
            pClient.Account = lobbyClient.Account;
            pClient.Match = match;
            pClient.MatchPlayer = matchPlayer;

            GameStarting.Builder gameStarting = GameStarting.CreateBuilder();
            gameStarting.SetGameHandle(match.GameHandle); // TODO: Remove, not sent in logs?
            pClient.SendPacket(new Packet((int)GameStarting.Types.PacketID.ID, gameStarting.Build().ToByteArray()));
        }

        [GamePacketHandler((int)BeginPlaying.Types.PacketID.ID, GameClientProtocolState.GetGameState)]
        public static void BeginPlayingHandler(GameClient pClient, Packet pPacket)
        {
            BeginPlaying beginPlaying = BeginPlaying.ParseFrom(pPacket.ReadAll());
            pClient.Log(LogManagerLevel.Debug, "BeginPlaying: {0}", beginPlaying.Mode.ToSafeString());

            Match match = pClient.Match;
            List<MatchPlayer> realPlayers = match.Players.FindAll(p => !p.AI);
            if (Interlocked.Increment(ref match.PlayersReady) == realPlayers.Count)
            {
                // This is only executed when the last non-AI player enters each mode
                match.PlayersReady = 0;
                switch (beginPlaying.Mode)
                {
                    case BeginPlaying.Types.Mode.COUNTDOWN:
                        {
                            GameSetup.Builder gameSetup = GameSetup.CreateBuilder();
                            gameSetup.SetBoard("MOP"); // STR, MOP, ORG, ?
                            ClientInfo.Builder clientInfoBuilder = ClientInfo.CreateBuilder().AddPieces(1).AddPieces(2).AddPieces(3).AddPieces(4).SetCardBack(0);
                            pClient.Match.Players.ForEach(p => gameSetup.AddClients(clientInfoBuilder));
                            gameSetup.SetMaxSecretsPerPlayer(5);
                            gameSetup.SetMaxFriendlyMinionsPerPlayer(7);

                            foreach (MatchPlayer matchPlayer in match.Players)
                            {
                                if (matchPlayer.AI) continue;
                                matchPlayer.Client.SendPacket(new Packet((int)GameSetup.Types.PacketID.ID, gameSetup.Build().ToByteArray()));
                            }
                            break;
                        }
                    case BeginPlaying.Types.Mode.READY:
                        {
                            //match.FirstPlayer = match.Players[new Random().Next(1, match.Players.Count) - 1];
                            match.FirstPlayer = match.Players[0];
                            match.CurrentPlayer = match.FirstPlayer;

                            match.SendPowerHistoryTagChange(match.GameEntity.SetTag(GAME_TAG.STATE, (int)TAG_STATE.RUNNING));
                            match.Players.ForEach(p => match.SendPowerHistoryTagChange(p.SetTag(GAME_TAG.PLAYSTATE, (int)TAG_PLAYSTATE.PLAYING)));
                            
                            //match.SendPowerHistoryStart(PowerHistoryStart.Types.Type.TRIGGER, -1, match.GameEntity.EntityID, 0);
                            match.SendPowerHistoryTagChange(match.CurrentPlayer.SetTag(GAME_TAG.CURRENT_PLAYER, 1));
                            match.SendPowerHistoryTagChange(match.CurrentPlayer.SetTag(GAME_TAG.FIRST_PLAYER, 1));
                            match.SendPowerHistoryTagChange(match.GameEntity.SetTag(GAME_TAG.TURN, match.Turn));
                            foreach (MatchPlayer matchPlayer in match.Players)
                            {
                                int drawCount = matchPlayer == match.FirstPlayer ? 3 : 4;
                                while (drawCount-- > 0)
                                {
                                    MatchCard matchCard = matchPlayer.DrawCard();
                                    matchPlayer.SendPowerHistoryShowEntity(matchCard.ToShownPowerHistoryEntity());
                                    match.SendPowerHistoryZoneChange(matchCard, matchPlayer);
                                }
                                match.SendPowerHistoryTagChange(matchPlayer.SetTag(GAME_TAG.NUM_TURNS_LEFT, 1));
                                if (matchPlayer == match.FirstPlayer) continue;

                                MatchCard coinCard = new MatchCard(match, matchPlayer, CardManager.TheCoinCard);
                                matchPlayer.HandCards.Add(coinCard);

                                coinCard.SetZoneAndPositionTags(TAG_ZONE.HAND, matchPlayer.HandCards.Count);
                                coinCard.SetTag(GAME_TAG.CREATOR, matchPlayer.EntityID);

                                matchPlayer.SendPowerHistoryFullEntity(coinCard.ToShownPowerHistoryEntity());
                                match.SendPowerHistoryFullEntity(coinCard.ToHiddenPowerHistoryEntity(), matchPlayer);
                            }
                            //match.SendPowerHistoryTagChange(match.GameEntity.SetTag(GAME_TAG.NEXT_STEP, (int)TAG_STEP.BEGIN_MULLIGAN));
                            //match.SendPowerHistoryEnd();

                            match.SendPowerHistoryTagChange(match.GameEntity.SetTag(GAME_TAG.STEP, (int)TAG_STEP.BEGIN_MULLIGAN));

                            //match.SendPowerHistoryStart(PowerHistoryStart.Types.Type.TRIGGER, -1, match.GameEntity.EntityID, 0);
                            match.Players.ForEach(p => match.SendPowerHistoryTagChange(p.SetTag(GAME_TAG.MULLIGAN_STATE, (int)TAG_MULLIGAN.INPUT)));
                            match.SendPowerHistoryTagChange(match.GameEntity.SetTag(GAME_TAG.TURN_START, DateTime.UtcNow.ToUnixTimestamp()));
                            //match.SendPowerHistoryEnd();

                            foreach (MatchPlayer matchPlayer in match.Players)
                            {
                                if (!matchPlayer.AI) continue;

                                match.SendPowerHistoryTagChange(matchPlayer.SetTag(GAME_TAG.MULLIGAN_STATE, (int)TAG_MULLIGAN.DEALING));

                                //match.SendPowerHistoryStart(PowerHistoryStart.Types.Type.TRIGGER, -1, matchPlayer.EntityID, 0);
                                match.SendPowerHistoryTagChange(matchPlayer.SetTag(GAME_TAG.MULLIGAN_STATE, (int)TAG_MULLIGAN.WAITING));
                                //match.SendPowerHistoryEnd();

                                //match.SendPowerHistoryStart(PowerHistoryStart.Types.Type.TRIGGER, -1, matchPlayer.EntityID, 0);
                                match.SendPowerHistoryTagChange(matchPlayer.SetTag(GAME_TAG.MULLIGAN_STATE, (int)TAG_MULLIGAN.DONE));
                                //match.SendPowerHistoryEnd();
                            }

                            match.FlushPowerHistory();

                            foreach (MatchPlayer matchPlayer in match.Players)
                            {
                                if (matchPlayer.AI) continue;

                                EntityChoice.Builder entityChoiceBuilder = EntityChoice.CreateBuilder();
                                entityChoiceBuilder.SetId(matchPlayer.PlayerID);
                                entityChoiceBuilder.SetChoiceType((int)CHOICE_TYPE.MULLIGAN);
                                entityChoiceBuilder.SetCancelable(false);
                                entityChoiceBuilder.SetCountMin(0);
                                entityChoiceBuilder.SetCountMax(matchPlayer.HandCards.Count);
                                matchPlayer.HandCards.ForEach(c => entityChoiceBuilder.AddEntities(c.EntityID));
                                entityChoiceBuilder.SetSource(matchPlayer.PlayerID);
                                matchPlayer.Client.SendPacket(new Packet((int)EntityChoice.Types.PacketID.ID, entityChoiceBuilder.Build().ToByteArray()));
                            }
                            break;
                        }
                    default: break;
                }
            }
        }

        [GamePacketHandler((int)GetGameState.Types.PacketID.ID, GameClientProtocolState.GetGameState)]
        public static void GetGameStateHandler(GameClient pClient, Packet pPacket)
        {
            GetGameState getGameState = GetGameState.ParseFrom(pPacket.ReadAll());
            pClient.Log(LogManagerLevel.Debug, "GetGameState");

            StartGameState.Builder startGameState = StartGameState.CreateBuilder();
            startGameState.SetGameEntity(pClient.Match.GameEntity.ToEntity());
            foreach (MatchPlayer matchPlayer in pClient.Match.Players)
                startGameState.AddPlayers(matchPlayer.ToPlayer());
            pClient.SendPacket(new Packet((int)StartGameState.Types.PacketID.ID, startGameState.Build().ToByteArray()));

            pClient.SendPacket(new Packet((int)FinishGameState.Types.PacketID.ID, FinishGameState.CreateBuilder().Build().ToByteArray()));

            PowerHistoryCreateGame.Builder powerHistoryCreateGame = PowerHistoryCreateGame.CreateBuilder();
            powerHistoryCreateGame.SetGameEntity(pClient.Match.GameEntity.ToEntity());
            pClient.Match.Players.ForEach(p => powerHistoryCreateGame.AddPlayers(p.ToPlayer()));
            pClient.MatchPlayer.SendPowerHistoryData(PowerHistoryData.CreateBuilder().SetCreateGame(powerHistoryCreateGame).Build());

            foreach (MatchPlayer matchPlayer in pClient.Match.Players)
            {
                pClient.MatchPlayer.SendPowerHistoryFullEntity(matchPlayer.HeroCard.ToShownPowerHistoryEntity());
                pClient.MatchPlayer.SendPowerHistoryFullEntity(matchPlayer.HeroPowerCard.ToShownPowerHistoryEntity());
                matchPlayer.DeckCards.ForEach(c => pClient.MatchPlayer.SendPowerHistoryFullEntity(c.ToHiddenPowerHistoryEntity()));
            }

            pClient.MatchPlayer.FlushPowerHistory();
        }

        [GamePacketHandler((int)ChooseEntities.Types.PacketID.ID, GameClientProtocolState.GetGameState)]
        public static void ChooseEntitiesHandler(GameClient pClient, Packet pPacket)
        {
            ChooseEntities chooseEntities = ChooseEntities.ParseFrom(pPacket.ReadAll());
            pClient.Log(LogManagerLevel.Debug, "ChooseEntities");

            PreLoad.Builder preLoad = PreLoad.CreateBuilder();
            pClient.SendPacket(new Packet((int)PreLoad.Types.PacketID.ID, preLoad.Build().ToByteArray()));

            Random random = new Random();

            pClient.Match.SendPowerHistoryTagChange(pClient.MatchPlayer.SetTag(GAME_TAG.MULLIGAN_STATE, (int)TAG_MULLIGAN.DEALING));

            //pClient.Match.SendPowerHistoryStart(PowerHistoryStart.Types.Type.TRIGGER, -1, pClient.MatchPlayer.EntityID, 0);
            List<MatchCard> returnedCards = pClient.MatchPlayer.HandCards.Where(c => c.Card != CardManager.TheCoinCard && !chooseEntities.EntitiesList.Contains(c.EntityID)).ToList();
            foreach (MatchCard returnedCard in returnedCards)
            {
                MatchCard newCard = pClient.MatchPlayer.DrawCard();
                pClient.MatchPlayer.HandCards.Remove(returnedCard);
                pClient.MatchPlayer.DeckCards.Insert(random.Next(0, pClient.MatchPlayer.DeckCards.Count - 1), returnedCard);

                newCard.SetZoneAndPositionTags(TAG_ZONE.HAND, returnedCard.GetTag(GAME_TAG.ZONE_POSITION).Value);
                returnedCard.SetZoneAndPositionTags(TAG_ZONE.DECK, 0);

                pClient.MatchPlayer.SendPowerHistoryShowEntity(newCard.ToShownPowerHistoryEntity());
                pClient.Match.SendPowerHistoryZoneChange(newCard, pClient.MatchPlayer);

                pClient.Match.SendPowerHistoryHideEntity(returnedCard.EntityID, TAG_ZONE.DECK);
                pClient.Match.SendPowerHistoryZoneChange(returnedCard);
            }
            pClient.Match.SendPowerHistoryTagChange(pClient.MatchPlayer.SetTag(GAME_TAG.MULLIGAN_STATE, (int)TAG_MULLIGAN.WAITING));
            //pClient.Match.SendPowerHistoryEnd();

            //pClient.Match.SendPowerHistoryStart(PowerHistoryStart.Types.Type.TRIGGER, -1, pClient.MatchPlayer.EntityID, 0);
            pClient.Match.SendPowerHistoryTagChange(pClient.MatchPlayer.SetTag(GAME_TAG.MULLIGAN_STATE, (int)TAG_MULLIGAN.DONE));
            pClient.Match.SendPowerHistoryTagChange(pClient.Match.FirstPlayer.SetTag(GAME_TAG.TURN_START, DateTime.UtcNow.ToUnixTimestamp()));
            //pClient.Match.SendPowerHistoryTagChange(pClient.Match.GameEntity.SetTag(GAME_TAG.NEXT_STEP, (int)TAG_STEP.MAIN_READY));
            //pClient.Match.SendPowerHistoryEnd();

            pClient.Match.FlushPowerHistory();

            List<MatchPlayer> realPlayers = pClient.Match.Players.FindAll(p => !p.AI);
            if (Interlocked.Increment(ref pClient.Match.PlayersReady) == realPlayers.Count)
            {
                // This is only executed when the last non-AI player gets here
                pClient.Match.PlayersReady = 0;
                pClient.Match.StartTurn();
            }
        }
    }
}
