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
            pClient.AccountID = lobbyClient.AccountID;
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
                            foreach (MatchPlayer matchPlayer in pClient.Match.Players) gameSetup.AddClients(ClientInfo.CreateBuilder().AddPieces(1).AddPieces(2).AddPieces(3).AddPieces(4).SetCardBack(0));
                            gameSetup.SetMaxSecretsPerPlayer(5);
                            gameSetup.SetMaxFriendlyMinionsPerPlayer(7);

                            foreach (MatchPlayer matchPlayer in realPlayers)
                                matchPlayer.Client.SendPacket(new Packet((int)GameSetup.Types.PacketID.ID, gameSetup.Build().ToByteArray()));
                            break;
                        }
                    case BeginPlaying.Types.Mode.READY:
                        {
                            match.FirstPlayer = match.Players[new Random().Next(0, match.Players.Count - 1)];
                            match.CurrentPlayer = match.FirstPlayer;
                            foreach (MatchPlayer player in realPlayers)
                            {
                                player.PowerHistoryBuilder.ClearList();
                                player.PowerHistoryBuilder.AddList(pClient.Match.GameEntity.SetTag(GAME_TAG.STATE, (int)TAG_STATE.RUNNING));
                                foreach (MatchPlayer matchPlayer in pClient.Match.Players)
                                    player.PowerHistoryBuilder.AddList(matchPlayer.Entity.SetTag(GAME_TAG.PLAYSTATE, (int)TAG_PLAYSTATE.PLAYING));
                                player.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetPowerStart(PowerHistoryStart.CreateBuilder().SetType(PowerHistoryStart.Types.Type.TRIGGER).SetIndex(-1).SetSource(pClient.Match.GameEntity.ID).SetTarget(0)));
                                player.PowerHistoryBuilder.AddList(pClient.Match.CurrentPlayer.Entity.SetTag(GAME_TAG.CURRENT_PLAYER, 1));
                                player.PowerHistoryBuilder.AddList(pClient.Match.CurrentPlayer.Entity.SetTag(GAME_TAG.FIRST_PLAYER, 1));
                                player.PowerHistoryBuilder.AddList(pClient.Match.GameEntity.SetTag(GAME_TAG.TURN, pClient.Match.Turn));
                            }
                            foreach (MatchPlayer player in match.Players)
                            {
                                int drawCount = 3;
                                if (player != match.FirstPlayer) ++drawCount;
                                for (int index = 0; index < drawCount; ++index)
                                {
                                    MatchCard matchCard = player.DrawCard();
                                    foreach (MatchPlayer matchPlayer in realPlayers)
                                    {
                                        if (matchPlayer == player)
                                            matchPlayer.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetShowEntity(matchCard.Entity.ToPowerHistoryEntity()));
                                        else
                                        {
                                            matchPlayer.PowerHistoryBuilder.AddList(matchCard.Entity.GetTag(GAME_TAG.ZONE));
                                            matchPlayer.PowerHistoryBuilder.AddList(matchCard.Entity.GetTag(GAME_TAG.ZONE_POSITION));
                                        }
                                    }
                                }
                                PowerHistoryData tagNumTurnsLeft = player.Entity.SetTag(GAME_TAG.NUM_TURNS_LEFT, 1);
                                foreach (MatchPlayer matchPlayer in realPlayers) matchPlayer.PowerHistoryBuilder.AddList(tagNumTurnsLeft);
                                if (player != match.FirstPlayer)
                                {
                                    MatchCard matchCard = new MatchCard(CardManager.TheCoinCardAsset);
                                    player.HandCards.Add(matchCard);

                                    matchCard.Entity = match.CreateEntity();

                                    matchCard.Entity.SetTag(GAME_TAG.ZONE, (int)TAG_ZONE.HAND);
                                    matchCard.Entity.SetTag(GAME_TAG.CONTROLLER, player.PlayerID);
                                    matchCard.Entity.SetTag(GAME_TAG.ZONE_POSITION, player.HandCards.Count);
                                    matchCard.Entity.SetTag(GAME_TAG.CREATOR, player.Entity.ID);

                                    foreach (MatchPlayer matchPlayer in realPlayers)
                                    {
                                        if (matchPlayer == player) continue;
                                        matchPlayer.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetFullEntity(matchCard.Entity.ToPowerHistoryEntity()));
                                    }
                                    matchCard.Entity.Name = matchCard.Card.CardID;
                                    matchCard.Entity.SetTag(GAME_TAG.CARD_SET, (int)matchCard.Card.CardSet);
                                    matchCard.Entity.SetTag(GAME_TAG.CARDTYPE, (int)matchCard.Card.CardType);
                                    if (!player.AI)
                                        player.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetFullEntity(matchCard.Entity.ToPowerHistoryEntity()));
                                }
                            }
                            int turnStart = DateTime.UtcNow.ToUnixTimestamp();
                            foreach (MatchPlayer player in realPlayers)
                            {
                                player.PowerHistoryBuilder.AddList(match.GameEntity.SetTag(GAME_TAG.NEXT_STEP, (int)TAG_STEP.BEGIN_MULLIGAN));
                                player.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetPowerEnd(PowerHistoryEnd.CreateBuilder()));
                                player.PowerHistoryBuilder.AddList(match.GameEntity.SetTag(GAME_TAG.STEP, (int)TAG_STEP.BEGIN_MULLIGAN));
                                player.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetPowerStart(PowerHistoryStart.CreateBuilder().SetType(PowerHistoryStart.Types.Type.TRIGGER).SetIndex(-1).SetSource(pClient.Match.GameEntity.ID).SetTarget(0)));
                                foreach (MatchPlayer matchPlayer in match.Players)
                                    player.PowerHistoryBuilder.AddList(matchPlayer.Entity.SetTag(GAME_TAG.MULLIGAN_STATE, (int)TAG_MULLIGAN.INPUT));
                                player.PowerHistoryBuilder.AddList(match.GameEntity.SetTag(GAME_TAG.TURN_START, turnStart));
                                player.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetPowerEnd(PowerHistoryEnd.CreateBuilder()));
                                foreach (MatchPlayer matchPlayer in match.Players.FindAll(p => p.AI))
                                {
                                    player.PowerHistoryBuilder.AddList(matchPlayer.Entity.SetTag(GAME_TAG.MULLIGAN_STATE, (int)TAG_MULLIGAN.DEALING));
                                    player.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetPowerStart(PowerHistoryStart.CreateBuilder().SetType(PowerHistoryStart.Types.Type.TRIGGER).SetIndex(-1).SetSource(matchPlayer.Entity.ID).SetTarget(0)));
                                    player.PowerHistoryBuilder.AddList(matchPlayer.Entity.SetTag(GAME_TAG.MULLIGAN_STATE, (int)TAG_MULLIGAN.WAITING));
                                    player.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetPowerEnd(PowerHistoryEnd.CreateBuilder()));
                                    player.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetPowerStart(PowerHistoryStart.CreateBuilder().SetType(PowerHistoryStart.Types.Type.TRIGGER).SetIndex(-1).SetSource(matchPlayer.Entity.ID).SetTarget(0)));
                                    player.PowerHistoryBuilder.AddList(matchPlayer.Entity.SetTag(GAME_TAG.MULLIGAN_STATE, (int)TAG_MULLIGAN.DONE));
                                    player.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetPowerEnd(PowerHistoryEnd.CreateBuilder()));
                                }
                                player.Client.SendPacket(new Packet((int)PowerHistory.Types.PacketID.ID, player.PowerHistoryBuilder.Build().ToByteArray()));

                                EntityChoice.Builder entityChoiceBuilder = EntityChoice.CreateBuilder();
                                entityChoiceBuilder.SetId(player.PlayerID);
                                entityChoiceBuilder.SetChoiceType((int)CHOICE_TYPE.MULLIGAN);
                                entityChoiceBuilder.SetCancelable(false);
                                entityChoiceBuilder.SetCountMin(0);
                                entityChoiceBuilder.SetCountMax(player.HandCards.Count);
                                player.HandCards.ForEach(c => entityChoiceBuilder.AddEntities(c.Entity.ID));
                                entityChoiceBuilder.SetSource(player.PlayerID);
                                player.Client.SendPacket(new Packet((int)EntityChoice.Types.PacketID.ID, entityChoiceBuilder.Build().ToByteArray()));
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
            foreach (MatchPlayer matchPlayer in pClient.Match.Players)
                powerHistoryCreateGame.AddPlayers(matchPlayer.ToPlayer());

            PowerHistory.Builder powerHistory = PowerHistory.CreateBuilder();
            powerHistory.AddList(PowerHistoryData.CreateBuilder().SetCreateGame(powerHistoryCreateGame));
            foreach (MatchPlayer matchPlayer in pClient.Match.Players)
            {
                powerHistory.AddList(PowerHistoryData.CreateBuilder().SetFullEntity(matchPlayer.HeroEntity.ToPowerHistoryEntity()));
                powerHistory.AddList(PowerHistoryData.CreateBuilder().SetFullEntity(matchPlayer.HeroPowerEntity.ToPowerHistoryEntity()));
                foreach (MatchCard matchCard in matchPlayer.DeckCards)
                    powerHistory.AddList(PowerHistoryData.CreateBuilder().SetFullEntity(matchCard.Entity.ToPowerHistoryEntity()));
            }
            pClient.SendPacket(new Packet((int)PowerHistory.Types.PacketID.ID, powerHistory.Build().ToByteArray()));

            //powerHistory = PowerHistory.CreateBuilder();
            //powerHistory.AddList(PowerHistoryData.CreateBuilder().SetTagChange(PowerHistoryTagChange.CreateBuilder().SetEntity(pClient.Match.GameEntity.Id).SetTag((int)GAME_TAG.STATE).SetValue((int)TAG_STATE.RUNNING)));
            //powerHistory.AddList(PowerHistoryData.CreateBuilder().SetTagChange(PowerHistoryTagChange.CreateBuilder().SetEntity(pClient.Match.Player1Entity.Id).SetTag((int)GAME_TAG.PLAYSTATE).SetValue((int)TAG_PLAYSTATE.PLAYING)));
            //powerHistory.AddList(PowerHistoryData.CreateBuilder().SetTagChange(PowerHistoryTagChange.CreateBuilder().SetEntity(pClient.Match.Player2Entity.Id).SetTag((int)GAME_TAG.PLAYSTATE).SetValue((int)TAG_PLAYSTATE.PLAYING)));
            //powerHistory.AddList(PowerHistoryData.CreateBuilder().SetPowerStart(PowerHistoryStart.CreateBuilder().SetSource(pClient.Match.GameEntity.Id).SetTarget(0).SetType(PowerHistoryStart.Types.Type.TRIGGER).SetIndex(-1)));
            //powerHistory.AddList(PowerHistoryData.CreateBuilder().SetTagChange(PowerHistoryTagChange.CreateBuilder().SetEntity(pClient.Match.Player1Entity.Id).SetTag((int)GAME_TAG.CURRENT_PLAYER).SetValue(1)));
            //powerHistory.AddList(PowerHistoryData.CreateBuilder().SetTagChange(PowerHistoryTagChange.CreateBuilder().SetEntity(pClient.Match.Player1Entity.Id).SetTag((int)GAME_TAG.FIRST_PLAYER).SetValue(1)));
            //powerHistory.AddList(PowerHistoryData.CreateBuilder().SetTagChange(PowerHistoryTagChange.CreateBuilder().SetEntity(pClient.Match.GameEntity.Id).SetTag((int)GAME_TAG.TURN).SetValue(1)));

            //MatchCard drawnCard = null;
            //for (int index = 0; index < 3; ++index)
            //{
            //    drawnCard = pClient.Match.Player1DeckCards[0];
            //    pClient.Match.Player1DeckCards.RemoveAt(0);
            //    pClient.Match.Player1HandCards.Add(drawnCard);

            //    drawnCard.Entity.ClearTags();

            //    drawnCard.Entity.SetName(drawnCard.Card.CardID);
            //    drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.HEALTH).SetValue(drawnCard.Card.Health));
            //    drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ATK).SetValue(drawnCard.Card.Atk));
            //    drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.COST).SetValue(drawnCard.Card.Cost));
            //    drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ZONE).SetValue((int)TAG_ZONE.HAND));
            //    drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARD_SET).SetValue((int)drawnCard.Card.CardSet));
            //    drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.FACTION).SetValue((int)drawnCard.Card.Faction));
            //    drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARDTYPE).SetValue((int)drawnCard.Card.CardType));
            //    drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.RARITY).SetValue((int)drawnCard.Card.Rarity));
            //    //drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName(251).SetValue(1));

            //    powerHistory.AddList(PowerHistoryData.CreateBuilder().SetShowEntity(drawnCard.Entity));

            //    powerHistory.AddList(PowerHistoryData.CreateBuilder().SetTagChange(PowerHistoryTagChange.CreateBuilder().SetEntity(drawnCard.Entity.Entity).SetTag((int)GAME_TAG.ZONE_POSITION).SetValue(pClient.Match.Player1HandCards.Count)));
            //}
            //powerHistory.AddList(PowerHistoryData.CreateBuilder().SetTagChange(PowerHistoryTagChange.CreateBuilder().SetEntity(pClient.Match.Player1Entity.Id).SetTag((int)GAME_TAG.NUM_TURNS_LEFT).SetValue(1)));

            //for (int index = 0; index < 4; ++index)
            //{
            //    drawnCard = pClient.Match.Player2DeckCards[0];
            //    pClient.Match.Player2DeckCards.RemoveAt(0);
            //    pClient.Match.Player2HandCards.Add(drawnCard);

            //    //drawnCard.Entity.ClearTags();

            //    //drawnCard.Entity.SetName(drawnCard.Card.CardID);
            //    //drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.HEALTH).SetValue(drawnCard.Card.Health));
            //    //drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ATK).SetValue(drawnCard.Card.Atk));
            //    //drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.COST).SetValue(drawnCard.Card.Cost));
            //    //drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARD_SET).SetValue((int)drawnCard.Card.CardSet));
            //    //drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARDTYPE).SetValue((int)drawnCard.Card.CardType));
            //    //drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.FACTION).SetValue((int)drawnCard.Card.Faction));
            //    //drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.RARITY).SetValue((int)drawnCard.Card.Rarity));
            //    //drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ZONE).SetValue((int)TAG_ZONE.HAND));
            //    //drawnCard.Entity.AddTags(Tag.CreateBuilder().SetName(251).SetValue(1));

            //    //powerHistory.AddList(PowerHistoryData.CreateBuilder().SetShowEntity(drawnCard.Entity));

            //    powerHistory.AddList(PowerHistoryData.CreateBuilder().SetTagChange(PowerHistoryTagChange.CreateBuilder().SetEntity(drawnCard.Entity.Entity).SetTag((int)GAME_TAG.ZONE).SetValue((int)TAG_ZONE.HAND)));
            //    powerHistory.AddList(PowerHistoryData.CreateBuilder().SetTagChange(PowerHistoryTagChange.CreateBuilder().SetEntity(drawnCard.Entity.Entity).SetTag((int)GAME_TAG.ZONE_POSITION).SetValue(pClient.Match.Player2HandCards.Count)));
            //}
            ////powerHistory.AddList(PowerHistoryData.CreateBuilder().SetFullEntity(pClient.Match.TheCoinCard.Entity));
            //powerHistory.AddList(PowerHistoryData.CreateBuilder().SetTagChange(PowerHistoryTagChange.CreateBuilder().SetEntity(pClient.Match.Player2Entity.Id).SetTag((int)GAME_TAG.NUM_TURNS_LEFT).SetValue(1)));

            //powerHistory.AddList(PowerHistoryData.CreateBuilder().SetTagChange(PowerHistoryTagChange.CreateBuilder().SetEntity(pClient.Match.GameEntity.Id).SetTag((int)GAME_TAG.NEXT_STEP).SetValue((int)TAG_STEP.BEGIN_MULLIGAN)));

            //powerHistory.AddList(PowerHistoryData.CreateBuilder().SetPowerEnd(PowerHistoryEnd.CreateBuilder()));

            //powerHistory.AddList(PowerHistoryData.CreateBuilder().SetTagChange(PowerHistoryTagChange.CreateBuilder().SetEntity(pClient.Match.GameEntity.Id).SetTag((int)GAME_TAG.STEP).SetValue((int)TAG_STEP.BEGIN_MULLIGAN)));

            //powerHistory.AddList(PowerHistoryData.CreateBuilder().SetPowerStart(PowerHistoryStart.CreateBuilder().SetSource(pClient.Match.GameEntity.Id).SetTarget(0).SetType(PowerHistoryStart.Types.Type.TRIGGER).SetIndex(-1)));

            //powerHistory.AddList(PowerHistoryData.CreateBuilder().SetTagChange(PowerHistoryTagChange.CreateBuilder().SetEntity(pClient.Match.Player1Entity.Id).SetTag((int)GAME_TAG.MULLIGAN_STATE).SetValue((int)TAG_MULLIGAN.INPUT)));

            ////powerHistory.AddList(PowerHistoryData.CreateBuilder().SetTagChange(PowerHistoryTagChange.CreateBuilder().SetEntity(pClient.Match.Player2Entity.Id).SetTag((int)GAME_TAG.MULLIGAN_STATE).SetValue((int)TAG_MULLIGAN.INPUT)));

            ////pClient.SendPacket(new Packet((int)PowerHistory.Types.PacketID.ID, powerHistory.Build().ToByteArray()));

            //byte[] buf5 = new byte[]
            //{
            //    0x08, 0x01, 0x10, 0x01, 0x18, 0x00, 0x20, 0x00, 0x28, 0x05, 0x32, 0x05, 0x0F, 0x20, 0x18, 0x06, 0x44, 0x38, 0x01
            //};
            //pClient.SendPacket(new Packet(Opcode.EntityChoice, buf5));

            //byte[] buf6 = new byte[]
            //{
            //    0x0A, 0x09, 0x22, 0x07, 0x08, 0x03, 0x10, 0xB1, 0x02, 0x18, 0x01
            //};
            //pClient.SendPacket(new Packet(Opcode.PowerHistory, buf6));

            //byte[] buf7 = new byte[]
            //{
            //    0x0A, 0x0C, 0x22, 0x0A, 0x08, 0x01, 0x10, 0x08, 0x18, 0xBB, 0xC5, 0x95, 0x94, 0x05, 0x0A, 0x02, 0x3A, 0x00, 0x0A, 0x09, 0x22, 0x07, 0x08, 0x03, 0x10, 0xB1, 0x02, 0x18, 0x02, 0x0A, 0x13, 0x32, 0x11, 0x08, 0x05, 0x10, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x18, 0x03, 0x20, 0x00, 0x0A, 0x09, 0x22, 0x07, 0x08, 0x03, 0x10, 0xB1, 0x02, 0x18, 0x03, 0x0A, 0x02, 0x3A, 0x00, 0x0A, 0x13, 0x32, 0x11, 0x08, 0x05, 0x10, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0x18, 0x03, 0x20, 0x00, 0x0A, 0x09, 0x22, 0x07, 0x08, 0x03, 0x10, 0xB1, 0x02, 0x18, 0x04, 0x0A, 0x02, 0x3A, 0x00
            //};
            //pClient.SendPacket(new Packet(Opcode.PowerHistory, buf7));
        }

        [GamePacketHandler((int)ChooseEntities.Types.PacketID.ID, GameClientProtocolState.GetGameState)]
        public static void ChooseEntitiesHandler(GameClient pClient, Packet pPacket)
        {
            ChooseEntities chooseEntities = ChooseEntities.ParseFrom(pPacket.ReadAll());
            pClient.Log(LogManagerLevel.Debug, "ChooseEntities");

            PreLoad.Builder preLoad = PreLoad.CreateBuilder();
            pClient.SendPacket(new Packet((int)PreLoad.Types.PacketID.ID, preLoad.Build().ToByteArray()));

            pClient.MatchPlayer.PowerHistoryBuilder.ClearList();
            pClient.MatchPlayer.PowerHistoryBuilder.AddList(pClient.MatchPlayer.Entity.SetTag(GAME_TAG.MULLIGAN_STATE, (int)TAG_MULLIGAN.DEALING));
            pClient.MatchPlayer.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetPowerStart(PowerHistoryStart.CreateBuilder().SetType(PowerHistoryStart.Types.Type.TRIGGER).SetIndex(-1).SetSource(pClient.MatchPlayer.Entity.ID).SetTarget(0)));

            Random random = new Random();
            List<MatchCard> returnedCards = new List<MatchCard>();
            foreach (MatchCard matchCard in pClient.MatchPlayer.HandCards)
            {
                if (matchCard.Card == CardManager.TheCoinCardAsset) continue;
                if (chooseEntities.EntitiesList.Contains(matchCard.Entity.ID)) continue;
                returnedCards.Add(matchCard);
            }
            foreach (MatchCard returnedCard in returnedCards)
            {
                int indexOfReturnedCard = pClient.MatchPlayer.HandCards.IndexOf(returnedCard);
                MatchCard newCard = pClient.MatchPlayer.DrawCard();
                pClient.MatchPlayer.HandCards.Remove(returnedCard);
                pClient.MatchPlayer.DeckCards.Insert(random.Next(0, pClient.MatchPlayer.DeckCards.Count - 1), returnedCard);

                newCard.Entity.SetTag(GAME_TAG.ZONE_POSITION, indexOfReturnedCard + 1);

                pClient.MatchPlayer.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetShowEntity(newCard.Entity.ToPowerHistoryEntity()));
                pClient.MatchPlayer.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetHideEntity(PowerHistoryHide.CreateBuilder().SetEntity(returnedCard.Entity.ID).SetZone((int)TAG_ZONE.DECK)));
                // tag change for returnedCard zone position?
                // TODO: Update other clients with hide entity of old card, and zone change of new card
            }
            pClient.MatchPlayer.PowerHistoryBuilder.AddList(pClient.MatchPlayer.Entity.SetTag(GAME_TAG.MULLIGAN_STATE, (int)TAG_MULLIGAN.WAITING));
            pClient.MatchPlayer.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetPowerEnd(PowerHistoryEnd.CreateBuilder()));
            pClient.MatchPlayer.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetPowerStart(PowerHistoryStart.CreateBuilder().SetType(PowerHistoryStart.Types.Type.TRIGGER).SetIndex(-1).SetSource(pClient.MatchPlayer.Entity.ID).SetTarget(0)));
            pClient.MatchPlayer.PowerHistoryBuilder.AddList(pClient.MatchPlayer.Entity.SetTag(GAME_TAG.MULLIGAN_STATE, (int)TAG_MULLIGAN.DONE));

            pClient.MatchPlayer.PowerHistoryBuilder.AddList(pClient.Match.GameEntity.SetTag(GAME_TAG.NEXT_STEP, (int)TAG_STEP.MAIN_READY));
            pClient.MatchPlayer.PowerHistoryBuilder.AddList(PowerHistoryData.CreateBuilder().SetPowerEnd(PowerHistoryEnd.CreateBuilder()));

            pClient.SendPacket(new Packet((int)PowerHistory.Types.PacketID.ID, pClient.MatchPlayer.PowerHistoryBuilder.Build().ToByteArray()));
        }
    }
}
