USE [EACC]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PI_ACH_TEA]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[PI_ACH_TEA]
GO

CREATE PROCEDURE dbo.PI_ACH_TEA
	@ACH_ID								INT,
	@TEA_ID								INT

AS

INSERT INTO [dbo].[TEA_ACH]
           ([ACH_ID]
           ,[TEA_ID])
     VALUES
	 (	
	 @ACH_ID,
	 @TEA_ID		
	 )
GO

GRANT  EXECUTE  ON [dbo].[PI_ACH_TEA]  TO [public]
GO
