USE [Eacc]
GO
/****** Object:  UserDefinedTableType [dbo].[TVP_INT_INT_CHAR]    Script Date: 05/25/2009 15:08:59 ******/
IF  EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'TVP_INT_INT_CHAR' AND ss.name = N'dbo')
DROP TYPE [dbo].[TVP_INT_INT_CHAR]
GO
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'TVP_INT_INT_CHAR' AND ss.name = N'dbo')
CREATE TYPE [dbo].[TVP_INT_INT_CHAR] AS TABLE(
	[ID1] [int] NULL,
	[ID2] [int] NULL,
	[LABEL1] [nvarchar](4000) NULL
)
GO
GRANT CONTROL ON TYPE::[dbo].[TVP_INT_INT_CHAR] TO [public] AS [dbo]
GO
