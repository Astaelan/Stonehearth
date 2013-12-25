using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Data
{
    public class AccountRecord
    {
        public long AccountID { get; set; }
        public NetCache.PlayerRecord.Type Type { get; set; }
        public int Hero { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
    }
}
