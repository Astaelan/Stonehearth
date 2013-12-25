using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Data
{
    public class Card
    {
        public string CardID { get; set; }
        public int AssetID { get; set; }
        public string Name { get; set; }
        public TAG_CARD_SET Set { get; set; }
        public TAG_CARDTYPE Type { get; set; }
        public bool Craftable { get; set; }
        public bool? Collectible { get; set; }
        public Guid? MasterPowerID { get; set; }
        public TAG_FACTION? Faction { get; set; }
        public TAG_RACE? Race { get; set; }
        public TAG_CLASS? Class { get; set; }
        public TAG_RARITY? Rarity { get; set; }
        public int? Cost { get; set; }
        public int? Atk { get; set; }
        public int? Health { get; set; }
        public int? AttackVisualType { get; set; }
        public string TextInHand { get; set; }
        public string TextInPlay { get; set; }
        public int? DevState { get; set; }
        public TAG_ENCHANTMENT_VISUAL? EnchantmentBirthVisual { get; set; }
        public TAG_ENCHANTMENT_VISUAL? EnchantmentIdleVisual { get; set; }
        public string FlavorText { get; set; }
        public string ArtistName { get; set; }
        public string TargetingArrowText { get; set; }
        public string HowToGetThisGoldCard { get; set; }
        public string HowToGetThisCard { get; set; }
        public int? StandardBuyValue { get; set; }
        public int? StandardSellValue { get; set; }
        public int? FoilBuyValue { get; set; }
        public int? FoilSellValue { get; set; }
        public int? Recall { get; set; }
        public int? Durability { get; set; }
        public bool? TriggerVisual { get; set; }
        public bool? Elite { get; set; }
        public bool? Deathrattle { get; set; }
        public bool? Charge { get; set; }
        public bool? DivineShield { get; set; }
        public bool? Windfury { get; set; }
        public bool? Taunt { get; set; }
        public bool? Aura { get; set; }
        public bool? Enrage { get; set; }
        public bool? OneTurnEffect { get; set; }
        public bool? Stealth { get; set; }
        public bool? Battlecry { get; set; }
        public bool? Secret { get; set; }
        public bool? Morph { get; set; }
        public bool? AffectedBySpellPower { get; set; }
        public bool? Freeze { get; set; }
        public bool? Spellpower { get; set; }
        public bool? Combo { get; set; }
        public bool? Silence { get; set; }
        public bool? Summoned { get; set; }
        public bool? ImmuneToSpellpower { get; set; }
        public bool? AdjacentBuff { get; set; }
        public bool? GrantCharge { get; set; }
        public bool? Poisonous { get; set; }
        public bool? HealTarget { get; set; }

        public List<CardPower> Powers = new List<CardPower>();
    }
}
