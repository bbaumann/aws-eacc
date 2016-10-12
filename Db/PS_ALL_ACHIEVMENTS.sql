USE Eacc
GO
if exists (select * from dbo.sysobjects where id = object_id(N'dbo.PS_ALL_ACHIEVMENTS') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure dbo.PS_ALL_ACHIEVMENTS
GO

CREATE PROCEDURE dbo.PS_ALL_ACHIEVMENTS
AS

select
ACH_ID
      ,ACH_NAME_CH
      ,ACH_DESCR_CH
      ,ACH_IMG_CH
      ,ACH_ECC_RWRD_NU
      ,ACH_POS_RWRD_NU
from
	ACH_ACHIEVMENT
	
GO

GRANT  EXECUTE  ON dbo.PS_ALL_ACHIEVMENTS  TO public
GO