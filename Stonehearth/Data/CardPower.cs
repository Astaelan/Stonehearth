using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Data
{
    public class CardPower
    {
        public Guid CardPowerID { get; set; }
        public string CardID { get; set; }

        public List<CardPowerRequirement> Requirements = new List<CardPowerRequirement>();
    }
}
