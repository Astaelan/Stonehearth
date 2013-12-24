USE [Stonehearth]
GO
/****** Object:  User [Stonehearth]    Script Date: 12/24/2013 1:20:48 PM ******/
CREATE USER [Stonehearth] FOR LOGIN [Stonehearth] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [Stonehearth]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [Stonehearth]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 12/24/2013 1:20:48 PM ******/
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
	[Progress] [bigint] NOT NULL,
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
/****** Object:  Table [dbo].[Achieve]    Script Date: 12/24/2013 1:20:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Achieve](
	[AchieveID] [int] NOT NULL,
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
 CONSTRAINT [PK_Achieve] PRIMARY KEY CLUSTERED 
(
	[AchieveID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Achievement]    Script Date: 12/24/2013 1:20:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Achievement](
	[AccountID] [bigint] NOT NULL,
	[AchieveID] [int] NOT NULL,
	[Progress] [int] NOT NULL,
	[AckProgress] [int] NOT NULL,
	[CompletionCount] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[StartedCount] [int] NOT NULL,
	[Given] [datetime] NOT NULL,
	[Completed] [datetime] NULL,
	[DoNotAck] [bit] NOT NULL,
 CONSTRAINT [PK_Achievement] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC,
	[AchieveID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Card]    Script Date: 12/24/2013 1:20:48 PM ******/
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
	[Set] [varchar](32) NOT NULL,
	[Type] [varchar](32) NOT NULL,
	[Craftable] [bit] NOT NULL,
	[Collectible] [bit] NULL,
	[MasterPowerID] [uniqueidentifier] NULL,
	[Faction] [varchar](32) NULL,
	[Race] [varchar](32) NULL,
	[Class] [varchar](32) NULL,
	[Rarity] [varchar](32) NULL,
	[Cost] [int] NULL,
	[Atk] [int] NULL,
	[Health] [int] NULL,
	[AttackVisualType] [int] NULL,
	[TextInHand] [nvarchar](255) NULL,
	[TextInPlay] [nvarchar](255) NULL,
	[DevState] [int] NULL,
	[EnchantmentBirthVisual] [varchar](32) NULL,
	[EnchantmentIdleVisual] [varchar](32) NULL,
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
/****** Object:  Table [dbo].[CardPower]    Script Date: 12/24/2013 1:20:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CardPower](
	[CardID] [varchar](32) NOT NULL,
	[PowerID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_CardPower] PRIMARY KEY CLUSTERED 
(
	[CardID] ASC,
	[PowerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Collection]    Script Date: 12/24/2013 1:20:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Collection](
	[AccountID] [bigint] NOT NULL,
	[CardID] [varchar](32) NOT NULL,
	[Premium] [int] NOT NULL,
	[Count] [int] NOT NULL,
	[CountSeen] [int] NOT NULL,
	[LatestInserted] [datetime] NOT NULL,
 CONSTRAINT [PK_Collection] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC,
	[CardID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Deck]    Script Date: 12/24/2013 1:20:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Deck](
	[DeckID] [bigint] IDENTITY(1,1) NOT NULL,
	[AccountID] [bigint] NOT NULL,
	[Name] [varchar](32) NOT NULL,
	[Hero] [int] NOT NULL,
	[DeckType] [int] NOT NULL,
	[HeroPremium] [int] NOT NULL,
	[Box] [int] NOT NULL,
	[Validity] [bigint] NOT NULL,
 CONSTRAINT [PK_Deck] PRIMARY KEY CLUSTERED 
(
	[DeckID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DeckCard]    Script Date: 12/24/2013 1:20:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DeckCard](
	[AccountID] [bigint] NOT NULL,
	[DeckID] [bigint] NOT NULL,
	[CardID] [varchar](32) NOT NULL,
	[Quantity] [int] NOT NULL,
	[Handle] [int] NOT NULL,
	[Previous] [int] NOT NULL,
 CONSTRAINT [PK_DeckCard] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC,
	[DeckID] ASC,
	[CardID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HeroLevelReward]    Script Date: 12/24/2013 1:20:48 PM ******/
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
/****** Object:  Table [dbo].[HeroXP]    Script Date: 12/24/2013 1:20:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HeroXP](
	[AccountID] [bigint] NOT NULL,
	[ClassID] [int] NOT NULL,
	[Level] [int] NOT NULL,
	[CurrentXP] [bigint] NOT NULL,
 CONSTRAINT [PK_HeroXP] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC,
	[ClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PlayerRecord]    Script Date: 12/24/2013 1:20:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlayerRecord](
	[AccountID] [bigint] NOT NULL,
	[Type] [int] NOT NULL,
	[Hero] [int] NOT NULL,
	[Wins] [int] NOT NULL,
	[Losses] [int] NOT NULL,
	[Ties] [int] NOT NULL,
 CONSTRAINT [PK_PlayerRecord] PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC,
	[Type] ASC,
	[Hero] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Power]    Script Date: 12/24/2013 1:20:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Power](
	[PowerID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Power] PRIMARY KEY CLUSTERED 
(
	[PowerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PowerRequirement]    Script Date: 12/24/2013 1:20:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PowerRequirement](
	[PowerID] [uniqueidentifier] NOT NULL,
	[Type] [varchar](64) NOT NULL,
	[Parameter] [int] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_CardPower_CardID]    Script Date: 12/24/2013 1:20:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_CardPower_CardID] ON [dbo].[CardPower]
(
	[CardID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Collection_AccountID]    Script Date: 12/24/2013 1:20:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_Collection_AccountID] ON [dbo].[Collection]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Deck_AccountID]    Script Date: 12/24/2013 1:20:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_Deck_AccountID] ON [dbo].[Deck]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DeckCard_AccountID]    Script Date: 12/24/2013 1:20:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_DeckCard_AccountID] ON [dbo].[DeckCard]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DeckCard_AccountIDAndDeckID]    Script Date: 12/24/2013 1:20:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_DeckCard_AccountIDAndDeckID] ON [dbo].[DeckCard]
(
	[AccountID] ASC,
	[DeckID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_HeroLevelReward_ClassID]    Script Date: 12/24/2013 1:20:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_HeroLevelReward_ClassID] ON [dbo].[HeroLevelReward]
(
	[ClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_HeroXP_AccountID]    Script Date: 12/24/2013 1:20:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_HeroXP_AccountID] ON [dbo].[HeroXP]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PlayerRecord_AccountID]    Script Date: 12/24/2013 1:20:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_PlayerRecord_AccountID] ON [dbo].[PlayerRecord]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PowerRequirement_PowerID]    Script Date: 12/24/2013 1:20:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_PowerRequirement_PowerID] ON [dbo].[PowerRequirement]
(
	[PowerID] ASC
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
ALTER TABLE [dbo].[Achieve] ADD  CONSTRAINT [DF_Achieve_Group]  DEFAULT ('') FOR [Group]
GO
ALTER TABLE [dbo].[Achieve] ADD  CONSTRAINT [DF_Achieve_MaxProgress]  DEFAULT ((0)) FOR [MaxProgress]
GO
ALTER TABLE [dbo].[Achieve] ADD  CONSTRAINT [DF_Achieve_RaceRequirement]  DEFAULT ((0)) FOR [RaceRequirement]
GO
ALTER TABLE [dbo].[Achieve] ADD  CONSTRAINT [DF_Achieve_CardSetRequirement]  DEFAULT ((0)) FOR [CardSetRequirement]
GO
ALTER TABLE [dbo].[Achieve] ADD  CONSTRAINT [DF_Achieve_RewardType]  DEFAULT ('') FOR [RewardType]
GO
ALTER TABLE [dbo].[Achieve] ADD  CONSTRAINT [DF_Achieve_Parameter1]  DEFAULT ((0)) FOR [Parameter1]
GO
ALTER TABLE [dbo].[Achieve] ADD  CONSTRAINT [DF_Achieve_Parameter2]  DEFAULT ((0)) FOR [Parameter2]
GO
ALTER TABLE [dbo].[Achieve] ADD  CONSTRAINT [DF_Achieve_UnlockableFeature]  DEFAULT ('') FOR [UnlockableFeature]
GO
ALTER TABLE [dbo].[Achieve] ADD  CONSTRAINT [DF_Achieve_ParentID]  DEFAULT ((0)) FOR [ParentID]
GO
ALTER TABLE [dbo].[Achieve] ADD  CONSTRAINT [DF_Achieve_Trigger]  DEFAULT ('') FOR [Trigger]
GO
ALTER TABLE [dbo].[Achievement] ADD  CONSTRAINT [DF_Achievement_Given]  DEFAULT (getutcdate()) FOR [Given]
GO
ALTER TABLE [dbo].[Achievement] ADD  CONSTRAINT [DF_Achievement_Completed]  DEFAULT (NULL) FOR [Completed]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Name]  DEFAULT ('') FOR [Name]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Set]  DEFAULT ('') FOR [Set]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_Type]  DEFAULT ('') FOR [Type]
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
ALTER TABLE [dbo].[Collection] ADD  CONSTRAINT [DF_Collection_Premium]  DEFAULT ((0)) FOR [Premium]
GO
ALTER TABLE [dbo].[Collection] ADD  CONSTRAINT [DF_Collection_LatestInserted]  DEFAULT (getutcdate()) FOR [LatestInserted]
GO
ALTER TABLE [dbo].[PlayerRecord] ADD  CONSTRAINT [DF_PlayerRecord_Wins]  DEFAULT ((0)) FOR [Wins]
GO
ALTER TABLE [dbo].[PlayerRecord] ADD  CONSTRAINT [DF_PlayerRecord_Losses]  DEFAULT ((0)) FOR [Losses]
GO
ALTER TABLE [dbo].[PlayerRecord] ADD  CONSTRAINT [DF_PlayerRecord_Ties]  DEFAULT ((0)) FOR [Ties]
GO
ALTER TABLE [dbo].[PowerRequirement] ADD  CONSTRAINT [DF_PowerRequirement_Parameter]  DEFAULT (NULL) FOR [Parameter]
GO
