using PegasusShared;
using PegasusUtil;
using Stonehearth.Properties;
using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Stonehearth.Lobby
{
    public static class LobbyUtilPacketHandlers
    {
        [LobbyUtilPacketHandler((int)GetAccountInfo.Types.PacketID.ID)]
        public static void GetAccountInfoHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            GetAccountInfo getAccountInfo = GetAccountInfo.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "GetAccountInfo: {0}", getAccountInfo.Request.ToSafeString());

            using (SqlConnection db = DB.Open(Settings.Default.Database))
            {
                switch (getAccountInfo.Request)
                {
                    case GetAccountInfo.Types.Request.BOOSTERS:
                        {
                            BoosterList.Builder boosterList = BoosterList.CreateBuilder();
                            using (SqlDataReader dr = db.ExecuteReader(null, "SELECT [ExpertBoosters] FROM [Account] WHERE [AccountID]=@0", pClient.AccountID))
                            {
                                if (!dr.Read())
                                {
                                    pClient.Disconnect();
                                    return;
                                }
                                if ((int)dr["ExpertBoosters"] > 0)
                                {
                                    BoosterInfo.Builder boosterInfo = BoosterInfo.CreateBuilder();
                                    boosterInfo.SetCount((int)dr["ExpertBoosters"]);
                                    boosterInfo.SetType((int)BoosterType.EXPERT);
                                    boosterList.AddList(boosterInfo);
                                }

                            }
                            pClient.SendUtilPacket((int)BoosterList.Types.PacketID.ID, pContext, boosterList.Build().ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.CAMPAIGN_INFO:
                        {
                            ProfileProgress.Builder profileProgress = ProfileProgress.CreateBuilder();
                            using (SqlDataReader dr = db.ExecuteReader(null, "SELECT [BestForge],[LastForge],[Progress] FROM [Account] WHERE [AccountID]=@0", pClient.AccountID))
                            {
                                if (!dr.Read())
                                {
                                    pClient.Disconnect();
                                    return;
                                }
                                profileProgress.SetProgress((long)dr["Progress"]);
                                profileProgress.SetBestForge((int)dr["BestForge"]);
                                if (profileProgress.BestForge > 0) profileProgress.SetLastForge(Date.CreateBuilder().FromDateTime((DateTime)dr["LastForge"]));
                            }
                            pClient.SendUtilPacket((int)ProfileProgress.Types.PacketID.ID, pContext, profileProgress.Build().ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.FEATURES:
                        {
                            GuardianVars.Builder guardianVars = GuardianVars.CreateBuilder();
                            guardianVars.SetShowUserUI(1);
                            pClient.SendUtilPacket((int)GuardianVars.Types.PacketID.ID, pContext, guardianVars.Build().ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.MEDAL_INFO:
                        {
                            MedalInfo.Builder medalInfo = MedalInfo.CreateBuilder();
                            medalInfo.SetCanLose(false);
                            medalInfo.SetCurrMedal((int)Medal.Type.MEDAL_NOVICE);
                            medalInfo.SetCurrXp(0);
                            medalInfo.SetLegendRank(1);
                            medalInfo.SetLevelEnd(3);
                            medalInfo.SetLevelStart(1);
                            medalInfo.SetMidLevel(0);
                            medalInfo.SetPrevMedal((int)Medal.Type.MEDAL_NOVICE);
                            medalInfo.SetPrevXp(0);
                            medalInfo.SetSeasonWins(0);
                            medalInfo.SetStarLevel(1);
                            medalInfo.SetStars(0);
                            medalInfo.SetStreak(0);
                            pClient.SendUtilPacket((int)MedalInfo.Types.PacketID.ID, pContext, medalInfo.Build().ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.NOTICES:
                        {
                            ProfileNotices.Builder profileNotices = ProfileNotices.CreateBuilder();
                            pClient.SendUtilPacket((int)ProfileNotices.Types.PacketID.ID, pContext, profileNotices.Build().ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.DECK_LIST:
                        {
                            DeckList.Builder deckList = DeckList.CreateBuilder();
                            using (SqlDataReader dr = db.ExecuteReader(null, "SELECT [DeckID],[Name],[Hero],[DeckType],[HeroPremium],[Box],[Validity] FROM [Deck] WHERE [AccountID]=@0", pClient.AccountID))
                            {
                                while (dr.Read())
                                {
                                    DeckInfo.Builder deckInfo = DeckInfo.CreateBuilder();

                                    deckInfo.SetId((long)dr["DeckID"]);
                                    deckInfo.SetName((string)dr["Name"]);
                                    deckInfo.SetHero(CardManager.CoreHeroCardsByClassID[(TAG_CLASS)(int)dr["Hero"]].AssetID);
                                    deckInfo.SetDeckType((DeckInfo.Types.DeckType)(int)dr["DeckType"]);
                                    deckInfo.SetHeroPremium((int)dr["HeroPremium"]);
                                    deckInfo.SetBox((int)dr["Box"]);
                                    deckInfo.SetValidity((long)dr["Validity"]);
                                    deckList.AddDecks(deckInfo);
                                }
                            }
                            pClient.SendUtilPacket((int)DeckList.Types.PacketID.ID, pContext, deckList.Build().ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.COLLECTION:
                        {
                            Collection.Builder collection = Collection.CreateBuilder();
                            using (SqlDataReader dr = db.ExecuteReader(null, "SELECT [CardID],[Premium],[Count],[CountSeen],[LatestInserted] FROM [Collection] WHERE [AccountID]=@0", pClient.AccountID))
                            {
                                while (dr.Read())
                                {
                                    CardStack.Builder cardStack = CardStack.CreateBuilder();
                                    Data.Card card = null;
                                    if (!CardManager.CardsByCardID.TryGetValue((string)dr["CardID"], out card)) continue;

                                    PegasusShared.CardDef.Builder cardDefBuilder = PegasusShared.CardDef.CreateBuilder();
                                    cardDefBuilder.SetAsset(card.AssetID);
                                    cardDefBuilder.SetPremium((int)dr["Premium"]);

                                    cardStack.SetCardDef(cardDefBuilder);
                                    cardStack.SetCount((int)dr["Count"]);
                                    cardStack.SetNumSeen((int)dr["CountSeen"]);
                                    cardStack.SetLatestInsertDate(Date.CreateBuilder().FromDateTime((DateTime)dr["LatestInserted"]));

                                    collection.AddStacks(cardStack);
                                }
                            }
                            pClient.SendUtilPacket((int)Collection.Types.PacketID.ID, pContext, collection.Build().ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.DECK_LIMIT:
                        {
                            ProfileDeckLimit.Builder profileDeckLimit = ProfileDeckLimit.CreateBuilder();
                            using (SqlDataReader dr = db.ExecuteReader(null, "SELECT [DeckLimit] FROM [Account] WHERE [AccountID]=@0", pClient.AccountID))
                            {
                                if (!dr.Read())
                                {
                                    pClient.Disconnect();
                                    return;
                                }
                                profileDeckLimit.SetDeckLimit((int)dr["DeckLimit"]);
                            }
                            pClient.SendUtilPacket((int)ProfileDeckLimit.Types.PacketID.ID, pContext, profileDeckLimit.Build().ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.CARD_VALUES:
                        {
                            pClient.SendUtilPacket((int)CardValues.Types.PacketID.ID, pContext, CardManager.CardValues.ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.ARCANE_DUST_BALANCE:
                        {
                            ArcaneDustBalance.Builder arcaneDustBalance = ArcaneDustBalance.CreateBuilder();
                            using (SqlDataReader dr = db.ExecuteReader(null, "SELECT [ArcaneDustBalance] FROM [Account] WHERE [AccountID]=@0", pClient.AccountID))
                            {
                                if (!dr.Read())
                                {
                                    pClient.Disconnect();
                                    return;
                                }
                                arcaneDustBalance.SetBalance((long)dr["ArcaneDustBalance"]);
                            }
                            pClient.SendUtilPacket((int)ArcaneDustBalance.Types.PacketID.ID, pContext, arcaneDustBalance.Build().ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.REWARD_PROGRESS:
                        {
                            RewardProgress.Builder rewardProgress = RewardProgress.CreateBuilder();
                            rewardProgress
                                .SetGoldPerReward(5)
                                .SetMaxGoldPerDay(100)
                                .SetMaxHeroLevel(30)
                                .SetNextQuestCancel(Date.CreateBuilder().FromDateTime(DateTime.UtcNow))
                                .SetPackId(1)
                                .SetWinsPerGold(5)
                                .SetXpSoloLimit(10)
                                .SetSeasonEnd(Date.CreateBuilder()
                                    .SetYear(2015)
                                    .SetMonth(1)
                                    .SetDay(1)
                                    .SetHours(0)
                                    .SetMin(0)
                                    .SetSec(0));
                            pClient.SendUtilPacket((int)RewardProgress.Types.PacketID.ID, pContext, rewardProgress.Build().ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.PLAYER_RECORD:
                        {
                            PlayerRecords.Builder playerRecords = PlayerRecords.CreateBuilder();
                            Dictionary<NetCache.PlayerRecord.Type, PlayerRecord.Builder> playerRecordTotals = new Dictionary<NetCache.PlayerRecord.Type, PlayerRecord.Builder>();
                            using (SqlDataReader dr = db.ExecuteReader(null, "SELECT [Type],[Hero],[Wins],[Losses],[Ties] FROM [PlayerRecord] WHERE [AccountID]=@0 ORDER BY [Type]", pClient.AccountID))
                            {
                                while (dr.Read())
                                {
                                    NetCache.PlayerRecord.Type type = (NetCache.PlayerRecord.Type)(int)dr["Type"];
                                    int wins = (int)dr["Wins"];
                                    int losses = (int)dr["Losses"];
                                    int ties = (int)dr["Ties"];
                                    playerRecords.AddRecords(PlayerRecord.CreateBuilder().SetType((int)type).SetWins(wins).SetLosses(losses).SetTies(ties).SetData(CardManager.CoreHeroCardsByClassID[(TAG_CLASS)(int)dr["Hero"]].AssetID));
                                    if (wins > 0 || losses > 0 || ties > 0)
                                    {
                                        PlayerRecord.Builder playerRecord = null;
                                        if (!playerRecordTotals.ContainsKey(type))
                                        {
                                            playerRecord = PlayerRecord.CreateBuilder();
                                            playerRecord.SetType((int)type);
                                            playerRecord.SetWins(0);
                                            playerRecord.SetLosses(0);
                                            playerRecord.SetTies(0);
                                            playerRecordTotals[type] = playerRecord;
                                        }
                                        if (wins > 0) playerRecord.SetWins(playerRecord.Wins + wins);
                                        if (losses > 0) playerRecord.SetLosses(playerRecord.Losses + losses);
                                        if (ties > 0) playerRecord.SetTies(playerRecord.Ties + ties);
                                    }
                                }
                            }
                            foreach (PlayerRecord.Builder playerRecord in playerRecordTotals.Values) playerRecords.AddRecords(playerRecord);
                            pClient.SendUtilPacket((int)PlayerRecords.Types.PacketID.ID, pContext, playerRecords.Build().ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.DISCONNECTED:
                        {
                            Disconnected.Builder disconnected = Disconnected.CreateBuilder();
                            pClient.SendUtilPacket((int)Disconnected.Types.PacketID.ID, pContext, disconnected.Build().ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.GOLD_BALANCE:
                        {
                            GoldBalance.Builder goldBalance = GoldBalance.CreateBuilder();
                            using (SqlDataReader dr = db.ExecuteReader(null, "SELECT [GoldBalance] FROM [Account] WHERE [AccountID]=@0", pClient.AccountID))
                            {
                                if (!dr.Read())
                                {
                                    pClient.Disconnect();
                                    return;
                                }
                                goldBalance.SetBonusBalance(0);
                                goldBalance.SetCap(9999999);
                                goldBalance.SetCappedBalance((long)dr["GoldBalance"]);
                                goldBalance.SetCapWarning(2000);
                            }
                            pClient.SendUtilPacket((int)GoldBalance.Types.PacketID.ID, pContext, goldBalance.Build().ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.HERO_XP:
                        {
                            //byte[] temp = new byte[] {
                            //    0x0a, 0x14, 0x08, 0x04, 0x10, 0x07, 0x18, 0x0f, 0x20, 0x82, 0x01, 0x2a, 0x09, 0x08, 0x08, 0x1a, 0x05, 0x0a, 0x03, 0x08, 0x8b, 0x03, 0x0a, 0x13, 0x08, 0x0a,
                            //    0x10, 0x01, 0x18, 0x00, 0x20, 0x46, 0x2a, 0x09, 0x08, 0x02, 0x1a, 0x05, 0x0a, 0x03, 0x08, 0xac, 0x07, 0x0a, 0x13, 0x08, 0x03, 0x10, 0x01, 0x18, 0x00, 0x20,
                            //    0x46, 0x2a, 0x09, 0x08, 0x02, 0x1a, 0x05, 0x0a, 0x03, 0x08, 0xb5, 0x03, 0x0a, 0x13, 0x08, 0x02, 0x10, 0x01, 0x18, 0x00, 0x20, 0x46, 0x2a, 0x09, 0x08, 0x02,
                            //    0x1a, 0x05, 0x0a, 0x03, 0x08, 0xb7, 0x06, 0x0a, 0x13, 0x08, 0x05, 0x10, 0x01, 0x18, 0x00, 0x20, 0x46, 0x2a, 0x09, 0x08, 0x02, 0x1a, 0x05, 0x0a, 0x03, 0x08,
                            //    0xcf, 0x06, 0x0a, 0x13, 0x08, 0x06, 0x10, 0x01, 0x18, 0x00, 0x20, 0x46, 0x2a, 0x09, 0x08, 0x02, 0x1a, 0x05, 0x0a, 0x03, 0x08, 0xd1, 0x0a, 0x0a, 0x13, 0x08,
                            //    0x09, 0x10, 0x01, 0x18, 0x00, 0x20, 0x46, 0x2a, 0x09, 0x08, 0x02, 0x1a, 0x05, 0x0a, 0x03, 0x08, 0xd6, 0x07, 0x0a, 0x13, 0x08, 0x07, 0x10, 0x01, 0x18, 0x00,
                            //    0x20, 0x46, 0x2a, 0x09, 0x08, 0x02, 0x1a, 0x05, 0x0a, 0x03, 0x08, 0xa5, 0x03, 0x0a, 0x13, 0x08, 0x08, 0x10, 0x01, 0x18, 0x00, 0x20, 0x46, 0x2a, 0x09, 0x08,
                            //    0x02, 0x1a, 0x05, 0x0a, 0x03, 0x08, 0x93, 0x09
                            //};
                            //byte[] buf = temp;

                            HeroXP.Builder heroXP = HeroXP.CreateBuilder();
                            using (SqlDataReader dr = db.ExecuteReader(null, "SELECT [ClassID],[Level],[CurrentXP] FROM [HeroXP] WHERE [AccountID]=@0", pClient.AccountID))
                            {
                                while (dr.Read())
                                {
                                    HeroXPInfo.Builder heroXPInfo = HeroXPInfo.CreateBuilder();
                                    heroXPInfo.SetClassId((int)dr["ClassID"]);
                                    heroXPInfo.SetLevel((int)dr["Level"]);
                                    heroXPInfo.SetCurrXp((long)dr["CurrentXP"]);
                                    if (heroXPInfo.Level == 60) heroXPInfo.SetMaxXp(0);
                                    else heroXPInfo.SetMaxXp(heroXPInfo.Level * 100);
                                    //NextHeroLevelReward.Builder nextHeroLevelRewardBuilder = NextHeroLevelReward.CreateBuilder();
                                    //nextHeroLevelRewardBuilder.SetLevel(1).SetRewardBooster(ProfileNoticeRewardCard.CreateBuilder().SetCard(PegasusShared.CardDef.CreateBuilder().Set ));
                                    //heroXPInfoBuilder.SetNextReward(nextHeroLevelRewardBuilder);
                                    heroXP.AddXpInfos(heroXPInfo);
                                }
                            }
                            pClient.SendUtilPacket((int)HeroXP.Types.PacketID.ID, pContext, heroXP.Build().ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.MEDAL_HISTORY:
                        {
                            MedalHistory.Builder medalHistory = MedalHistory.CreateBuilder();
                            medalHistory.AddMedals(MedalHistoryInfo.CreateBuilder().SetMedal((int)Medal.Type.MEDAL_NOVICE).SetRank(0).SetWhen(Date.CreateBuilder().FromDateTime(DateTime.UtcNow)));
                            pClient.SendUtilPacket((int)MedalHistory.Types.PacketID.ID, pContext, medalHistory.Build().ToByteArray());
                            break;
                        }
                    case GetAccountInfo.Types.Request.PVP_QUEUE:
                        {
                            PlayQueue.Builder playQueue = PlayQueue.CreateBuilder();
                            playQueue.SetQueue(PlayQueueInfo.CreateBuilder().SetQueue(0));
                            pClient.SendUtilPacket((int)PlayQueue.Types.PacketID.ID, pContext, playQueue.Build().ToByteArray());
                            break;
                        }
                    default: break;
                }
            }
        }

        [LobbyUtilPacketHandler((int)UpdateLogin.Types.PacketID.ID)]
        public static void UpdateLoginHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            UpdateLogin updateLogin = UpdateLogin.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "UpdateLogin");

            using (SqlConnection db = DB.Open(Settings.Default.Database))
            {
                DateTime lastLoginDate = DateTime.UtcNow;
                db.Execute(null, "UPDATE [Account] SET [LastLogin]=@0 WHERE [AccountID]=@1", lastLoginDate, pClient.AccountID);

                pClient.SendUtilPacket((int)NOP.Types.PacketID.ID, pContext, NOP.CreateBuilder().Build().ToByteArray());
            }
        }

        [LobbyUtilPacketHandler((int)CheckAccountLicenses.Types.PacketID.ID)]
        public static void CheckAccountLicensesHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            CheckAccountLicenses checkAccountLicenses = CheckAccountLicenses.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "CheckAccountLicenses");

            CheckLicensesResponse.Builder checkLicensesResponse = CheckLicensesResponse.CreateBuilder();
            checkLicensesResponse.SetAccountLevel(true);
            checkLicensesResponse.SetSuccess(true);
            pClient.SendUtilPacket((int)CheckLicensesResponse.Types.PacketID.ID, pContext, checkLicensesResponse.Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)CheckGameLicenses.Types.PacketID.ID)]
        public static void CheckGameLicensesHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            CheckGameLicenses checkGameLicenses = CheckGameLicenses.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "CheckGameLicenses");

            CheckLicensesResponse.Builder checkLicensesResponse = CheckLicensesResponse.CreateBuilder();
            checkLicensesResponse.SetAccountLevel(false);
            checkLicensesResponse.SetSuccess(true);
            pClient.SendUtilPacket((int)CheckLicensesResponse.Types.PacketID.ID, pContext, checkLicensesResponse.Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)GetBattlePayConfig.Types.PacketID.ID)]
        public static void GetBattlePayConfigHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            GetBattlePayConfig getBattlePayConfig = GetBattlePayConfig.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "GetBattlePayConfig");

            BattlePayConfigResponse.Builder battlePayConfigResponse = BattlePayConfigResponse.CreateBuilder();
            battlePayConfigResponse.SetCurrency((int)Currency.USD);
            battlePayConfigResponse.SetSecsBeforeAutoCancel(600);
            battlePayConfigResponse.AddGoldPrices(GoldPrice.CreateBuilder().SetProduct((int)ProductType.BOOSTER).SetCost(100).SetData(1));
            battlePayConfigResponse.AddGoldPrices(GoldPrice.CreateBuilder().SetProduct((int)ProductType.DRAFT_TICKET).SetCost(150));
            battlePayConfigResponse.AddBundles(Bundle.CreateBuilder().SetType(1).SetProduct((int)ProductType.BOOSTER).SetQuantity(2).SetCost(2.99).SetData(1));
            battlePayConfigResponse.AddBundles(Bundle.CreateBuilder().SetType(2).SetProduct((int)ProductType.BOOSTER).SetQuantity(7).SetCost(9.99).SetData(1));
            battlePayConfigResponse.AddBundles(Bundle.CreateBuilder().SetType(3).SetProduct((int)ProductType.BOOSTER).SetQuantity(15).SetCost(19.99).SetData(1));
            battlePayConfigResponse.AddBundles(Bundle.CreateBuilder().SetType(4).SetProduct((int)ProductType.BOOSTER).SetQuantity(40).SetCost(49.99).SetData(1));
            battlePayConfigResponse.AddBundles(Bundle.CreateBuilder().SetType(5).SetProduct((int)ProductType.DRAFT_TICKET).SetQuantity(1).SetCost(1.99).SetData(0));
            pClient.SendUtilPacket((int)BattlePayConfigResponse.Types.PacketID.ID, pContext, battlePayConfigResponse.Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)GetOptions.Types.PacketID.ID)]
        public static void GetOptionsHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            GetOptions getOptions = GetOptions.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "GetOptions");
            
            //byte[] temp = new byte[] {
            //    0x0a, 0x0c, 0x08, 0x01, 0x30, 0xff, 0xf9, 0xb3, 0x80, 0x80, 0xe0, 0x80, 0xf8, 0x30, 0x0a, 0x04, 0x08, 0x02, 0x30, 0x03, 0x0a, 0x04, 0x08, 0x06, 0x18, 0x14, 0x0a, 0x04, 0x08, 0x08, 0x18, 0x04, 0x0a, 0x04, 0x08, 0x09, 0x18, 0x00, 0x0a, 0x04, 0x08, 0x0a, 0x18, 0x03, 0x0a, 0x04, 0x08, 0x0b, 0x18, 0x03, 0x0a, 0x07, 0x08, 0x12, 0x20, 0xac, 0x8d, 0x99, 0x04
            //};
            //byte[] buf = temp;
            ClientOptions.Builder clientOptions = ClientOptions.CreateBuilder();
            //clientOptions.AddOptions(PegasusUtil.ClientOption.CreateBuilder().SetIndex((int)Option.HAS_OPENED_BOOSTER).SetAsBool(true));
            pClient.SendUtilPacket((int)ClientOptions.Types.PacketID.ID, pContext, clientOptions.Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)GetAchieves.Types.PacketID.ID)]
        public static void GetAchievesHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            GetAchieves getAchieves = GetAchieves.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "GetAchieves");

            Achieves.Builder achieves = Achieves.CreateBuilder();
            using (SqlConnection db = DB.Open(Settings.Default.Database))
            {
                using (SqlDataReader dr = db.ExecuteReader(null, "SELECT * FROM [Achievement] WHERE [AccountID]=@0 ORDER BY [AchieveID]", pClient.AccountID))
                {
                    while (dr.Read())
                    {
                        Achieve.Builder achieve = Achieve.CreateBuilder();
                        achieve.SetId((int)dr["AchieveID"]);
                        achieve.SetProgress((int)dr["Progress"]);
                        achieve.SetAckProgress((int)dr["AckProgress"]);
                        achieve.SetCompletionCount((int)dr["CompletionCount"]);
                        achieve.SetActive((bool)dr["Active"]);
                        achieve.SetStartedCount((int)dr["StartedCount"]);
                        achieve.SetDateGiven(Date.CreateBuilder().FromDateTime((DateTime)dr["Given"]));
                        if (dr["Completed"] != DBNull.Value) achieve.SetDateCompleted(Date.CreateBuilder().FromDateTime((DateTime)dr["Completed"]));
                        achieve.SetDoNotAck((bool)dr["DoNotAck"]);
                        achieves.AddList(achieve);
                    }
                }
            }
            pClient.SendUtilPacket((int)Achieves.Types.PacketID.ID, pContext, achieves.Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)ValidateAchieve.Types.PacketID.ID)]
        public static void ValidateAchieveHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            ValidateAchieve validateAchieve = ValidateAchieve.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "ValidateAchieve");

            ValidateAchieveResponse.Builder validateAchieveResponse = ValidateAchieveResponse.CreateBuilder();
            validateAchieveResponse.SetAchieve(1);
            pClient.SendUtilPacket((int)ValidateAchieveResponse.Types.PacketID.ID, pContext, validateAchieveResponse.Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)GetBattlePayStatus.Types.PacketID.ID)]
        public static void GetBattlePayStatusHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            GetBattlePayStatus getBattlePayStatus = GetBattlePayStatus.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "GetBattlePayStatus");

            BattlePayStatusResponse.Builder battlePayStatusResponse = BattlePayStatusResponse.CreateBuilder();
            battlePayStatusResponse.SetBattlePayAvailable(true).SetStatus(BattlePayStatusResponse.Types.PurchaseState.PS_READY);
            pClient.SendUtilPacket((int)BattlePayStatusResponse.Types.PacketID.ID, pContext, battlePayStatusResponse.Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)GuardianTrack.Types.PacketID.ID)]
        public static void GuardianTrackHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            GuardianTrack guardianTrack = GuardianTrack.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "GuardianTrack");
            pClient.SendUtilPacket((int)NOP.Types.PacketID.ID, pContext, NOP.CreateBuilder().Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)SetOptions.Types.PacketID.ID)]
        public static void SetOptionsHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            SetOptions setOptions = SetOptions.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "SetOptions");
            //foreach (PegasusUtil.ClientOption clientOption in setOptions.OptionsList)
            //{
            //    if (clientOption.HasAsBool) pClient.Log(LogManagerLevel.Debug, "  {0}: {1}", (Option)clientOption.Index, clientOption.AsBool);
            //    if (clientOption.HasAsFloat) pClient.Log(LogManagerLevel.Debug, "  {0}: {1}", (Option)clientOption.Index, clientOption.AsFloat);
            //    if (clientOption.HasAsInt32) pClient.Log(LogManagerLevel.Debug, "  {0}: {1}", (Option)clientOption.Index, clientOption.AsInt32);
            //    if (clientOption.HasAsInt64) pClient.Log(LogManagerLevel.Debug, "  {0}: {1}", (Option)clientOption.Index, clientOption.AsInt64);
            //    if (clientOption.HasAsUint64) pClient.Log(LogManagerLevel.Debug, "  {0}: {1}", (Option)clientOption.Index, clientOption.AsUint64);
            //}

            pClient.SendUtilPacket((int)NOP.Types.PacketID.ID, pContext, NOP.CreateBuilder().Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)OpenBooster.Types.PacketID.ID)]
        public static void OpenBoosterHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            OpenBooster openBooster = OpenBooster.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "OpenBooster");

            Random random = new Random();
            bool includedRareOrBetter = false;
            DateTime insertDate = DateTime.UtcNow;

            BoosterContent.Builder boosterContent = BoosterContent.CreateBuilder();
            using (SqlConnection db = DB.Open(Settings.Default.Database))
            {
                if (db.ExecuteScalar<int>(null, "SELECT [ExpertBoosters] FROM [Account] WHERE [AccountID]=@0", pClient.AccountID) <= 0)
                {
                    DBAction.Builder dbAction = DBAction.CreateBuilder();
                    dbAction.SetResult(DBAction.Types.Result.E_NOT_FOUND);
                    dbAction.SetAction(DBAction.Types.Action.A_OPEN_BOOSTER);
                    dbAction.SetMetaData(openBooster.BoosterType);
                    pClient.SendUtilPacket((int)DBAction.Types.PacketID.ID, pContext, dbAction.Build().ToByteArray());
                    return;
                }

                for (int index = 0; index < 5; ++index)
                {
                    bool premium = (random.Next(1, 100) % 10) == 0;
                    TAG_RARITY rarity = TAG_RARITY.COMMON;
                    if (index == 4 && !includedRareOrBetter) rarity = TAG_RARITY.RARE;
                    int chanceMod = 5;
                    while (rarity != TAG_RARITY.LEGENDARY && (random.Next(1, 100) % chanceMod) == 0)
                    {
                        rarity++;
                        chanceMod <<= 1;
                        if (rarity == TAG_RARITY.FREE) rarity = TAG_RARITY.RARE;
                    }
                    if (rarity != TAG_RARITY.COMMON) includedRareOrBetter = true;
                    List<Data.Card> expertCollectibleCards = CardManager.ExpertCollectibleCardsByRarity[rarity];
                    Data.Card card = expertCollectibleCards[random.Next(0, expertCollectibleCards.Count - 1)];

                    if (db.ExecuteScalar<int>(null, "SELECT COUNT(*) FROM [Collection] WHERE [AccountID]=@0 AND [CardID]=@1", pClient.AccountID, card.CardID) == 0)
                        db.Execute(null, "INSERT INTO [Collection]([AccountID],[CardID],[Premium],[Count],[CountSeen],[LatestInserted]) VALUES(@0,@1,@2,@3,@4,@5)", pClient.AccountID, card.CardID, Convert.ToInt32(premium), 1, 0, insertDate);
                    else
                    {
                        DB.QueryBuilder query = new DB.QueryBuilder();
                        query.Append("UPDATE [Collection] SET [Count]=[Count]+1,");
                        if (premium) query.Append("[Premium]=1,");
                        query.Append("[LatestInserted]=@0 WHERE [AccountID]=@1 AND [CardID]=@2", insertDate, pClient.AccountID, card.CardID);
                        db.Execute(null, query);
                    }

                    BoosterCard.Builder boosterCard = BoosterCard.CreateBuilder();
                    boosterCard.SetCardDef(PegasusShared.CardDef.CreateBuilder().SetAsset(card.AssetID).SetPremium(Convert.ToInt32(premium)));
                    boosterCard.SetInsertDate(Date.CreateBuilder().FromDateTime(insertDate));
                    boosterContent.AddList(boosterCard);
                }
                db.Execute(null, "UPDATE [Account] SET [ExpertBoosters]=[ExpertBoosters]-1 WHERE [AccountID]=@0", pClient.AccountID);
            }
            pClient.SendUtilPacket((int)BoosterContent.Types.PacketID.ID, pContext, boosterContent.Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)CreateDeck.Types.PacketID.ID)]
        public static void CreateDeckHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            CreateDeck createDeck = CreateDeck.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "CreateDeck");

            int heroPremium = 0;
            if (createDeck.HasHeroPremium) heroPremium = createDeck.HeroPremium;
            long deckID = 0;
            using (SqlConnection db = DB.Open(Settings.Default.Database))
            {
                int deckLimit = db.ExecuteScalar<int>(null, "SELECT [DeckLimit] FROM [Account] WHERE [AccountID]=@0", pClient.AccountID);
                if (db.ExecuteScalar<int>(null, "SELECT COUNT(*) FROM [Deck] WHERE [AccountID]=@0", pClient.AccountID) >= deckLimit)
                {
                    DBAction.Builder dbAction = DBAction.CreateBuilder();
                    dbAction.SetResult(DBAction.Types.Result.E_CONSTRAINT);
                    dbAction.SetAction(DBAction.Types.Action.A_CREATE_DECK);
                    dbAction.SetMetaData(0);
                    pClient.SendUtilPacket((int)DBAction.Types.PacketID.ID, pContext, dbAction.Build().ToByteArray());
                    return;
                }
                deckID = db.ExecuteScalar<long>(null, "INSERT INTO [Deck]([AccountID],[Name],[Hero],[DeckType],[HeroPremium],[Box],[Validity]) OUTPUT INSERTED.[DeckID] VALUES(@0,@1,@2,@3,@4,@5,@6)", pClient.AccountID, createDeck.Name, (int)CardManager.CardsByAssetID[createDeck.Hero].Class, (int)DeckInfo.Types.DeckType.NORMAL_DECK, heroPremium, 0, 0);
            }

            DeckCreated.Builder deckCreated = DeckCreated.CreateBuilder();
            DeckInfo.Builder deckInfo = DeckInfo.CreateBuilder();
            deckInfo.SetId(deckID);
            deckInfo.SetName(createDeck.Name);
            deckInfo.SetHero(createDeck.Hero);
            deckInfo.SetDeckType(DeckInfo.Types.DeckType.NORMAL_DECK);
            if (createDeck.HasHeroPremium) deckInfo.SetHeroPremium(createDeck.HeroPremium);
            deckInfo.SetBox(0);
            deckInfo.SetValidity(0);
            deckCreated.SetInfo(deckInfo);
            pClient.SendUtilPacket((int)DeckCreated.Types.PacketID.ID, pContext, deckCreated.Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)DeleteDeck.Types.PacketID.ID)]
        public static void DeleteDeckHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            DeleteDeck deleteDeck = DeleteDeck.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "DeleteDeck");

            using (SqlConnection db = DB.Open(Settings.Default.Database))
            {
                if (db.ExecuteScalar<int>(null, "SELECT COUNT(*) FROM [Deck] WHERE [AccountID]=@0 AND [DeckID]=@1", pClient.AccountID, deleteDeck.Deck) == 0)
                {
                    DBAction.Builder dbAction = DBAction.CreateBuilder();
                    dbAction.SetResult(DBAction.Types.Result.E_NOT_FOUND);
                    dbAction.SetAction(DBAction.Types.Action.A_DELETE_DECK);
                    dbAction.SetMetaData(deleteDeck.Deck);
                    pClient.SendUtilPacket((int)DBAction.Types.PacketID.ID, pContext, dbAction.Build().ToByteArray());
                    return;
                }
                db.Execute(null, "DELETE FROM [Deck] WHERE [DeckID]=@0", deleteDeck.Deck);
            }

            DeckDeleted.Builder deckDeleted = DeckDeleted.CreateBuilder();
            deckDeleted.SetDeck(deleteDeck.Deck);
            pClient.SendUtilPacket((int)DeckDeleted.Types.PacketID.ID, pContext, deckDeleted.Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)RenameDeck.Types.PacketID.ID)]
        public static void RenameDeckHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            RenameDeck renameDeck = RenameDeck.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "RenameDeck");

            using (SqlConnection db = DB.Open(Settings.Default.Database))
            {
                if (db.ExecuteScalar<int>(null, "SELECT COUNT(*) FROM [Deck] WHERE [AccountID]=@0 AND [DeckID]=@1", pClient.AccountID, renameDeck.Deck) == 0)
                {
                    DBAction.Builder dbAction = DBAction.CreateBuilder();
                    dbAction.SetResult(DBAction.Types.Result.E_NOT_FOUND);
                    dbAction.SetAction(DBAction.Types.Action.A_RENAME_DECK);
                    dbAction.SetMetaData(renameDeck.Deck);
                    pClient.SendUtilPacket((int)DBAction.Types.PacketID.ID, pContext, dbAction.Build().ToByteArray());
                    return;
                }
                db.Execute(null, "UPDATE [Deck] SET [Name]=@0 WHERE [DeckID]=@1", renameDeck.Name, renameDeck.Deck);
            }

            DeckRenamed.Builder deckRenamed = DeckRenamed.CreateBuilder();
            deckRenamed.SetDeck(renameDeck.Deck);
            deckRenamed.SetName(renameDeck.Name);
            pClient.SendUtilPacket((int)DeckRenamed.Types.PacketID.ID, pContext, deckRenamed.Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)DeckSetData.Types.PacketID.ID)]
        public static void DeckSetDataHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            DeckSetData deckSetData = DeckSetData.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "DeckSetData");

            DBAction.Builder dbAction = DBAction.CreateBuilder();
            using (SqlConnection db = DB.Open(Settings.Default.Database))
            {
                if (db.ExecuteScalar<int>(null, "SELECT COUNT(*) FROM [Deck] WHERE [AccountID]=@0 AND [DeckID]=@1", pClient.AccountID, deckSetData.Deck) == 0)
                    dbAction.SetResult(DBAction.Types.Result.E_NOT_FOUND);
                else
                {
                    // TODO: CollectionDeckValidator? Otherwise manually check and update deck Validity
                    db.Execute(null, "DELETE FROM [DeckCard] WHERE [DeckID]=@0", deckSetData.Deck);

                    foreach (DeckCardData deckCardData in deckSetData.CardsList)
                    {
                        Data.Card card = CardManager.CardsByAssetID[deckCardData.Def.Asset];
                        db.Execute(null, "INSERT INTO [DeckCard]([AccountID],[DeckID],[CardID],[Quantity],[Handle],[Previous]) VALUES(@0,@1,@2,@3,@4,@5)", pClient.AccountID, deckSetData.Deck, card.CardID, deckCardData.Qty, deckCardData.Handle, deckCardData.Prev);
                    }

                    dbAction.SetResult(DBAction.Types.Result.E_SUCCESS);
                }
            }
            dbAction.SetAction(DBAction.Types.Action.A_SET_DECK);
            dbAction.SetMetaData(deckSetData.Deck);
            pClient.SendUtilPacket((int)DBAction.Types.PacketID.ID, pContext, dbAction.Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)GetDeck.Types.PacketID.ID)]
        public static void GetDeckHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            GetDeck getDeck = GetDeck.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "GetDeck");

            DeckContents.Builder deckContents = DeckContents.CreateBuilder();
            deckContents.SetDeck(getDeck.Deck);
            using (SqlConnection db = DB.Open(Settings.Default.Database))
            {
                if (db.ExecuteScalar<int>(null, "SELECT COUNT(*) FROM [Deck] WHERE [AccountID]=@0 AND [DeckID]=@1", pClient.AccountID, getDeck.Deck) == 0)
                {
                    DBAction.Builder dbAction = DBAction.CreateBuilder();
                    dbAction.SetResult(DBAction.Types.Result.E_NOT_FOUND);
                    dbAction.SetAction(DBAction.Types.Action.A_GET_DECK);
                    dbAction.SetMetaData(getDeck.Deck);
                    pClient.SendUtilPacket((int)DBAction.Types.PacketID.ID, pContext, dbAction.Build().ToByteArray());
                    return;
                }
                using (SqlDataReader dr = db.ExecuteReader(null, "SELECT [CardID],[Quantity],[Handle],[Previous] FROM [DeckCard] WHERE [DeckID]=@0 ORDER BY [Handle] ASC", getDeck.Deck))
                {
                    while (dr.Read())
                    {
                        DeckCardData.Builder deckCardData = DeckCardData.CreateBuilder();
                        deckCardData.SetDef(PegasusShared.CardDef.CreateBuilder().SetAsset(CardManager.CardsByCardID[(string)dr["CardID"]].AssetID).SetPremium(0));
                        deckCardData.SetQty((int)dr["Quantity"]);
                        deckCardData.SetHandle((int)dr["Handle"]);
                        deckCardData.SetPrev((int)dr["Previous"]);
                        deckContents.AddCards(deckCardData);
                    }
                }
            }
            pClient.SendUtilPacket((int)DeckContents.Types.PacketID.ID, pContext, deckContents.Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)PurchaseWithGold.Types.PacketID.ID)]
        public static void PurchaseWithGoldHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            PurchaseWithGold purchaseWithGold = PurchaseWithGold.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "PurchaseWithGold");

            PurchaseWithGoldResponse.Builder purchaseWithGoldResponse = PurchaseWithGoldResponse.CreateBuilder();
            if (purchaseWithGold.Quantity <= 0 || purchaseWithGold.Quantity > 50)
                purchaseWithGoldResponse.SetResult(PurchaseWithGoldResponse.Types.PurchaseResult.PR_INVALID_QUANTITY);
            else if (purchaseWithGold.Product != (int)ProductType.BOOSTER || purchaseWithGold.Data != 1)
                purchaseWithGoldResponse.SetResult(PurchaseWithGoldResponse.Types.PurchaseResult.PR_PRODUCT_NA);
            else
            {
                long goldCost = 100 * purchaseWithGold.Quantity;

                using (SqlConnection db = DB.Open(Settings.Default.Database))
                {
                    if (db.ExecuteScalar<long>(null, "SELECT [GoldBalance] FROM [Account] WHERE [AccountID]=@0", pClient.AccountID) < goldCost)
                    {
                        purchaseWithGoldResponse.SetResult(PurchaseWithGoldResponse.Types.PurchaseResult.PR_INSUFFICIENT_FUNDS);
                    }
                    else
                    {
                        db.Execute(null, "UPDATE [Account] SET [GoldBalance]=[GoldBalance]-@0,[ExpertBoosters]=[ExpertBoosters]+@1 WHERE [AccountID]=@2", goldCost, purchaseWithGold.Quantity, pClient.AccountID);
                        purchaseWithGoldResponse.SetResult(PurchaseWithGoldResponse.Types.PurchaseResult.PR_SUCCESS);
                        purchaseWithGoldResponse.SetGoldUsed(goldCost);
                    }
                }
            }
            pClient.SendUtilPacket((int)PurchaseWithGoldResponse.Types.PacketID.ID, pContext, purchaseWithGoldResponse.Build().ToByteArray());
        }

        [LobbyUtilPacketHandler((int)BuySellCard.Types.PacketID.ID)]
        public static void BuySellCardHandler(LobbyClient pClient, int pContext, byte[] pData)
        {
            BuySellCard buySellCard = BuySellCard.ParseFrom(pData);
            pClient.Log(LogManagerLevel.Debug, "BuySellCard");

            int buySellCount = 1;
            if (buySellCard.HasCount) buySellCount = buySellCard.Count;

            Data.Card card = CardManager.CardsByAssetID[buySellCard.Def.Asset];
            int sellValue = 0;
            int buyValue = 0;
            switch (card.Rarity)
            {
                case TAG_RARITY.COMMON: sellValue = buySellCard.Def.Premium == 0 ? 5 : 50; buyValue = buySellCard.Def.Premium == 0 ? 40 : 400; break;
                case TAG_RARITY.RARE: sellValue = buySellCard.Def.Premium == 0 ? 20 : 100; buyValue = buySellCard.Def.Premium == 0 ? 100 : 800; break;
                case TAG_RARITY.EPIC: sellValue = buySellCard.Def.Premium == 0 ? 100 : 400; buyValue = buySellCard.Def.Premium == 0 ? 400 : 1600; break;
                case TAG_RARITY.LEGENDARY: sellValue = buySellCard.Def.Premium == 0 ? 400 : 1600; buyValue = buySellCard.Def.Premium == 0 ? 1600 : 3200; break;
                default: break;
            }

            BoughtSoldCard.Builder boughtSoldCard = BoughtSoldCard.CreateBuilder();
            using (SqlConnection db = DB.Open(Settings.Default.Database))
            {
                if (buySellCard.Buying)
                {
                    long arcaneDustBalance = db.ExecuteScalar<long>(null, "SELECT [ArcaneDustBalance] FROM [Account] WHERE [AccountID]=@0", pClient.AccountID);
                    using (SqlDataReader dr = db.ExecuteReader(null, "SELECT [Count],[Premium] FROM [Collection] WHERE [AccountID]=@0 AND [CardID]=@1", pClient.AccountID, card.CardID))
                    {
                        int currentCount = 0;
                        if (dr.Read()) currentCount = (int)dr["Count"];
                        if (currentCount == 0 && buySellCard.Def.Premium != 0) boughtSoldCard.SetResult(BoughtSoldCard.Types.Result.FAILED);
                        else if (currentCount > 0 && buySellCard.Def.Premium != (int)dr["Premium"]) boughtSoldCard.SetResult(BoughtSoldCard.Types.Result.FAILED);
                        else if (arcaneDustBalance < (buyValue * buySellCount)) boughtSoldCard.SetResult(BoughtSoldCard.Types.Result.FAILED);
                        else if (card.Set != TAG_CARD_SET.EXPERT1) boughtSoldCard.SetResult(BoughtSoldCard.Types.Result.SOULBOUND);
                        else if (buySellCard.UnitBuyPrice != buyValue) boughtSoldCard.SetResult(BoughtSoldCard.Types.Result.WRONG_BUY_PRICE);
                        else
                        {
                            if (currentCount > 0)
                                db.Execute(null, "UPDATE [Collection] SET [Count]=[Count]+@0,[CountSeen]=[Count],[LatestInserted]=@1 WHERE [AccountID]=@2 AND [CardID]=@3", buySellCount, DateTime.UtcNow, pClient.AccountID, card.CardID);
                            else
                                db.Execute(null, "INSERT INTO [Collection]([AccountID],[CardID],[Premium],[Count],[CountSeen],[LatestInserted]) VALUES(@0,@1,@2,@3,@4,@5)", pClient.AccountID, card.CardID, buySellCard.Def.Premium, buySellCount, buySellCount, DateTime.UtcNow);
                            db.Execute(null, "UPDATE [Account] SET [ArcaneDustBalance]=[ArcaneDustBalance]-@0 WHERE [AccountID]=@1", buyValue * buySellCount, pClient.AccountID);

                            boughtSoldCard.SetResult(BoughtSoldCard.Types.Result.BOUGHT);
                            boughtSoldCard.SetDef(buySellCard.Def);
                            boughtSoldCard.SetCount(buySellCount);
                            boughtSoldCard.SetNerfed(false);
                            boughtSoldCard.SetAmount(currentCount - buySellCount);
                            boughtSoldCard.SetUnitSellPrice(sellValue);
                            boughtSoldCard.SetUnitBuyPrice(buyValue);
                        }
                    }
                }
                else
                {
                    using (SqlDataReader dr = db.ExecuteReader(null, "SELECT [Count],[Premium] FROM [Collection] WHERE [AccountID]=@0 AND [CardID]=@1", pClient.AccountID, card.CardID))
                    {
                        int currentCount = 0;
                        if (dr.Read()) currentCount = (int)dr["Count"];
                        if (currentCount == 0) boughtSoldCard.SetResult(BoughtSoldCard.Types.Result.FAILED);
                        else if (buySellCard.Def.Premium != (int)dr["Premium"]) boughtSoldCard.SetResult(BoughtSoldCard.Types.Result.FAILED);
                        else if (buySellCount > currentCount) boughtSoldCard.SetResult(BoughtSoldCard.Types.Result.FAILED);
                        else if (card.Set != TAG_CARD_SET.EXPERT1) boughtSoldCard.SetResult(BoughtSoldCard.Types.Result.SOULBOUND);
                        else if (buySellCard.UnitSellPrice != sellValue) boughtSoldCard.SetResult(BoughtSoldCard.Types.Result.WRONG_SELL_PRICE);
                        else
                        {
                            if (buySellCount < currentCount)
                                db.Execute(null, "UPDATE [Collection] SET [Count]=[Count]-@0 WHERE [AccountID]=@1 AND [CardID]=@2", buySellCount, pClient.AccountID, card.CardID);
                            else
                                db.Execute(null, "DELETE FROM [Collection] WHERE [AccountID]=@0 AND [CardID]=@1", pClient.AccountID, card.CardID);
                            db.Execute(null, "UPDATE [Account] SET [ArcaneDustBalance]=[ArcaneDustBalance]+@0 WHERE [AccountID]=@1", sellValue * buySellCount, pClient.AccountID);

                            boughtSoldCard.SetResult(BoughtSoldCard.Types.Result.SOLD);
                            boughtSoldCard.SetDef(buySellCard.Def);
                            boughtSoldCard.SetCount(buySellCount);
                            boughtSoldCard.SetNerfed(false);
                            boughtSoldCard.SetAmount(currentCount - buySellCount);
                            boughtSoldCard.SetUnitSellPrice(sellValue);
                            boughtSoldCard.SetUnitBuyPrice(buyValue);
                        }
                    }
                }
            }
            pClient.SendUtilPacket((int)BoughtSoldCard.Types.PacketID.ID, pContext, boughtSoldCard.Build().ToByteArray());
        }
    }
}
