USE [EACC]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PU_FIGHT_WON]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[PU_FIGHT_WON]
GO

CREATE PROCEDURE dbo.PU_FIGHT_WON
	@VCL_ID		int,
	@VCL_FIGHT_WON_NU int
AS

update VCL_VEHICLE
set VCL_FIGHT_WON_NU = @VCL_FIGHT_WON_NU
where
	VCL_ID = @VCL_ID

GO

GRANT  EXECUTE  ON [dbo].PU_FIGHT_WON  TO [public]
GO

