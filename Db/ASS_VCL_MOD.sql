USE [EACC]
GO

/****** Object:  Table [dbo].[ASS_VCL_MOD]    Script Date: 09/09/2009 14:41:58 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ASS_VCL_MOD]') AND type in (N'U'))
DROP TABLE [dbo].[ASS_VCL_MOD]
GO

USE [EACC]
GO
CREATE TABLE [dbo].[ASS_VCL_MOD](
	[VCL_ID] [int] NOT NULL,
	[MOD_ID] [int] NOT NULL
 CONSTRAINT [PK_ASS_VCL_MOD] PRIMARY KEY CLUSTERED 
(
	[VCL_ID] ASC,
	[MOD_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

