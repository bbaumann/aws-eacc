USE [Eacc]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PS_ALL_TEAMS]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[PS_ALL_TEAMS]
GO

CREATE PROCEDURE dbo.PS_ALL_TEAMS
AS

select
	   TEA_ID
      ,TEA_NAME_CH
      ,TEA_COLOR_CH
      ,TEA_MONEY_NU
      ,TEA_SCORE_NU
      ,TEA_SECRET_CH
from
	TEA_TEAM
	
GO

GRANT  EXECUTE  ON [dbo].[PS_ALL_TEAMS]  TO [public]
GO

