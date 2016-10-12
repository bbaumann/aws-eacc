USE [EACC]
GO

/****** Object:  Table [dbo].[TEA_TEAM]    Script Date: 09/09/2009 14:41:58 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[[TEA_ACH]]') AND type in (N'U'))
DROP TABLE [dbo].[TEA_ACH]
GO

USE [EACC]
GO
CREATE TABLE [dbo].[TEA_ACH](
	[ACH_ID] [int] NOT NULL ,
	[TEA_ID] [int] NOT NULL )
GO