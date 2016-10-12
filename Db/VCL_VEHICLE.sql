USE [EACC]
GO

/****** Object:  Table [dbo].[VCL_VEHICLE]    Script Date: 09/09/2009 14:41:58 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VCL_VEHICLE]') AND type in (N'U'))
DROP TABLE [dbo].[VCL_VEHICLE]
GO

USE [EACC]
GO
CREATE TABLE [dbo].[VCL_VEHICLE](
	[VCL_ID] [int] NOT NULL identity,
	[TEA_ID] [int] NOT NULL,
	[VCL_NAME_CH] [nvarchar](500) NOT NULL,
	[VCL_SQUARE_NU] [int] NOT NULL,
	[VCL_DOMAIN_CH] [nvarchar](2048) NOT NULL,
	[DEL_BL] [bit] NOT NULL,
	[VCL_TURNS_TO_MISS_NU] INT NOT NULL default(0),
	[VCL_FIGHT_WON_NU] INT NOT NULL  DEFAULT (0),
 CONSTRAINT [PK_VCL_VEHICLE] PRIMARY KEY CLUSTERED 
(
	[VCL_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

