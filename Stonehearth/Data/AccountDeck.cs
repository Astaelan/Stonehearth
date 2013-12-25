using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Data
{
    public class AccountDeck
    {
        public long AccountDeckID { get; set; }
        public long AccountID { get; set; }
        public string Name { get; set; }
        public int Hero { get; set; }
        public PegasusUtil.DeckInfo.Types.DeckType DeckType { get; set; }
        public CardFlair.PremiumType HeroPremium { get; set; }
        public int Box { get; set; }
        public NetCache.DeckFlags Validity { get; set; }

        public List<AccountDeckCard> Cards = new List<AccountDeckCard>();
    }
}
