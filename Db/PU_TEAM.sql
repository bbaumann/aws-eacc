USE [Eacc]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PU_TEAM]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[PU_TEAM]
GO

CREATE PROCEDURE dbo.PU_TEAM
@TEA_ID int,
@TEA_MONEY_NU int,
@TEA_SCORE_NU int
AS

update TEA_TEAM
set
      TEA_MONEY_NU = @TEA_MONEY_NU,
      TEA_SCORE_NU = @TEA_SCORE_NU
where TEA_ID = @TEA_ID
	
GO

GRANT  EXECUTE  ON [dbo].[PU_TEAM]  TO [public]
GO

