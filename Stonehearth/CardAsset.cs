﻿using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Stonehearth
{
    public class CardAsset
    {
        private static HashSet<string> sUnhandledTags = new HashSet<string>();

        public string CardID = null;
        public int AssetID = 0;
        public string CardName = null;
        public TAG_CARD_SET CardSet = TAG_CARD_SET.TEST_TEMPORARY;
        public TAG_CARDTYPE CardType = TAG_CARDTYPE.INVALID;
        public TAG_FACTION Faction = TAG_FACTION.NEUTRAL;
        public TAG_RACE Race = TAG_RACE.INVALID;
        public TAG_CLASS Class = TAG_CLASS.INVALID;
        public TAG_RARITY Rarity = TAG_RARITY.COMMON;
        public int Cost = 0;
        public int Atk = 0;
        public int Health = 0;
        public int AttackVisualType = 0;
        public string CardTextInHand = null;
        public string CardTextInPlay = null;
        public int DevState = 0;
        public TAG_ENCHANTMENT_VISUAL EnchantmentBirthVisual = TAG_ENCHANTMENT_VISUAL.INVALID;
        public TAG_ENCHANTMENT_VISUAL EnchantmentIdleVisual = TAG_ENCHANTMENT_VISUAL.INVALID;
        public string FlavorText = null;
        public string ArtistName = null;
        public string TargetingArrowText = null;
        public string HowToGetThisGoldCard = null;
        public string HowToGetThisCard = null;
        public int Recall = 0;
        public int Durability = 0;
        public bool TriggerVisual = false;
        public bool Collectible = false;
        public bool Elite = false;
        public bool Deathrattle = false;
        public bool Charge = false;
        public bool DivineShield = false;
        public bool Windfury = false;
        public bool Taunt = false;
        public bool Aura = false;
        public bool Enrage = false;
        public bool OneTurnEffect = false;
        public bool Stealth = false;
        public bool Battlecry = false;
        public bool Secret = false;
        public bool Morph = false;
        public bool AffectedBySpellPower = false;
        public bool Freeze = false;
        public bool Spellpower = false;
        public bool Combo = false;
        public bool Silence = false;
        public bool Summoned = false;
        public bool ImmuneToSpellpower = false;
        public bool AdjacentBuff = false;
        public bool GrantCharge = false;
        public bool Poisonous = false;
        public bool HealTarget = false;

        public bool AllowCrafting = false;
        public Dictionary<CardFlair.PremiumType, int> ArcaneDustBuyValues = new Dictionary<CardFlair.PremiumType, int>();
        public Dictionary<CardFlair.PremiumType, int> ArcaneDustSellValues = new Dictionary<CardFlair.PremiumType, int>();

        public CardAssetPower MasterPower = null;
        public List<CardAssetPower> Powers = new List<CardAssetPower>();

        public static CardAsset Load(string pCardID, int pAssetID, XmlElement pRootElement)
        {
            CardAsset cardAsset = new CardAsset() { CardID = pCardID, AssetID = pAssetID };
            foreach (XmlNode tagNode in pRootElement.GetElementsByTagName("Tag"))
            {
                switch (tagNode.Attributes["name"].Value)
                {
                    case "CardName": cardAsset.CardName = tagNode.ChildNodes.Item(0).InnerText; break;
                    case "CardSet": cardAsset.CardSet = (TAG_CARD_SET)int.Parse(tagNode.Attributes["value"].Value); break;
                    case "CardType": cardAsset.CardType = (TAG_CARDTYPE)int.Parse(tagNode.Attributes["value"].Value); break;
                    case "Faction": cardAsset.Faction = (TAG_FACTION)int.Parse(tagNode.Attributes["value"].Value); break;
                    case "Race": cardAsset.Race = (TAG_RACE)int.Parse(tagNode.Attributes["value"].Value); break;
                    case "Class": cardAsset.Class = (TAG_CLASS)int.Parse(tagNode.Attributes["value"].Value); break;
                    case "Rarity": cardAsset.Rarity = (TAG_RARITY)int.Parse(tagNode.Attributes["value"].Value); break;
                    case "Cost": cardAsset.Cost = int.Parse(tagNode.Attributes["value"].Value); break;
                    case "Atk": cardAsset.Atk = int.Parse(tagNode.Attributes["value"].Value); break;
                    case "Health": cardAsset.Health = int.Parse(tagNode.Attributes["value"].Value); break;
                    case "AttackVisualType": cardAsset.AttackVisualType = int.Parse(tagNode.Attributes["value"].Value); break;
                    case "CardTextInHand": cardAsset.CardTextInHand = tagNode.ChildNodes.Item(0).InnerText; break;
                    case "CardTextInPlay": cardAsset.CardTextInPlay = tagNode.ChildNodes.Item(0).InnerText; break;
                    case "DevState": cardAsset.DevState = int.Parse(tagNode.Attributes["value"].Value); break;
                    case "EnchantmentBirthVisual": cardAsset.EnchantmentBirthVisual = (TAG_ENCHANTMENT_VISUAL)int.Parse(tagNode.Attributes["value"].Value); break;
                    case "EnchantmentIdleVisual": cardAsset.EnchantmentIdleVisual = (TAG_ENCHANTMENT_VISUAL)int.Parse(tagNode.Attributes["value"].Value); break;
                    case "FlavorText": cardAsset.FlavorText = tagNode.ChildNodes.Item(0).InnerText; break;
                    case "ArtistName": cardAsset.ArtistName = tagNode.ChildNodes.Item(0).InnerText; break;
                    case "TargetingArrowText": cardAsset.TargetingArrowText = tagNode.ChildNodes.Item(0).InnerText; break;
                    case "HowToGetThisGoldCard": cardAsset.HowToGetThisGoldCard = tagNode.ChildNodes.Item(0).InnerText; break;
                    case "HowToGetThisCard": cardAsset.HowToGetThisCard = tagNode.ChildNodes.Item(0).InnerText; break;
                    case "Recall": cardAsset.Recall = int.Parse(tagNode.Attributes["value"].Value); break;
                    case "Durability": cardAsset.Durability = int.Parse(tagNode.Attributes["value"].Value); break;
                    case "TriggerVisual": cardAsset.TriggerVisual = tagNode.Attributes["value"].Value == "1"; break;
                    case "Collectible": cardAsset.Collectible = tagNode.Attributes["value"].Value == "1"; break;
                    case "Elite": cardAsset.Elite = tagNode.Attributes["value"].Value == "1"; break;
                    case "Deathrattle": cardAsset.Deathrattle = tagNode.Attributes["value"].Value == "1"; break;
                    case "Charge": cardAsset.Charge = tagNode.Attributes["value"].Value == "1"; break;
                    case "Divine Shield": cardAsset.DivineShield = tagNode.Attributes["value"].Value == "1"; break;
                    case "Windfury": cardAsset.Windfury = tagNode.Attributes["value"].Value == "1"; break;
                    case "Taunt": cardAsset.Taunt = tagNode.Attributes["value"].Value == "1"; break;
                    case "Aura": cardAsset.Aura = tagNode.Attributes["value"].Value == "1"; break;
                    case "Enrage": cardAsset.Enrage = tagNode.Attributes["value"].Value == "1"; break;
                    case "OneTurnEffect": cardAsset.OneTurnEffect = tagNode.Attributes["value"].Value == "1"; break;
                    case "Stealth": cardAsset.Stealth = tagNode.Attributes["value"].Value == "1"; break;
                    case "Battlecry": cardAsset.Battlecry = tagNode.Attributes["value"].Value == "1"; break;
                    case "Secret": cardAsset.Secret = tagNode.Attributes["value"].Value == "1"; break;
                    case "Morph": cardAsset.Morph = tagNode.Attributes["value"].Value == "1"; break;
                    case "AffectedBySpellPower": cardAsset.AffectedBySpellPower = tagNode.Attributes["value"].Value == "1"; break;
                    case "Freeze": cardAsset.Freeze = tagNode.Attributes["value"].Value == "1"; break;
                    case "Spellpower": cardAsset.Spellpower = tagNode.Attributes["value"].Value == "1"; break;
                    case "Combo": cardAsset.Combo = tagNode.Attributes["value"].Value == "1"; break;
                    case "Silence": cardAsset.Silence = tagNode.Attributes["value"].Value == "1"; break;
                    case "Summoned": cardAsset.Summoned = tagNode.Attributes["value"].Value == "1"; break;
                    case "ImmuneToSpellpower": cardAsset.ImmuneToSpellpower = tagNode.Attributes["value"].Value == "1"; break;
                    case "AdjacentBuff": cardAsset.AdjacentBuff = tagNode.Attributes["value"].Value == "1"; break;
                    case "GrantCharge": cardAsset.GrantCharge = tagNode.Attributes["value"].Value == "1"; break;
                    case "Poisonous": cardAsset.Poisonous = tagNode.Attributes["value"].Value == "1"; break;
                    case "HealTarget": cardAsset.HealTarget = tagNode.Attributes["value"].Value == "1"; break;

                    default:
                        if (!sUnhandledTags.Contains(tagNode.Attributes["name"].Value))
                        {
                            sUnhandledTags.Add(tagNode.Attributes["name"].Value);
                            LogManager.WriteLine(LogManagerLevel.Debug, "Unhandled Card Tag: {0} on {1}", tagNode.Attributes["name"].Value, pCardID);
                        }
                        break;
                }
            }
            foreach (XmlNode powerNode in pRootElement.GetElementsByTagName("Power"))
            {
                CardAssetPower cardAssetPower = CardAssetPower.Load((XmlElement)powerNode);
                cardAsset.Powers.Add(cardAssetPower);
            }
            foreach (XmlNode masterPowerNode in pRootElement.GetElementsByTagName("MasterPower"))
            {
                Guid masterPowerDefinition = new Guid(masterPowerNode.InnerText);
                cardAsset.MasterPower = cardAsset.Powers.Find(p => p.Definition == masterPowerDefinition);
            }

            if (cardAsset.CardSet == TAG_CARD_SET.EXPERT1 && cardAsset.Collectible)
            {
                switch (cardAsset.Rarity)
                {
                    case TAG_RARITY.COMMON:
                        cardAsset.AllowCrafting = true;
                        cardAsset.ArcaneDustBuyValues.Add(CardFlair.PremiumType.STANDARD, 40);
                        cardAsset.ArcaneDustSellValues.Add(CardFlair.PremiumType.STANDARD, 5);
                        cardAsset.ArcaneDustBuyValues.Add(CardFlair.PremiumType.FOIL, 400);
                        cardAsset.ArcaneDustSellValues.Add(CardFlair.PremiumType.FOIL, 50);
                        break;
                    case TAG_RARITY.RARE:
                        cardAsset.AllowCrafting = true;
                        cardAsset.ArcaneDustBuyValues.Add(CardFlair.PremiumType.STANDARD, 100);
                        cardAsset.ArcaneDustSellValues.Add(CardFlair.PremiumType.STANDARD, 20);
                        cardAsset.ArcaneDustBuyValues.Add(CardFlair.PremiumType.FOIL, 800);
                        cardAsset.ArcaneDustSellValues.Add(CardFlair.PremiumType.FOIL, 100);
                        break;
                    case TAG_RARITY.EPIC:
                        cardAsset.AllowCrafting = true;
                        cardAsset.ArcaneDustBuyValues.Add(CardFlair.PremiumType.STANDARD, 400);
                        cardAsset.ArcaneDustSellValues.Add(CardFlair.PremiumType.STANDARD, 100);
                        cardAsset.ArcaneDustBuyValues.Add(CardFlair.PremiumType.FOIL, 1600);
                        cardAsset.ArcaneDustSellValues.Add(CardFlair.PremiumType.FOIL, 400);
                        break;
                    case TAG_RARITY.LEGENDARY:
                        cardAsset.AllowCrafting = true;
                        cardAsset.ArcaneDustBuyValues.Add(CardFlair.PremiumType.STANDARD, 1600);
                        cardAsset.ArcaneDustSellValues.Add(CardFlair.PremiumType.STANDARD, 400);
                        cardAsset.ArcaneDustBuyValues.Add(CardFlair.PremiumType.FOIL, 3200);
                        cardAsset.ArcaneDustSellValues.Add(CardFlair.PremiumType.FOIL, 1600);
                        break;
                    default: break;
                }
            }
            
            return cardAsset;
        }

        public override string ToString() { return CardName + " (" + CardSet + ":" + CardType + ")"; }
    }
}