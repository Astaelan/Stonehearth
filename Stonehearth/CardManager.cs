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
        public static List<Data.Achieve> Achieves = null;
        public static Dictionary<int, Data.Achieve> AchievesByAchieveID = new Dictionary<int, Data.Achieve>();

        public static List<Data.Power> Powers = null;
        public static Dictionary<Guid, Data.Power> PowersByPowerID = new Dictionary<Guid, Data.Power>();

        public static List<Data.Card> Cards = null;
        public static Dictionary<string, Data.Card> CardsByCardID = new Dictionary<string, Data.Card>();
        public static Dictionary<int, Data.Card> CardsByAssetID = new Dictionary<int, Data.Card>();
        public static Dictionary<TAG_CLASS, List<Data.Card>> CoreFreeCardsByClassID = new Dictionary<TAG_CLASS, List<Data.Card>>();
        public static Dictionary<TAG_CLASS, Data.Card> CoreHeroCardsByClassID = new Dictionary<TAG_CLASS, Data.Card>();
        public static Dictionary<TAG_CLASS, Data.Card> CoreHeroPowerCardsByClassID = new Dictionary<TAG_CLASS, Data.Card>();
        public static Dictionary<TAG_RARITY, List<Data.Card>> ExpertCollectibleCardsByRarity = new Dictionary<TAG_RARITY, List<Data.Card>>();
        public static Data.Card TheCoinCard = null;

        public static CardValues CardValues = null;

        public static void Initialize()
        {
            using (SqlConnection db = DB.Open(Settings.Default.Database))
            {
                Achieves = db.Query<Data.Achieve>("SELECT * FROM [Achieve]", null).ToList();
                Achieves.ForEach(a => AchievesByAchieveID.Add(a.AchieveID, a));
                Powers = db.Query<Data.Power>("SELECT * FROM [Power]", null).ToList();
                Powers.ForEach(p => PowersByPowerID.Add(p.PowerID, p));
                db.Query<Data.PowerRequirement>("SELECT * FROM [PowerRequirement]", null).ForEach(r => PowersByPowerID[r.PowerID].Requirements.Add(r));
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
                db.Query<Data.CardPower>("SELECT * FROM [CardPower]", null).ForEach(c => CardsByCardID[c.CardID].Powers.Add(PowersByPowerID[c.PowerID]));
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
