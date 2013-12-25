using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Data
{
    public class AccountHero
    {
        public long AccountID { get; set; }
        public TAG_CLASS ClassID { get; set; }
        public int Level { get; set; }
        public long CurrentXP { get; set; }
    }
}
