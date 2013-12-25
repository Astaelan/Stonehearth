using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Data
{
    public class AccountDeckCard
    {
        public long AccountID { get; set; }
        public long AccountDeckID { get; set; }
        public string CardID { get; set; }
        public int Quantity { get; set; }
        public int Handle { get; set; }
        public int Previous { get; set; }
    }
}
