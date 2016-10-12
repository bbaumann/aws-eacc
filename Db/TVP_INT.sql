USE [Eacc]
GO
/****** Object:  UserDefinedTableType [dbo].[TVP_INT]    Script Date: 05/25/2009 15:08:59 ******/
IF  EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'TVP_INT' AND ss.name = N'dbo')
DROP TYPE [dbo].[TVP_INT]
GO
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'TVP_INT' AND ss.name = N'dbo')
CREATE TYPE [dbo].[TVP_INT] AS TABLE(
	[ID1] [int] NULL
)
GO
GRANT CONTROL ON TYPE::[dbo].[TVP_INT] TO [public] AS [dbo]
GO

