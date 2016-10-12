USE [EACC]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PS_HALLOFFAME]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[PS_HALLOFFAME]
GO

CREATE PROCEDURE dbo.PS_HALLOFFAME
	@DEL_BL	bit = NULL
AS

select tea_id, SUM(VCL_FIGHT_WON_NU) AS SUM_FIGHT,MAX(VCL_FIGHT_WON_NU) AS MAX_FIGHT, SUM(VCL_SQUARE_NU) AS SUM_DISTANCE, MAX(VCL_SQUARE_NU) AS MAX_DISTANCE
from VCL_VEHICLE
group by TEA_ID


GO

GRANT  EXECUTE  ON [dbo].[PS_HALLOFFAME]  TO [public]
GO

