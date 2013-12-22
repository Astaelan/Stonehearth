using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth
{
    public sealed class MatchCard
    {
        public CardAsset Card = null;
        public MatchEntity Entity = null;

        public MatchCard(CardAsset pCard)
        {
            Card = pCard;
        }
    }
}
