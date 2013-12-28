using PegasusGame;
using PegasusShared;
using Stonehearth.Game;
using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Stonehearth
{
    public sealed class Match
    {
        public int GameHandle = 0;
        public string Password = "Password";

        public MatchPlayer FirstPlayer = null;
        public MatchPlayer CurrentPlayer = null;
        private int mLastPlayerID = 0;
        public int PlayersReady = 0;
        public List<MatchPlayer> Players = new List<MatchPlayer>();

        private int mLastEntityID = 0;
        private int mLastOptionsID = 0;

        public MatchEntity GameEntity = null;
        public int Turn = 1;


        public Match()
        {
            GameEntity = new MatchEntity(this);
            GameEntity.SetTag(GAME_TAG.ZONE, (int)TAG_ZONE.PLAY);
            GameEntity.SetTag(GAME_TAG.CARDTYPE, (int)TAG_CARDTYPE.GAME);
            GameEntity.SetTag(GAME_TAG.STATE, (int)TAG_STATE.LOADING);
        }

        public int GetNextEntityID() { return Interlocked.Increment(ref mLastEntityID); }

        //public MatchEntity CreateEntity()
        //{
        //    MatchEntity matchEntity = new MatchEntity(this);
        //    Entities.Add(matchEntity);
        //    return matchEntity;
        //}

        public MatchPlayer CreatePlayer(bool pAI, Data.Card pHeroCard, Data.Card pHeroPowerCard, List<Data.Card> pCards, long pClientID = 0, long pAccountID = 0)
        {
            MatchPlayer matchPlayer = new MatchPlayer(this, pAI, Interlocked.Increment(ref mLastPlayerID), pHeroCard, pHeroPowerCard, pCards, pClientID, pAccountID);
            Players.Add(matchPlayer);

            //matchPlayer.HeroCard = new MatchCard(this, matchPlayer, pHeroCard);
            //matchPlayer.HeroPowerCard = new MatchCard(this, matchPlayer, pHeroPowerCard);

            //matchPlayer.SetTag(GAME_TAG.HERO_ENTITY, matchPlayer.HeroCard.EntityID);
            //matchPlayer.HeroCard.SetTag(GAME_TAG.ZONE, (int)TAG_ZONE.PLAY);
            //matchPlayer.HeroPowerCard.SetTag(GAME_TAG.ZONE, (int)TAG_ZONE.PLAY);
            //matchPlayer.HeroPowerCard.SetTag(GAME_TAG.CREATOR, matchPlayer.HeroCard.EntityID);

            //foreach (CardAsset cardAsset in matchPlayer.Cards)
            //{
            //    MatchCard matchCard = new MatchCard(this, matchPlayer, cardAsset);
            //    Entities.Add(matchCard);
            //    matchPlayer.DeckCards.Add(matchCard);
            //}

            return matchPlayer;
        }

        public void FlushPowerHistory(params MatchPlayer[] pExceptMatchPlayers)
        {
            Players.Except(pExceptMatchPlayers).ForEach(p => p.FlushPowerHistory());
        }

        public void SendPowerHistoryData(PowerHistoryData pPowerHistoryData, params MatchPlayer[] pExceptMatchPlayers)
        {
            Players.Except(pExceptMatchPlayers).ForEach(p => p.SendPowerHistoryData(pPowerHistoryData));
        }

        public void SendPowerHistoryFullEntity(PowerHistoryEntity pPowerHistoryEntity, params MatchPlayer[] pExceptMatchPlayers)
        {
            SendPowerHistoryData(PowerHistoryData.CreateBuilder().SetFullEntity(pPowerHistoryEntity).Build(), pExceptMatchPlayers);
        }

        public void SendPowerHistoryShowEntity(PowerHistoryEntity pPowerHistoryEntity, params MatchPlayer[] pExceptMatchPlayers)
        {
            SendPowerHistoryData(PowerHistoryData.CreateBuilder().SetShowEntity(pPowerHistoryEntity).Build(), pExceptMatchPlayers);
        }

        public void SendPowerHistoryHideEntity(int pEntityID, TAG_ZONE pZone, params MatchPlayer[] pExceptMatchPlayers)
        {
            SendPowerHistoryData(PowerHistoryData.CreateBuilder().SetHideEntity(PowerHistoryHide.CreateBuilder().SetEntity(pEntityID).SetZone((int)pZone)).Build(), pExceptMatchPlayers);
        }

        public void SendPowerHistoryTagChange(PowerHistoryTagChange pPowerHistoryTagChange, params MatchPlayer[] pExceptMatchPlayers)
        {
            SendPowerHistoryData(PowerHistoryData.CreateBuilder().SetTagChange(pPowerHistoryTagChange).Build(), pExceptMatchPlayers);
        }

        public void SendPowerHistoryZoneChange(MatchEntity pMatchEntity, params MatchPlayer[] pExceptMatchPlayers)
        {
            SendPowerHistoryTagChange(pMatchEntity.GetTag(GAME_TAG.ZONE), pExceptMatchPlayers);
            SendPowerHistoryTagChange(pMatchEntity.GetTag(GAME_TAG.ZONE_POSITION), pExceptMatchPlayers);
        }

        public void SendPowerHistoryZoneChange(MatchEntity pMatchEntity, TAG_ZONE pZone, int pZonePosition, params MatchPlayer[] pExceptMatchPlayers)
        {
            SendPowerHistoryTagChange(pMatchEntity.SetTag(GAME_TAG.ZONE, (int)pZone), pExceptMatchPlayers);
            SendPowerHistoryTagChange(pMatchEntity.SetTag(GAME_TAG.ZONE_POSITION, pZonePosition), pExceptMatchPlayers);
        }

        public void SendPowerHistoryStart(PowerHistoryStart.Types.Type pType, int pIndex, int pSource, int pTarget, params MatchPlayer[] pExceptMatchPlayers)
        {
            SendPowerHistoryData(PowerHistoryData.CreateBuilder().SetPowerStart(PowerHistoryStart.CreateBuilder().SetType(pType).SetSource(pSource).SetTarget(pTarget).SetIndex(pIndex)).Build(), pExceptMatchPlayers);
        }

        public void SendPowerHistoryEnd(params MatchPlayer[] pExceptMatchPlayers)
        {
            SendPowerHistoryData(PowerHistoryData.CreateBuilder().SetPowerEnd(PowerHistoryEnd.CreateBuilder()).Build(), pExceptMatchPlayers);
        }


        public void StartTurn()
        {
            // NOTE: MAIN_READY seems to be game data updates for each turn
            SendPowerHistoryTagChange(GameEntity.SetTag(GAME_TAG.STEP, (int)TAG_STEP.MAIN_READY));
            //SendPowerHistoryStart(PowerHistoryStart.Types.Type.TRIGGER, -1, FirstPlayer.EntityID, 0);

            SendPowerHistoryTagChange(CurrentPlayer.HeroCard.SetTag(GAME_TAG.NUM_TURNS_IN_PLAY, Turn));
            SendPowerHistoryTagChange(CurrentPlayer.HeroPowerCard.SetTag(GAME_TAG.NUM_TURNS_IN_PLAY, Turn));

            CurrentPlayer.Resources = Math.Min(10, Turn);
            SendPowerHistoryTagChange(CurrentPlayer.SetTag(GAME_TAG.RESOURCES, CurrentPlayer.Resources));

            CurrentPlayer.ResourcesUsed = 0;
            CurrentPlayer.HeroPowerExhausted = false;

            //SendPowerHistoryTagChange(GameEntity.SetTag(GAME_TAG.NEXT_STEP, (int)TAG_STEP.MAIN_START_TRIGGERS));
            //SendPowerHistoryEnd();


            // NOTE: START_TRIGGERS seems to be where we do updates from powers that trigger at the start of turns
            SendPowerHistoryTagChange(GameEntity.SetTag(GAME_TAG.STEP, (int)TAG_STEP.MAIN_START_TRIGGERS));

            //SendPowerHistoryStart(PowerHistoryStart.Types.Type.TRIGGER, -1, CurrentPlayer.EntityID, 0);
            //SendPowerHistoryTagChange(pClient.Match.GameEntity.SetTag(GAME_TAG.NEXT_STEP, (int)TAG_STEP.MAIN_START));
            //SendPowerHistoryEnd();

            // NOTE: MAIN_START seems to be where we do our main card draw for the turn
            SendPowerHistoryTagChange(GameEntity.SetTag(GAME_TAG.STEP, (int)TAG_STEP.MAIN_START));

            //SendPowerHistoryStart(PowerHistoryStart.Types.Type.TRIGGER, -1, CurrentPlayer.EntityID, 0);
            MatchCard matchCard = CurrentPlayer.DrawCard();
            CurrentPlayer.SendPowerHistoryShowEntity(matchCard.ToShownPowerHistoryEntity());
            SendPowerHistoryZoneChange(matchCard, CurrentPlayer);
            //SendPowerHistoryTagChange(pClient.Match.GameEntity.SetTag(GAME_TAG.NEXT_STEP, (int)TAG_STEP.MAIN_ACTION));
            //SendPowerHistoryEnd();

            // NOTE: MAIN_ACTION seems to be where we wait for an option to be choosen by a player (or processing for AI)
            SendPowerHistoryTagChange(GameEntity.SetTag(GAME_TAG.STEP, (int)TAG_STEP.MAIN_ACTION));

            //SendPowerHistoryStart(PowerHistoryStart.Types.Type.TRIGGER, -1, CurrentPlayer.EntityID, 0);
            //SendPowerHistoryTagChange(GameEntity.SetTag(GAME_TAG.NEXT_STEP, (int)TAG_STEP.MAIN_END));
            //SendPowerHistoryEnd();

            // NOTE: Now we figure out what options are available for the current player and send them out
            // unless it is an AI player, in which case we need to pick an available option, initial AI may
            // be able to pick an option at random.
            AllOptions allOptions = GetCurrentPlayerOptions();
            if (!CurrentPlayer.AI) CurrentPlayer.SendPowerHistoryTagChange(CurrentPlayer.SetTag(GAME_TAG.NUM_OPTIONS, allOptions.OptionsCount));
            else
            {
                // TODO: Picking an option and sending power history for it
            }

            FlushPowerHistory();

            CurrentPlayer.SendPacket(new Packet((int)AllOptions.Types.PacketID.ID, allOptions.ToByteArray()));
        }

        public AllOptions GetCurrentPlayerOptions()
        {
            AllOptions.Builder allOptionsBuilder = AllOptions.CreateBuilder();
            allOptionsBuilder.SetId(Interlocked.Increment(ref mLastOptionsID));
            allOptionsBuilder.AddOptions(PegasusGame.Option.CreateBuilder().SetType(PegasusGame.Option.Types.Type.END_TURN));
            if (!CurrentPlayer.HeroPowerExhausted && CurrentPlayer.HeroPowerCard.Card.Cost.GetValueOrDefault() < CurrentPlayer.ResourcesRemaining)
            {
                // TODO: Check requirements
            }
            List<MatchCard> allTargets = new List<MatchCard>();
            foreach (MatchPlayer matchPlayer in Players)
            {
                matchPlayer.PlayCards.ForEach(c => allTargets.Add(c));
                allTargets.Add(matchPlayer.HeroCard);
            }
            foreach (MatchCard matchCard in CurrentPlayer.HandCards)
            {
                if (!matchCard.Card.Cost.HasValue) continue;
                if (matchCard.Card.Cost.Value > CurrentPlayer.ResourcesRemaining) continue;
                if (matchCard.Card.Powers.Count == 0)
                {
                    allOptionsBuilder.AddOptions(PegasusGame.Option.CreateBuilder().SetType(PegasusGame.Option.Types.Type.POWER).SetMainOption(PegasusGame.SubOption.CreateBuilder().SetId(matchCard.EntityID)));
                    continue;
                }
                if (matchCard.Card.Powers.TrueForAll(p => p.Requirements.Count == 0))
                {
                    allOptionsBuilder.AddOptions(PegasusGame.Option.CreateBuilder().SetType(PegasusGame.Option.Types.Type.POWER).SetMainOption(PegasusGame.SubOption.CreateBuilder().SetId(matchCard.EntityID)));
                    continue;
                }
                bool allowed = true;
                bool targetRequired = false;
                bool targetIfAvailable = false;
                List<MatchCard> targets = new List<MatchCard>(allTargets);
                foreach (Data.CardPower cardPower in matchCard.Card.Powers)
                {
                    foreach (Data.CardPowerRequirement cardPowerRequirement in cardPower.Requirements)
                    {
                        switch (cardPowerRequirement.Type)
                        {
                            case PlayErrors.ErrorType.REQ_MINION_TARGET: targets.RemoveAll(e => e.CardType != TAG_CARDTYPE.MINION); break;
                            case PlayErrors.ErrorType.REQ_FRIENDLY_TARGET: targets.RemoveAll(e => e.Owner != CurrentPlayer); break;
                            case PlayErrors.ErrorType.REQ_ENEMY_TARGET: targets.RemoveAll(e => e.Owner == CurrentPlayer); break;
                            case PlayErrors.ErrorType.REQ_TARGET_TO_PLAY: targetRequired = true; break;
                            case PlayErrors.ErrorType.REQ_NUM_MINION_SLOTS: allowed = (CurrentPlayer.PlayCards.Count(c => c.CardType == TAG_CARDTYPE.MINION) + cardPowerRequirement.Parameter.Value) <= 7; break;
                            case PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE: targetIfAvailable = true; break;
                            default: allowed = false; break;
                        }
                        if (!allowed) break;
                    }
                    if (!allowed) break;
                }
                if (allowed && targetRequired && targets.Count == 0) allowed = false;
                if (!allowed) continue;
                PegasusGame.SubOption.Builder option = PegasusGame.SubOption.CreateBuilder().SetId(matchCard.EntityID);
                if (targetRequired || targetIfAvailable) targets.ForEach(c => option.AddTargets(c.EntityID));
                allOptionsBuilder.AddOptions(PegasusGame.Option.CreateBuilder().SetType(PegasusGame.Option.Types.Type.POWER).SetMainOption(option));
            }
            return allOptionsBuilder.Build();
        }

        //public bool Player1GoesFirst = false;
        //public long Player1AccountID = 0;
        //public long Player2AccountID = 0;

        //public CardAsset Player1HeroCard = null;
        //public CardAsset Player2HeroCard = null;
        //public CardAsset Player1HeroPowerCard = null;
        //public CardAsset Player2HeroPowerCard = null;
        //public List<CardAsset> Player1CardAssets = new List<CardAsset>();
        //public List<CardAsset> Player2CardAssets = new List<CardAsset>();

        //public PegasusGame.Entity.Builder GameEntityBuilder = null;
        //public PegasusGame.Player.Builder Player1Builder = null;
        //public PegasusGame.Player.Builder Player2Builder = null;
        //public PegasusGame.Entity.Builder Player1EntityBuilder = null;
        //public PegasusGame.Entity.Builder Player2EntityBuilder = null;
        //public PegasusGame.PowerHistoryEntity.Builder Player1HeroEntityBuilder = null;
        //public PegasusGame.PowerHistoryEntity.Builder Player2HeroEntityBuilder = null;
        //public PegasusGame.PowerHistoryEntity.Builder Player1HeroPowerEntityBuilder = null;
        //public PegasusGame.PowerHistoryEntity.Builder Player2HeroPowerEntityBuilder = null;

        //public List<MatchCard> Player1DeckCards = new List<MatchCard>();
        //public List<MatchCard> Player2DeckCards = new List<MatchCard>();

        //public List<MatchCard> Player1HandCards = new List<MatchCard>();
        //public List<MatchCard> Player2HandCards = new List<MatchCard>();

        //public MatchCard TheCoinCard = null;

        //public PegasusGame.Entity.Builder CreateBasicEntity()
        //{
        //    PegasusGame.Entity.Builder entity = PegasusGame.Entity.CreateBuilder();
        //    entity.SetId(Interlocked.Increment(ref mLastEntityID));
        //    entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ENTITY_ID).SetValue(entity.Id));
        //    return entity;
        //}

        //public PegasusGame.PowerHistoryEntity.Builder CreateFullEntity(string pName)
        //{
        //    PegasusGame.PowerHistoryEntity.Builder entity = PegasusGame.PowerHistoryEntity.CreateBuilder();
        //    entity.SetEntity(Interlocked.Increment(ref mLastEntityID));
        //    entity.SetName(pName);
        //    entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ENTITY_ID).SetValue(entity.Entity));
        //    return entity;
        //}

        //private void Shuffle(List<CardAsset> pCards)
        //{
        //    Random random = new Random();
        //    List<CardAsset> shuffled = new List<CardAsset>(pCards.Count);
        //    while (pCards.Count > 0)
        //    {
        //        int index = random.Next(0, pCards.Count - 1);
        //        shuffled.Add(pCards[index]);
        //        pCards.RemoveAt(index);
        //    }
        //    pCards.AddRange(shuffled);
        //}

        //public void Initialize()
        //{
        //    Shuffle(Player1CardAssets);
        //    Shuffle(Player2CardAssets);

        //    GameEntityBuilder = CreateBasicEntity();
        //    Player1Builder = PegasusGame.Player.CreateBuilder().SetId(1).SetGameAccountId(BnetId.CreateBuilder().SetHi(144115188075855872).SetLo((ulong)Player1AccountID)); // 144115193835963207
        //    Player2Builder = PegasusGame.Player.CreateBuilder().SetId(2).SetGameAccountId(BnetId.CreateBuilder().SetHi(144115188075855872).SetLo((ulong)Player2AccountID)); 
        //    Player1EntityBuilder = CreateBasicEntity();
        //    Player2EntityBuilder = CreateBasicEntity();
        //    Player1HeroEntityBuilder = CreateFullEntity(Player1HeroCard.CardID);
        //    Player2HeroEntityBuilder = CreateFullEntity(Player2HeroCard.CardID);
        //    Player1HeroPowerEntityBuilder = CreateFullEntity(Player1HeroPowerCard.CardID);
        //    Player2HeroPowerEntityBuilder = CreateFullEntity(Player2HeroPowerCard.CardID);
        //    TheCoinCard = new MatchCard(CardManager.TheCoinCardAsset, CreateFullEntity(""));

        //    GameEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ZONE).SetValue((int)TAG_ZONE.PLAY));
        //    GameEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARDTYPE).SetValue((int)TAG_CARDTYPE.GAME));
        //    GameEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.STATE).SetValue((int)TAG_STATE.LOADING));
        //    //GameEntity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.TURN).SetValue(1));
            
        //    Player1EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.HERO_ENTITY).SetValue(Player1HeroEntityBuilder.Entity));
        //    Player1EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.MAXHANDSIZE).SetValue(10));
        //    Player1EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.STARTHANDSIZE).SetValue(4));
        //    Player1EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.PLAYER_ID).SetValue(Player1Builder.Id));
        //    Player1EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.TEAM_ID).SetValue(Player1Builder.Id));
        //    Player1EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ZONE).SetValue((int)TAG_ZONE.PLAY));
        //    Player1EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CONTROLLER).SetValue(Player1Builder.Id));
        //    Player1EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.MAXRESOURCES).SetValue(10));
        //    Player1EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARDTYPE).SetValue((int)TAG_CARDTYPE.PLAYER));

        //    //Player1Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.FIRST_PLAYER).SetValue(1));
        //    //Player1Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CURRENT_PLAYER).SetValue(1));

        //    Player2EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.HERO_ENTITY).SetValue(Player2HeroEntityBuilder.Entity));
        //    Player2EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.MAXHANDSIZE).SetValue(10));
        //    Player2EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.STARTHANDSIZE).SetValue(4));
        //    Player2EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.PLAYER_ID).SetValue(Player2Builder.Id));
        //    Player2EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.TEAM_ID).SetValue(Player2Builder.Id));
        //    Player2EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ZONE).SetValue((int)TAG_ZONE.PLAY));
        //    Player2EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CONTROLLER).SetValue(Player2Builder.Id));
        //    Player2EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.MAXRESOURCES).SetValue(10));
        //    Player2EntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARDTYPE).SetValue((int)TAG_CARDTYPE.PLAYER));

        //    Player1HeroEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARDTYPE).SetValue((int)TAG_CARDTYPE.HERO));
        //    Player1HeroEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ZONE).SetValue((int)TAG_ZONE.PLAY));
        //    Player1HeroEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CONTROLLER).SetValue(Player1Builder.Id));
        //    Player1HeroEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.HEALTH).SetValue(Player1HeroCard.Health));
        //    Player1HeroEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARD_SET).SetValue((int)Player1HeroCard.CardSet));
        //    Player1HeroEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.FACTION).SetValue((int)Player1HeroCard.Faction));
        //    Player1HeroEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.RARITY).SetValue((int)Player1HeroCard.Rarity));
        //    //Player1HeroEntity.AddTags(Tag.CreateBuilder().SetName(251).SetValue(1));

        //    Player2HeroEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARDTYPE).SetValue((int)TAG_CARDTYPE.HERO));
        //    Player2HeroEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ZONE).SetValue((int)TAG_ZONE.PLAY));
        //    Player2HeroEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CONTROLLER).SetValue(Player2Builder.Id));
        //    Player2HeroEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.HEALTH).SetValue(Player2HeroCard.Health));
        //    Player2HeroEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARD_SET).SetValue((int)Player2HeroCard.CardSet));
        //    Player2HeroEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.FACTION).SetValue((int)Player2HeroCard.Faction));
        //    Player2HeroEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.RARITY).SetValue((int)Player2HeroCard.Rarity));
        //    //Player2HeroEntity.AddTags(Tag.CreateBuilder().SetName(251).SetValue(1));

        //    Player1HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CREATOR).SetValue(Player1HeroEntityBuilder.Entity));
        //    Player1HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARDTYPE).SetValue((int)TAG_CARDTYPE.HERO_POWER));
        //    Player1HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ZONE).SetValue((int)TAG_ZONE.PLAY));
        //    Player1HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CONTROLLER).SetValue(Player1Builder.Id));
        //    Player1HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.COST).SetValue(Player1HeroPowerCard.Cost));
        //    Player1HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARD_SET).SetValue((int)Player1HeroPowerCard.CardSet));
        //    Player1HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.FACTION).SetValue((int)Player1HeroPowerCard.Faction));
        //    Player1HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.RARITY).SetValue((int)Player1HeroPowerCard.Rarity));
        //    //Player1HeroEntity.AddTags(Tag.CreateBuilder().SetName(251).SetValue(1));

        //    Player2HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CREATOR).SetValue(Player2HeroEntityBuilder.Entity));
        //    Player2HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARDTYPE).SetValue((int)TAG_CARDTYPE.HERO_POWER));
        //    Player2HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ZONE).SetValue((int)TAG_ZONE.PLAY));
        //    Player2HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CONTROLLER).SetValue(Player2Builder.Id));
        //    Player2HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.COST).SetValue(Player2HeroPowerCard.Cost));
        //    Player2HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARD_SET).SetValue((int)Player2HeroPowerCard.CardSet));
        //    Player2HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.FACTION).SetValue((int)Player2HeroPowerCard.Faction));
        //    Player2HeroPowerEntityBuilder.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.RARITY).SetValue((int)Player2HeroPowerCard.Rarity));
        //    //Player1HeroEntity.AddTags(Tag.CreateBuilder().SetName(251).SetValue(1));

        //    Player1Builder.SetEntity(Player1EntityBuilder);
        //    Player2Builder.SetEntity(Player2EntityBuilder);

        //    foreach (CardAsset cardAsset in Player1CardAssets)
        //    {
        //        PowerHistoryEntity.Builder cardEntity = CreateFullEntity("");
        //        cardEntity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ZONE).SetValue((int)TAG_ZONE.DECK));
        //        cardEntity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CONTROLLER).SetValue(Player1Builder.Id));
        //        cardEntity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ZONE_POSITION).SetValue(0));
        //        cardEntity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CANT_PLAY).SetValue(0));
        //        Player1DeckCards.Add(new MatchCard(cardAsset, cardEntity));
        //    }

        //    foreach (CardAsset cardAsset in Player2CardAssets)
        //    {
        //        PowerHistoryEntity.Builder cardEntity = CreateFullEntity("");
        //        cardEntity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ZONE).SetValue((int)TAG_ZONE.DECK));
        //        cardEntity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CONTROLLER).SetValue(Player2Builder.Id));
        //        cardEntity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ZONE_POSITION).SetValue(0));
        //        cardEntity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CANT_PLAY).SetValue(0));
        //        Player2DeckCards.Add(new MatchCard(cardAsset, cardEntity));
        //    }

        //    TheCoinCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ZONE).SetValue((int)TAG_ZONE.HAND));
        //    TheCoinCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.ZONE_POSITION).SetValue(5));
        //    TheCoinCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CONTROLLER).SetValue(Player1GoesFirst ? Player2Builder.Id : Player1Builder.Id));
        //    TheCoinCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CREATOR).SetValue(Player1GoesFirst ? Player2EntityBuilder.Id : Player1Builder.Id));
        //    //TheCoinCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARD_SET).SetValue((int)CardManager.TheCoinCardAsset.CardSet));
        //    //TheCoinCard.Entity.AddTags(Tag.CreateBuilder().SetName((int)GAME_TAG.CARDTYPE).SetValue((int)CardManager.TheCoinCardAsset.CardType));
        //}
    }
}
