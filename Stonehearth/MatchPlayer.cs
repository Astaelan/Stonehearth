using PegasusGame;
using Stonehearth.Game;
using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth
{
    public sealed class MatchPlayer : MatchEntity
    {
        public bool AI = false;
        public long ClientHandle = 0;
        public GameClient Client = null;
        public long AccountID = 0;
        public int PlayerID = 0;
        public Data.Card HeroCardData = null;
        public Data.Card HeroPowerCardData = null;
        public List<Data.Card> Cards = null;

        public MatchCard HeroCard = null;
        public MatchCard HeroPowerCard = null;
        public List<MatchCard> DeckCards = new List<MatchCard>();
        public List<MatchCard> HandCards = new List<MatchCard>();

        private PegasusGame.PowerHistory.Builder mPowerHistoryBuilder = PegasusGame.PowerHistory.CreateBuilder();

        public MatchPlayer(Match pMatch, bool pAI, int pPlayerID, Data.Card pHeroCardData, Data.Card pHeroPowerCardData, List<Data.Card> pCards, long pClientHandle = 0, long pAccountID = 0)
            : base(pMatch)
        {
            Match = pMatch;
            AI = pAI;
            ClientHandle = pClientHandle;
            AccountID = pAccountID;
            PlayerID = pPlayerID;
            HeroCardData = pHeroCardData;
            HeroPowerCardData = pHeroPowerCardData;
            Cards = ShuffleCards(pCards);

            HeroCard = new MatchCard(pMatch, this, HeroCardData);
            HeroPowerCard = new MatchCard(pMatch, this, HeroPowerCardData);

            Cards.ForEach(c => DeckCards.Add(new MatchCard(pMatch, this, c)));

            SetTag(GAME_TAG.ZONE, (int)TAG_ZONE.PLAY);
            SetTag(GAME_TAG.CARDTYPE, (int)TAG_CARDTYPE.PLAYER);
            SetTag(GAME_TAG.PLAYER_ID, PlayerID);
            SetTag(GAME_TAG.TEAM_ID, PlayerID);
            SetTag(GAME_TAG.CONTROLLER, PlayerID);
            SetTag(GAME_TAG.MAXHANDSIZE, 10);
            SetTag(GAME_TAG.STARTHANDSIZE, 4);
            SetTag(GAME_TAG.MAXRESOURCES, 10);
            SetTag(GAME_TAG.HERO_ENTITY, HeroCard.EntityID);
            HeroCard.SetTag(GAME_TAG.ZONE, (int)TAG_ZONE.PLAY);
            HeroPowerCard.SetTag(GAME_TAG.ZONE, (int)TAG_ZONE.PLAY);
            HeroPowerCard.SetTag(GAME_TAG.CREATOR, HeroCard.EntityID);
        }

        private static List<Data.Card> ShuffleCards(List<Data.Card> pCards)
        {
            Random random = new Random();
            List<Data.Card> unshuffled = new List<Data.Card>(pCards);
            List<Data.Card> shuffled = new List<Data.Card>(pCards.Count);
            while (unshuffled.Count > 0)
            {
                int index = random.Next(0, unshuffled.Count - 1);
                shuffled.Add(unshuffled[index]);
                unshuffled.RemoveAt(index);
            }
            return shuffled;
        }

        public void SendPacket(Packet pPacket)
        {
            if (Client == null) return;
            Client.SendPacket(pPacket);
        }

        public MatchCard DrawCard()
        {
            if (DeckCards.Count == 0) return null;
            MatchCard matchCard = DeckCards[0];
            HandCards.Add(matchCard);
            DeckCards.RemoveAt(0);

            matchCard.SetZoneAndPositionTags(TAG_ZONE.HAND, HandCards.Count);

            return matchCard;
        }

        public void FlushPowerHistory()
        {
            if (AI) return;
            lock (mPowerHistoryBuilder)
            {
                Client.SendPacket(new Packet((int)PowerHistory.Types.PacketID.ID, mPowerHistoryBuilder.Build().ToByteArray()));
                mPowerHistoryBuilder.ClearList();
            }
        }

        public void SendPowerHistoryData(PowerHistoryData pPowerHistoryData)
        {
            if (AI) return;
            lock (mPowerHistoryBuilder)
            {
                mPowerHistoryBuilder.AddList(pPowerHistoryData);
            }
        }

        public void SendPowerHistoryFullEntity(PowerHistoryEntity pPowerHistoryEntity)
        {
            SendPowerHistoryData(PowerHistoryData.CreateBuilder().SetFullEntity(pPowerHistoryEntity).Build());
        }

        public void SendPowerHistoryShowEntity(PowerHistoryEntity pPowerHistoryEntity)
        {
            SendPowerHistoryData(PowerHistoryData.CreateBuilder().SetShowEntity(pPowerHistoryEntity).Build());
        }

        public void SendPowerHistoryTagChange(PowerHistoryTagChange pPowerHistoryTagChange)
        {
            SendPowerHistoryData(PowerHistoryData.CreateBuilder().SetTagChange(pPowerHistoryTagChange).Build());
        }

        public PegasusGame.Player ToPlayer()
        {
            PegasusGame.Player.Builder playerBuilder = PegasusGame.Player.CreateBuilder();
            playerBuilder.SetId(PlayerID);
            playerBuilder.SetGameAccountId(PegasusShared.BnetId.CreateBuilder().SetHi(AI ? 144115188075855872UL : 144115193835963207UL).SetLo((ulong)AccountID));
            playerBuilder.SetEntity(ToEntity());
            return playerBuilder.Build();
        }
    }
}
