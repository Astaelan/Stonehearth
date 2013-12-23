using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth
{
    public sealed class MatchCard : MatchEntity
    {
        public MatchPlayer Owner = null;
        public CardAsset Card = null;

        public MatchCard(Match pMatch, MatchPlayer pOwner, CardAsset pCard)
            : base(pMatch)
        {
            Owner = pOwner;
            Card = pCard;

            Name = Card.CardID;

            SetTag(GAME_TAG.CARDTYPE, (int)Card.CardType);
            SetTag(GAME_TAG.CARD_SET, (int)Card.CardSet);

            SetTag(GAME_TAG.ZONE, (int)TAG_ZONE.DECK);
            SetTag(GAME_TAG.ZONE_POSITION, 0);
            SetTag(GAME_TAG.CONTROLLER, Owner.PlayerID);
            SetTag(GAME_TAG.CANT_PLAY, 0);

            if (Card.Faction.HasValue) SetTag(GAME_TAG.FACTION, (int)Card.Faction.Value);
            if (Card.Race.HasValue) SetTag(GAME_TAG.CARDRACE, (int)Card.Race.Value);
            if (Card.Class.HasValue) SetTag(GAME_TAG.CLASS, (int)Card.Class.Value);
            if (Card.Rarity.HasValue) SetTag(GAME_TAG.RARITY, (int)Card.Rarity.Value);
            if (Card.Cost.HasValue) SetTag(GAME_TAG.COST, Card.Cost.Value);
            if (Card.Atk.HasValue) SetTag(GAME_TAG.ATK, Card.Atk.Value);
            if (Card.Health.HasValue) SetTag(GAME_TAG.HEALTH, Card.Health.Value);
            if (Card.AttackVisualType.HasValue) SetTag((GAME_TAG)251, Card.AttackVisualType.Value);
            if (Card.Recall.HasValue) SetTag(GAME_TAG.RECALL, Card.Recall.Value);
            if (Card.Durability.HasValue) SetTag(GAME_TAG.DURABILITY, Card.Durability.Value);

            if (Card.TriggerVisual.HasValue) SetTag(GAME_TAG.TRIGGER_VISUAL, Convert.ToInt32(Card.TriggerVisual.Value));
            if (Card.Elite.HasValue) SetTag(GAME_TAG.ELITE, Convert.ToInt32(Card.Elite.Value));
            if (Card.Deathrattle.HasValue) SetTag(GAME_TAG.DEATH_RATTLE, Convert.ToInt32(Card.Deathrattle.Value));
            if (Card.Charge.HasValue) SetTag(GAME_TAG.CHARGE, Convert.ToInt32(Card.Charge.Value));
            if (Card.DivineShield.HasValue) SetTag(GAME_TAG.DIVINE_SHIELD, Convert.ToInt32(Card.DivineShield.Value));
            if (Card.Windfury.HasValue) SetTag(GAME_TAG.WINDFURY, Convert.ToInt32(Card.Windfury.Value));
            if (Card.Taunt.HasValue) SetTag(GAME_TAG.TAUNT, Convert.ToInt32(Card.Taunt.Value));
            if (Card.Aura.HasValue) SetTag(GAME_TAG.AURA, Convert.ToInt32(Card.Aura.Value));
            if (Card.Enrage.HasValue) SetTag(GAME_TAG.ENRAGED, Convert.ToInt32(Card.Enrage.Value));
            if (Card.Stealth.HasValue) SetTag(GAME_TAG.STEALTH, Convert.ToInt32(Card.Stealth.Value));
            if (Card.Battlecry.HasValue) SetTag(GAME_TAG.BATTLECRY, Convert.ToInt32(Card.Battlecry.Value));
            if (Card.Secret.HasValue) SetTag(GAME_TAG.SECRET, Convert.ToInt32(Card.Secret.Value));
            if (Card.Morph.HasValue) SetTag(GAME_TAG.MORPH, Convert.ToInt32(Card.Morph.Value));
            if (Card.AffectedBySpellPower.HasValue) SetTag(GAME_TAG.AFFECTED_BY_SPELL_POWER, Convert.ToInt32(Card.AffectedBySpellPower.Value));
            if (Card.Freeze.HasValue) SetTag(GAME_TAG.FREEZE, Convert.ToInt32(Card.Freeze.Value));
            if (Card.Spellpower.HasValue) SetTag(GAME_TAG.SPELLPOWER, Convert.ToInt32(Card.Spellpower.Value));
            if (Card.Combo.HasValue) SetTag(GAME_TAG.COMBO, Convert.ToInt32(Card.Combo.Value));
            if (Card.Silence.HasValue) SetTag(GAME_TAG.SILENCE, Convert.ToInt32(Card.Silence.Value));
            if (Card.Summoned.HasValue) SetTag(GAME_TAG.SUMMONED, Convert.ToInt32(Card.Summoned.Value));
            if (Card.Poisonous.HasValue) SetTag(GAME_TAG.POISONOUS, Convert.ToInt32(Card.Poisonous.Value));
        }
    }
}
