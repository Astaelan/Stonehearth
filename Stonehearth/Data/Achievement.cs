using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Data
{
    public class Achievement
    {
        public int AchievementID { get; set; }
        public string Group { get; set; }
        public int MaxProgress { get; set; }
        public int RaceRequirement { get; set; }
        public int CardSetRequirement { get; set; }
        public string RewardType { get; set; }
        public int Parameter1 { get; set; }
        public int Parameter2 { get; set; }
        public string UnlockableFeature { get; set; }
        public int ParentID { get; set; }
        public string Trigger { get; set; }
    }
}
