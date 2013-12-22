using Stonehearth.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth
{
    public sealed class MatchPlayer
    {
        public bool AI = false;
        public long ClientHandle = 0;
        public GameClient Client = null;
        public long AccountID = 0;
        public int PlayerID = 0;
        public CardAsset HeroCard = null;
        public CardAsset HeroPowerCard = null;
        public List<CardAsset> Cards = null;
        public MatchEntity Entity = null;
        public MatchEntity HeroEntity = null;
        public MatchEntity HeroPowerEntity = null;
        public List<MatchCard> DeckCards = new List<MatchCard>();
        public List<MatchCard> HandCards = new List<MatchCard>();

        public PegasusGame.PowerHistory.Builder PowerHistoryBuilder = PegasusGame.PowerHistory.CreateBuilder();

        public MatchPlayer(bool pAI, int pPlayerID, CardAsset pHeroCard, CardAsset pHeroPowerCard, List<CardAsset> pCards, long pClientHandle = 0, long pAccountID = 0)
        {
            AI = pAI;
            ClientHandle = pClientHandle;
            AccountID = pAccountID;
            PlayerID = pPlayerID;
            HeroCard = pHeroCard;
            HeroPowerCard = pHeroPowerCard;
            Cards = ShuffleCards(pCards);
        }

        private static List<CardAsset> ShuffleCards(List<CardAsset> pCards)
        {
            Random random = new Random();
            List<CardAsset> unshuffled = new List<CardAsset>(pCards);
            List<CardAsset> shuffled = new List<CardAsset>(pCards.Count);
            while (unshuffled.Count > 0)
            {
                int index = random.Next(0, unshuffled.Count - 1);
                shuffled.Add(unshuffled[index]);
                unshuffled.RemoveAt(index);
            }
            return shuffled;
        }

        public MatchCard DrawCard()
        {
            if (DeckCards.Count == 0) return null;
            MatchCard matchCard = DeckCards[0];
            HandCards.Add(matchCard);
            DeckCards.RemoveAt(0);

            matchCard.Entity.ClearTags();

            matchCard.Entity.Name = matchCard.Card.CardID;
            matchCard.Entity.SetTag(GAME_TAG.HEALTH, matchCard.Card.Health);
            matchCard.Entity.SetTag(GAME_TAG.ATK, matchCard.Card.Atk);
            matchCard.Entity.SetTag(GAME_TAG.COST, matchCard.Card.Cost);
            matchCard.Entity.SetTag(GAME_TAG.ZONE, (int)TAG_ZONE.HAND);
            matchCard.Entity.SetTag(GAME_TAG.ZONE_POSITION, HandCards.Count);
            matchCard.Entity.SetTag(GAME_TAG.CARD_SET, (int)matchCard.Card.CardSet);
            matchCard.Entity.SetTag(GAME_TAG.FACTION, (int)matchCard.Card.Faction);
            matchCard.Entity.SetTag(GAME_TAG.CARDTYPE, (int)matchCard.Card.CardType);
            matchCard.Entity.SetTag(GAME_TAG.RARITY, (int)matchCard.Card.Rarity);

            return matchCard;
        }

        public PegasusGame.Player ToPlayer()
        {
            PegasusGame.Player.Builder playerBuilder = PegasusGame.Player.CreateBuilder();
            playerBuilder.SetId(PlayerID);
            playerBuilder.SetGameAccountId(PegasusShared.BnetId.CreateBuilder().SetHi(AI ? 144115188075855872UL : 144115193835963207UL).SetLo((ulong)AccountID));
            playerBuilder.SetEntity(Entity.ToEntity());
            return playerBuilder.Build();
        }
    }
}
