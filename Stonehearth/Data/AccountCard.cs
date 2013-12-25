using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Data
{
    public class AccountCard
    {
        public long AccountID { get; set; }
        public string CardID { get; set; }
        public CardFlair.PremiumType Premium { get; set; }
        public int Count { get; set; }
        public int CountSeen { get; set; }
        public DateTime LatestInserted { get; set; }
    }
}
