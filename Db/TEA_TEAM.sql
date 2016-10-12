USE [EACC]
GO

/****** Object:  Table [dbo].[TEA_TEAM]    Script Date: 09/09/2009 14:41:58 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TEA_TEAM]') AND type in (N'U'))
DROP TABLE [dbo].[TEA_TEAM]
GO

USE [EACC]
GO
CREATE TABLE [dbo].[TEA_TEAM](
	[TEA_ID] [int] NOT NULL identity,
	[TEA_NAME_CH] [nvarchar](500) NOT NULL,
	[TEA_COLOR_CH] [nvarchar](500) NOT NULL,
	[TEA_MONEY_NU] [int] NOT NULL,
	[TEA_SCORE_NU] [int] NOT NULL,
	[TEA_SECRET_CH] [nvarchar](500) NULL,
 CONSTRAINT [PK_TEA_TEAM] PRIMARY KEY CLUSTERED 
(
	[TEA_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
