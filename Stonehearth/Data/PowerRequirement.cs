using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Data
{
    public class PowerRequirement
    {
        public Guid PowerID { get; set; }
        public PlayErrors.ErrorType Type { get; set; }
        public int? Parameter { get; set; }
    }
}
