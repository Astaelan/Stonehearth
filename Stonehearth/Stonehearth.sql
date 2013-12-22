USE [Stonehearth]
GO
/****** Object:  User [Stonehearth]    Script Date: 12/22/2013 9:05:38 AM ******/
CREATE USER [Stonehearth] FOR LOGIN [Stonehearth] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [Stonehearth]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [Stonehearth]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 12/22/2013 9:05:38 AM ******/
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
/****** Object:  Table [dbo].[Achievement]    Script Date: 12/22/2013 9:05:38 AM ******/
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
/****** Object:  Table [dbo].[Collection]    Script Date: 12/22/2013 9:05:38 AM ******/
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
/****** Object:  Table [dbo].[Deck]    Script Date: 12/22/2013 9:05:38 AM ******/
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
/****** Object:  Table [dbo].[DeckCard]    Script Date: 12/22/2013 9:05:38 AM ******/
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
/****** Object:  Table [dbo].[HeroLevelReward]    Script Date: 12/22/2013 9:05:38 AM ******/
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
/****** Object:  Table [dbo].[HeroXP]    Script Date: 12/22/2013 9:05:38 AM ******/
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
/****** Object:  Table [dbo].[PlayerRecord]    Script Date: 12/22/2013 9:05:38 AM ******/
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
/****** Object:  Index [IX_Collection_AccountID]    Script Date: 12/22/2013 9:05:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_Collection_AccountID] ON [dbo].[Collection]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Deck_AccountID]    Script Date: 12/22/2013 9:05:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_Deck_AccountID] ON [dbo].[Deck]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DeckCard_AccountID]    Script Date: 12/22/2013 9:05:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_DeckCard_AccountID] ON [dbo].[DeckCard]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DeckCard_AccountIDAndDeckID]    Script Date: 12/22/2013 9:05:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_DeckCard_AccountIDAndDeckID] ON [dbo].[DeckCard]
(
	[AccountID] ASC,
	[DeckID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_HeroLevelReward_ClassID]    Script Date: 12/22/2013 9:05:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_HeroLevelReward_ClassID] ON [dbo].[HeroLevelReward]
(
	[ClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_HeroXP_AccountID]    Script Date: 12/22/2013 9:05:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_HeroXP_AccountID] ON [dbo].[HeroXP]
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PlayerRecord_AccountID]    Script Date: 12/22/2013 9:05:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_PlayerRecord_AccountID] ON [dbo].[PlayerRecord]
(
	[AccountID] ASC
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
ALTER TABLE [dbo].[Achievement] ADD  CONSTRAINT [DF_Achievement_Given]  DEFAULT (getutcdate()) FOR [Given]
GO
ALTER TABLE [dbo].[Achievement] ADD  CONSTRAINT [DF_Achievement_Completed]  DEFAULT (NULL) FOR [Completed]
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
