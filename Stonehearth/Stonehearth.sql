USE [master]
GO
/****** Object:  Database [Stonehearth]    Script Date: 12/25/2013 10:10:53 AM ******/
CREATE DATABASE [Stonehearth]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Stonehearth', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\Stonehearth.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Stonehearth_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\Stonehearth_log.ldf' , SIZE = 2304KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Stonehearth] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Stonehearth].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Stonehearth] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Stonehearth] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Stonehearth] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Stonehearth] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Stonehearth] SET ARITHABORT OFF 
GO
ALTER DATABASE [Stonehearth] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Stonehearth] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Stonehearth] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Stonehearth] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Stonehearth] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Stonehearth] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Stonehearth] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Stonehearth] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Stonehearth] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Stonehearth] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Stonehearth] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Stonehearth] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Stonehearth] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Stonehearth] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Stonehearth] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Stonehearth] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Stonehearth] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Stonehearth] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Stonehearth] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Stonehearth] SET  MULTI_USER 
GO
ALTER DATABASE [Stonehearth] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Stonehearth] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Stonehearth] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Stonehearth] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [Stonehearth]
GO
/****** Object:  User [Stonehearth]    Script Date: 12/25/2013 10:10:53 AM ******/
CREATE USER [Stonehearth] FOR LOGIN [Stonehearth] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [Stonehearth]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [Stonehearth]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 12/25/2013 10:10:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Account](
	[AccountID] [bigint] NOT NULL,
	[Email] [varchar](255) NOT NULL,
	[Password] [varchar](20) NOT NULL,
	[SessionID] [uniqueidentifier] NULL,
	[SessionHost] [varchar](15) NULL,
	[SessionExpiration] [datetime] NULL,
	[LastLogin] [datetime] NOT NULL,
	[ExpertBoosters] [int] NOT NULL,
	[BestForge] [int] NOT NULL,
	[LastForge] [datetime] NOT NULL,
	[Progress] [int] NOT NULL,
	[DeckLimit] [int] NOT NULL,
	[ArcaneDustBalance] [bigint] NOT NULL,
	[GoldBalance] [bigint] NOT NULL,
 CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Account_Email] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AccountAchievement]    Script Date: 12/25/2013 10:10:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountAchievement](
	[AccountID] [bigint] NOT NULL,
	[AchievementID] [int] NOT NULL,
	[Progress] [int] NOT NULL,
	[AckProgress] [int] NOT NULL,
	[CompletionCount] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[StartedCount] [int] NOT NULL,
	[Given] [datetime] NOT NULL,
	[Completed] [datetime] NULL,
	[DoNotAck] [bit] NOT NULL,
 CONSTRAINT [PK_AccountAchievement] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC,
	[AchievementID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AccountCard]    Script Date: 12/25/2013 10:10:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AccountCard](
	[AccountID] [bigint] NOT NULL,
	[CardID] [varchar](32) NOT NULL,
	[Premium] [int] NOT NULL,
	[Count] [int] NOT NULL,
	[CountSeen] [int] NOT NULL,
	[LatestInserted] [datetime] NOT NULL,
 CONSTRAINT [PK_AccountCard] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC,
	[CardID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AccountDeck]    Script Date: 12/25/2013 10:10:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AccountDeck](
	[AccountDeckID] [bigint] IDENTITY(1,1) NOT NULL,
	[AccountID] [bigint] NOT NULL,
	[Name] [varchar](32) NOT NULL,
	[Hero] [int] NOT NULL,
	[DeckType] [int] NOT NULL,
	[HeroPremium] [int] NOT NULL,
	[Box] [int] NOT NULL,
	[Validity] [int] NOT NULL,
 CONSTRAINT [PK_AccountDeck] PRIMARY KEY CLUSTERED 
(
	[AccountDeckID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AccountDeckCard]    Script Date: 12/25/2013 10:10:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AccountDeckCard](
	[AccountID] [bigint] NOT NULL,
	[AccountDeckID] [bigint] NOT NULL,
	[CardID] [varchar](32) NOT NULL,
	[Quantity] [int] NOT NULL,
	[Handle] [int] NOT NULL,
	[Previous] [int] NOT NULL,
 CONSTRAINT [PK_AccountDeckCard] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC,
	[AccountDeckID] ASC,
	[CardID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AccountHero]    Script Date: 12/25/2013 10:10:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountHero](
	[AccountID] [bigint] NOT NULL,
	[ClassID] [int] NOT NULL,
	[Level] [int] NOT NULL,
	[CurrentXP] [bigint] NOT NULL,
 CONSTRAINT [PK_AccountHero] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC,
	[ClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AccountRecord]    Script Date: 12/25/2013 10:10:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountRecord](
	[AccountID] [bigint] NOT NULL,
	[Type] [int] NOT NULL,
	[Hero] [int] NOT NULL,
	[Wins] [int] NOT NULL,
	[Losses] [int] NOT NULL,
	[Ties] [int] NOT NULL,
 CONSTRAINT [PK_AccountRecord] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC,
	[Type] ASC,
	[Hero] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Achievement]    Script Date: 12/25/2013 10:10:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Achievement](
	[AchievementID] [int] NOT NULL,
	[Group] [varchar](16) NOT NULL,
	[MaxProgress] [int] NOT NULL,
	[RaceRequirement] [int] NOT NULL,
	[CardSetRequirement] [int] NOT NULL,
	[RewardType] [varchar](16) NOT NULL,
	[Parameter1] [int] NOT NULL,
	[Parameter2] [int] NOT NULL,
	[UnlockableFeature] [varchar](16) NOT NULL,
	[ParentID] [int] NOT NULL,
	[Trigger] [varchar](16) NOT NULL,
 CONSTRAINT [PK_Achievement] PRIMARY KEY CLUSTERED 
(
	[AchievementID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Card]    Script Date: 12/25/2013 10:10:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Card](
	[CardID] [varchar](32) NOT NULL,
	[AssetID] [int] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Set] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[Craftable] [bit] NOT NULL,
	[Collectible] [bit] NULL,
	[MasterPowerID] [uniqueidentifier] NULL,
	[Faction] [int] NULL,
	[Race] [int] NULL,
	[Class] [int] NULL,
	[Rarity] [int] NULL,
	[Cost] [int] NULL,
	[Atk] [int] NULL,
	[Health] [int] NULL,
	[AttackVisualType] [int] NULL,
	[TextInHand] [nvarchar](255) NULL,
	[TextInPlay] [nvarchar](255) NULL,
	[DevState] [int] NULL,
	[EnchantmentBirthVisual] [int] NULL,
	[EnchantmentIdleVisual] [int] NULL,
	[FlavorText] [nvarchar](255) NULL,
	[ArtistName] [nvarchar](32) NULL,
	[TargetingArrowText] [nvarchar](255) NULL,
	[HowToGetThisGoldCard] [nvarchar](255) NULL,
	[HowToGetThisCard] [nvarchar](255) NULL,
	[StandardBuyValue] [int] NULL,
	[StandardSellValue] [int] NULL,
	[FoilBuyValue] [int] NULL,
	[FoilSellValue] [int] NULL,
	[Recall] [int] NULL,
	[Durability] [int] NULL,
	[TriggerVisual] [bit] NULL,
	[Elite] [bit] NULL,
	[Deathrattle] [bit] NULL,
	[Charge] [bit] NULL,
	[DivineShield] [bit] NULL,
	[Windfury] [bit] NULL,
	[Taunt] [bit] NULL,
	[Aura] [bit] NULL,
	[Enrage] [bit] NULL,
	[OneTurnEffect] [bit] NULL,
	[Stealth] [bit] NULL,
	[Battlecry] [bit] NULL,
	[Secret] [bit] NULL,
	[Morph] [bit] NULL,
	[AffectedBySpellPower] [bit] NULL,
	[Freeze] [bit] NULL,
	[Spellpower] [bit] NULL,
	[Combo] [bit] NULL,
	[Silence] [bit] NULL,
	[Summoned] [bit] NULL,
	[ImmuneToSpellpower] [bit] NULL,
	[AdjacentBuff] [bit] NULL,
	[GrantCharge] [bit] NULL,
	[Poisonous] [bit] NULL,
	[HealTarget] [bit] NULL,
 CONSTRAINT [PK_Card] PRIMARY KEY CLUSTERED 
(
	[CardID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_Card_AssetID] UNIQUE NONCLUSTERED 
(
	[AssetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CardPower]    Script Date: 12/25/2013 10:10:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CardPower](
	[CardPowerID] [uniqueidentifier] NOT NULL,
	[CardID] [varchar](32) NOT NULL,
 CONSTRAINT [PK_CardPower] PRIMARY KEY CLUSTERED 
(
	[CardPowerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CardPowerRequirement]    Script Date: 12/25/2013 10:10:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CardPowerRequirement](
	[CardPowerID] [uniqueidentifier] NOT NULL,
	[Type] [int] NOT NULL,
	[Parameter] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HeroLevelReward]    Script Date: 12/25/2013 10:10:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HeroLevelReward](
	[ClassID] [int] NOT NULL,
	[Level] [int] NOT NULL,
	[CardID] [varchar](32) NOT NULL,
	[CardCount] [int] NOT NULL,
	[CardPremium] [int] NOT NULL,
 CONSTRAINT [PK_HeroLevelReward] PRIMARY KEY CLUSTERED 
(
	[ClassID] ASC,
	[Level] ASC,
	[CardID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [IX_AccountAchievement_AccountID]    Script Date: 12/25/2013 10:10:53 AM ******/
CREATE NONCLUSTERED INDEX [IX_AccountAchievement_AccountID] ON [dbo].[AccountAchievement]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AccountCard_AccountID]    Script Date: 12/25/2013 10:10:53 AM ******/
CREATE NONCLUSTERED INDEX [IX_AccountCard_AccountID] ON [dbo].[AccountCard]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AccountDeck_AccountID]    Script Date: 12/25/2013 10:10:53 AM ******/
CREATE NONCLUSTERED INDEX [IX_AccountDeck_AccountID] ON [dbo].[AccountDeck]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AccountDeckCard_AccountID]    Script Date: 12/25/2013 10:10:53 AM ******/
CREATE NONCLUSTERED INDEX [IX_AccountDeckCard_AccountID] ON [dbo].[AccountDeckCard]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AccountDeckCard_AccountIDAndAccountDeckID]    Script Date: 12/25/2013 10:10:53 AM ******/
CREATE NONCLUSTERED INDEX [IX_AccountDeckCard_AccountIDAndAccountDeckID] ON [dbo].[AccountDeckCard]
(
	[AccountID] ASC,
	[AccountDeckID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AccountHero_AccountID]    Script Date: 12/25/2013 10:10:53 AM ******/
CREATE NONCLUSTERED INDEX [IX_AccountHero_AccountID] ON [dbo].[AccountHero]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AccountRecord_AccountID]    Script Date: 12/25/2013 10:10:53 AM ******/
CREATE NONCLUSTERED INDEX [IX_AccountRecord_AccountID] ON [dbo].[AccountRecord]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_CardPower_CardID]    Script Date: 12/25/2013 10:10:53 AM ******/
CREATE NONCLUSTERED INDEX [IX_CardPower_CardID] ON [dbo].[CardPower]
(
	[CardID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_CardPowerRequirement_CardPowerID]    Script Date: 12/25/2013 10:10:53 AM ******/
CREATE NONCLUSTERED INDEX [IX_CardPowerRequirement_CardPowerID] ON [dbo].[CardPowerRequirement]
(
	[CardPowerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_HeroLevelReward_ClassID]    Script Date: 12/25/2013 10:10:53 AM ******/
CREATE NONCLUSTERED INDEX [IX_HeroLevelReward_ClassID] ON [dbo].[HeroLevelReward]
(
	[ClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_LastLogin]  DEFAULT (getutcdate()) FOR [LastLogin]
GO
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_ExpertBoosters]  DEFAULT ((1)) FOR [ExpertBoosters]
GO
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_BestForge]  DEFAULT ((0)) FOR [BestForge]
GO
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_LastForge]  DEFAULT (getutcdate()) FOR [LastForge]
GO
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_Progress]  DEFAULT ((0)) FOR [Progress]
GO
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_DeckLimit]  DEFAULT ((50)) FOR [DeckLimit]
GO
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_ArcaneDustBalance]  DEFAULT ((0)) FOR [ArcaneDustBalance]
GO
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF_Account_GoldBalance]  DEFAULT ((0)) FOR [GoldBalance]
GO
ALTER TABLE [dbo].[AccountAchievement] ADD  CONSTRAINT [DF_AccountAchievement_Given]  DEFAULT (getutcdate()) FOR [Given]
GO
ALTER TABLE [dbo].[AccountAchievement] ADD  CONSTRAINT [DF_AccountAchievement_Completed]  DEFAULT (NULL) FOR [Completed]
GO
ALTER TABLE [dbo].[AccountCard] ADD  CONSTRAINT [DF_AccountCard_Premium]  DEFAULT ((0)) FOR [Premium]
GO
ALTER TABLE [dbo].[AccountCard] ADD  CONSTRAINT [DF_AccountCard_LatestInserted]  DEFAULT (getutcdate()) FOR [LatestInserted]
GO
ALTER TABLE [dbo].[AccountRecord] ADD  CONSTRAINT [DF_AccountRecord_Wins]  DEFAULT ((0)) FOR [Wins]
GO
ALTER TABLE [dbo].[AccountRecord] ADD  CONSTRAINT [DF_AccountRecord_Losses]  DEFAULT ((0)) FOR [Losses]
GO
ALTER TABLE [dbo].[AccountRecord] ADD  CONSTRAINT [DF_AccountRecord_Ties]  DEFAULT ((0)) FOR [Ties]
GO
ALTER TABLE [dbo].[Achievement] ADD  CONSTRAINT [DF_Achievement_Group]  DEFAULT ('') FOR [Group]
GO
ALTER TABLE [dbo].[Achievement] ADD  CONSTRAINT [DF_Achievement_MaxProgress]  DEFAULT ((0)) FOR [MaxProgress]
GO
ALTER TABLE [dbo].[Achievement] ADD  CONSTRAINT [DF_Achievement_RaceRequirement]  DEFAULT ((0)) FOR [RaceRequirement]
GO
ALTER TABLE [dbo].[Achievement] ADD  CONSTRAINT [DF_Achievement_CardSetRequirement]  DEFAULT ((0)) FOR [CardSetRequirement]
GO
ALTER TABLE [dbo].[Achievement] ADD  CONSTRAINT [DF_Achievement_RewardType]  DEFAULT ('') FOR [RewardType]
GO
ALTER TABLE [dbo].[Achievement] ADD  CONSTRAINT [DF_Achievement_Parameter1]  DEFAULT ((0)) FOR [Parameter1]
GO
ALTER TABLE [dbo].[Achievement] ADD  CONSTRAINT [DF_Achievement_Parameter2]  DEFAULT ((0)) FOR [Parameter2]
GO
ALTER TABLE [dbo].[Achievement] ADD  CONSTRAINT [DF_Achievement_UnlockableFeature]  DEFAULT ('') FOR [UnlockableFeature]
GO
ALTER TABLE [dbo].[Achievement] ADD  CONSTRAINT [DF_Achievement_ParentID]  DEFAULT ((0)) FOR [ParentID]
GO
ALTER TABLE [dbo].[Achievement] ADD  CONSTRAINT [DF_Achievement_Trigger]  DEFAULT ('') FOR [Trigger]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Name]  DEFAULT ('') FOR [Name]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Set]  DEFAULT ((0)) FOR [Set]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Type]  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Craftable]  DEFAULT ((0)) FOR [Craftable]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Collectible]  DEFAULT (NULL) FOR [Collectible]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_MasterPowerID]  DEFAULT (NULL) FOR [MasterPowerID]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Faction]  DEFAULT (NULL) FOR [Faction]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Race]  DEFAULT (NULL) FOR [Race]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Class]  DEFAULT (NULL) FOR [Class]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Rarity]  DEFAULT (NULL) FOR [Rarity]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Cost]  DEFAULT (NULL) FOR [Cost]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Atk]  DEFAULT (NULL) FOR [Atk]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Health]  DEFAULT (NULL) FOR [Health]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_AttackVisualType]  DEFAULT (NULL) FOR [AttackVisualType]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_TextInHand]  DEFAULT (NULL) FOR [TextInHand]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_TextInPlay]  DEFAULT (NULL) FOR [TextInPlay]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_DevState]  DEFAULT (NULL) FOR [DevState]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_EnchantmentBirthVisual]  DEFAULT (NULL) FOR [EnchantmentBirthVisual]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_EnchantmentIdleVisual]  DEFAULT (NULL) FOR [EnchantmentIdleVisual]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_FlavorText]  DEFAULT (NULL) FOR [FlavorText]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_ArtistName]  DEFAULT (NULL) FOR [ArtistName]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_TargetingArrowText]  DEFAULT (NULL) FOR [TargetingArrowText]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_HowToGetThisGoldCard]  DEFAULT (NULL) FOR [HowToGetThisGoldCard]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_HowToGetThisCard]  DEFAULT (NULL) FOR [HowToGetThisCard]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_StandardBuyValue]  DEFAULT (NULL) FOR [StandardBuyValue]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_StandardSellValue]  DEFAULT (NULL) FOR [StandardSellValue]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_FoilBuyValue]  DEFAULT (NULL) FOR [FoilBuyValue]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_FoilSellValue]  DEFAULT (NULL) FOR [FoilSellValue]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Recall]  DEFAULT (NULL) FOR [Recall]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Durability]  DEFAULT (NULL) FOR [Durability]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_TriggerVisual]  DEFAULT (NULL) FOR [TriggerVisual]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Elite]  DEFAULT (NULL) FOR [Elite]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Deathrattle]  DEFAULT (NULL) FOR [Deathrattle]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Charge]  DEFAULT (NULL) FOR [Charge]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_DivineShield]  DEFAULT (NULL) FOR [DivineShield]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Windfury]  DEFAULT (NULL) FOR [Windfury]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Taunt]  DEFAULT (NULL) FOR [Taunt]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Aura]  DEFAULT (NULL) FOR [Aura]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Enrage]  DEFAULT (NULL) FOR [Enrage]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_OneTurnEffect]  DEFAULT (NULL) FOR [OneTurnEffect]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Stealth]  DEFAULT (NULL) FOR [Stealth]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Battlecry]  DEFAULT (NULL) FOR [Battlecry]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Secret]  DEFAULT (NULL) FOR [Secret]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Morph]  DEFAULT (NULL) FOR [Morph]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_AffectedBySpellPower]  DEFAULT (NULL) FOR [AffectedBySpellPower]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Freeze]  DEFAULT (NULL) FOR [Freeze]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Spellpower]  DEFAULT (NULL) FOR [Spellpower]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Combo]  DEFAULT (NULL) FOR [Combo]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Silence]  DEFAULT (NULL) FOR [Silence]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Summoned]  DEFAULT (NULL) FOR [Summoned]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_ImmuneToSpellpower]  DEFAULT (NULL) FOR [ImmuneToSpellpower]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_AdjacentBuff]  DEFAULT (NULL) FOR [AdjacentBuff]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_GrantCharge]  DEFAULT (NULL) FOR [GrantCharge]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Poisonous]  DEFAULT (NULL) FOR [Poisonous]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_HealTarget]  DEFAULT (NULL) FOR [HealTarget]
GO
ALTER TABLE [dbo].[CardPowerRequirement] ADD  CONSTRAINT [DF_CardPowerRequirement_Parameter]  DEFAULT (NULL) FOR [Parameter]
GO
USE [master]
GO
ALTER DATABASE [Stonehearth] SET  READ_WRITE 
GO
