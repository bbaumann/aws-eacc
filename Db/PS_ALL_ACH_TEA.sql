use eacc
go

USE [Eacc]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PS_ACHIEVEMENTS]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[PS_ACHIEVEMENTS]
GO

CREATE PROCEDURE dbo.PS_ACHIEVEMENTS
  @TEA_ID int
AS

select
	   t.TEA_ID,
	   a.ACH_ID,
	   a.ACH_NAME_CH,
	   a.ACH_DESCR_CH,
	   a.ACH_IMG_CH,
	   a.ACH_ECC_RWRD_NU,
	   a.ACH_POS_RWRD_NU
from
	TEA_ACH t
	inner join ACH_ACHIEVMENT a on a.ACH_ID = t.ACH_ID
where TEA_ID = @TEA_ID
	
GO

GRANT  EXECUTE  ON [dbo].[PS_ACHIEVEMENTS]  TO [public]
GO