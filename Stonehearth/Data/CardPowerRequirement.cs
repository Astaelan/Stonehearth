using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Data
{
    public class CardPowerRequirement
    {
        public Guid CardPowerID { get; set; }
        public PlayErrors.ErrorType Type { get; set; }
        public int? Parameter { get; set; }
    }
}
