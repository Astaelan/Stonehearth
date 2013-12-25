using Dapper;
using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Stonehearth.Data
{
    public class Account
    {
        public long AccountID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid? SessionID { get; set; }
        public string SessionHost { get; set; }
        public DateTime? SessionExpiration { get; set; }
        public DateTime LastLogin { get; set; }
        public int ExpertBoosters { get; set; }
        public int BestForge { get; set; }
        public DateTime LastForge { get; set; }
        public MissionProgress Progress { get; set; }
        public int DeckLimit { get; set; }
        public long ArcaneDustBalance { get; set; }
        public long GoldBalance { get; set; }

        public List<AccountAchievement> Achievements = new List<AccountAchievement>();
        public List<AccountCard> Cards = new List<AccountCard>();
        public List<AccountDeck> Decks = new List<AccountDeck>();
        public List<AccountHero> Heros = new List<AccountHero>();
        public List<AccountRecord> Records = new List<AccountRecord>();

        public static Account Load(SqlConnection pDB, long pAccountID)
        {
            Account account = pDB.Query<Account>("SELECT * FROM [Account] WHERE [AccountID]=@AccountID", new { AccountID = pAccountID }).FirstOrDefault();
            account.Achievements.AddRange(pDB.Query<AccountAchievement>("SELECT * FROM [AccountAchievement] WHERE [AccountID]=@AccountID", new { AccountID = pAccountID }));
            account.Cards.AddRange(pDB.Query<AccountCard>("SELECT * FROM [AccountCard] WHERE [AccountID]=@AccountID", new { AccountID = pAccountID }));
            account.Decks.AddRange(pDB.Query<AccountDeck>("SELECT * FROM [AccountDeck] WHERE [AccountID]=@AccountID", new { AccountID = pAccountID }));
            pDB.Query<AccountDeckCard>("SELECT * FROM [AccountDeckCard] WHERE [AccountID]=@AccountID", new { AccountID = pAccountID }).ForEach(c => account.Decks.Find(d => d.AccountDeckID == c.AccountDeckID).Cards.Add(c));
            account.Heros.AddRange(pDB.Query<AccountHero>("SELECT * FROM [AccountHero] WHERE [AccountID]=@AccountID", new { AccountID = pAccountID }));
            account.Records.AddRange(pDB.Query<AccountRecord>("SELECT * FROM [AccountRecord] WHERE [AccountID]=@AccountID", new { AccountID = pAccountID }));
            return account;
        }
    }
}
