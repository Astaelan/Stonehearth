using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Data
{
    public class AccountAchievement
    {
        public long AccountID { get; set; }
        public int AchievementID { get; set; }
        public int Progress { get; set; }
        public int AckProgress { get; set; }
        public int CompletionCount { get; set; }
        public bool Active { get; set; }
        public int StartedCount { get; set; }
        public DateTime Given { get; set; }
        public DateTime? Completed { get; set; }
        public bool DoNotAck { get; set; }
    }
}
