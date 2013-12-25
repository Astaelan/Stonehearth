using Dapper;
using PegasusUtil;
using Stonehearth.Properties;
using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Stonehearth
{
    public static class CardManager
    {
        public static List<Data.Achievement> Achievements = null;
        public static Dictionary<int, Data.Achievement> AchievementsByAchievementID = new Dictionary<int, Data.Achievement>();

        public static List<Data.Card> Cards = null;
        public static Dictionary<string, Data.Card> CardsByCardID = new Dictionary<string, Data.Card>();
        public static Dictionary<int, Data.Card> CardsByAssetID = new Dictionary<int, Data.Card>();
        public static Dictionary<TAG_CLASS, List<Data.Card>> CoreFreeCardsByClassID = new Dictionary<TAG_CLASS, List<Data.Card>>();
        public static Dictionary<TAG_CLASS, Data.Card> CoreHeroCardsByClassID = new Dictionary<TAG_CLASS, Data.Card>();
        public static Dictionary<TAG_CLASS, Data.Card> CoreHeroPowerCardsByClassID = new Dictionary<TAG_CLASS, Data.Card>();
        public static Dictionary<TAG_RARITY, List<Data.Card>> ExpertCollectibleCardsByRarity = new Dictionary<TAG_RARITY, List<Data.Card>>();
        public static Data.Card TheCoinCard = null;

        public static CardValues CardValues = null;

        public static Dictionary<Guid, Data.CardPower> CardPowersByCardPowerID = new Dictionary<Guid, Data.CardPower>();

        public static void Initialize()
        {
            using (SqlConnection db = DB.Open(Settings.Default.Database))
            {
                Achievements = db.Query<Data.Achievement>("SELECT * FROM [Achievement]", null).ToList();
                Achievements.ForEach(a => AchievementsByAchievementID.Add(a.AchievementID, a));
                Cards = db.Query<Data.Card>("SELECT * FROM [Card]", null).ToList();
                foreach (Data.Card card in Cards)
                {
                    CardsByCardID.Add(card.CardID, card);
                    CardsByAssetID.Add(card.AssetID, card);

                    if (card.Set == TAG_CARD_SET.CORE)
                    {
                        if (card.Rarity == TAG_RARITY.FREE)
                        {
                            List<Data.Card> coreFreeCards = null;
                            if (!CoreFreeCardsByClassID.TryGetValue(card.Class.GetValueOrDefault(), out coreFreeCards))
                            {
                                coreFreeCards = new List<Data.Card>();
                                CoreFreeCardsByClassID.Add(card.Class.GetValueOrDefault(), coreFreeCards);
                            }
                            if (!card.CardID.StartsWith("GAME_")) coreFreeCards.Add(card);
                        }
                        if (card.Type == TAG_CARDTYPE.HERO)
                            CoreHeroCardsByClassID.Add(card.Class.GetValueOrDefault(), card);
                        if (card.Type == TAG_CARDTYPE.HERO_POWER)
                            CoreHeroPowerCardsByClassID.Add(card.Class.GetValueOrDefault(), card);
                    }
                    if (card.Set == TAG_CARD_SET.EXPERT1)
                    {
                        if (card.Collectible.GetValueOrDefault())
                        {
                            List<Data.Card> expertCollectibleCards = null;
                            if (!ExpertCollectibleCardsByRarity.TryGetValue(card.Rarity.GetValueOrDefault(), out expertCollectibleCards))
                            {
                                expertCollectibleCards = new List<Data.Card>();
                                ExpertCollectibleCardsByRarity.Add(card.Rarity.GetValueOrDefault(), expertCollectibleCards);
                            }
                            expertCollectibleCards.Add(card);
                        }
                    }
                    if (card.CardID == "GAME_005") TheCoinCard = card;
                }
                foreach (Data.CardPower cardPower in db.Query<Data.CardPower>("SELECT * FROM [CardPower]", null))
                {
                    CardPowersByCardPowerID.Add(cardPower.CardPowerID, cardPower);
                    CardsByCardID[cardPower.CardID].Powers.Add(cardPower);
                }
                foreach (Data.CardPowerRequirement cardPowerRequirement in db.Query<Data.CardPowerRequirement>("SELECT * FROM [CardPowerRequirement]", null))
                {
                    CardPowersByCardPowerID[cardPowerRequirement.CardPowerID].Requirements.Add(cardPowerRequirement);
                }
            }

            CardValues.Builder cardValues = CardValues.CreateBuilder();
            foreach (Data.Card card in Cards.Where(c => c.Craftable))
            {
                cardValues.AddCards(CardValue.CreateBuilder().SetBuy(card.StandardBuyValue.Value).SetSell(card.StandardSellValue.Value).SetNerfed(false).SetCard(PegasusShared.CardDef.CreateBuilder().SetAsset(card.AssetID).SetPremium((int)CardFlair.PremiumType.STANDARD)));
                cardValues.AddCards(CardValue.CreateBuilder().SetBuy(card.FoilBuyValue.Value).SetSell(card.FoilSellValue.Value).SetNerfed(false).SetCard(PegasusShared.CardDef.CreateBuilder().SetAsset(card.AssetID).SetPremium((int)CardFlair.PremiumType.FOIL)));
            }
            CardValues = cardValues.Build();

            LogManager.WriteLine(LogManagerLevel.Info, "[CardManager] Loaded {0} Cards", Cards.Count);
        }

    }
}
