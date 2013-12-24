using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Data
{
    public class Power
    {
        public Guid PowerID { get; set; }
        public List<PowerRequirement> Requirements = new List<PowerRequirement>();
    }
}
