USE [EACC]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PU_TURNS_TO_MISS]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[PU_TURNS_TO_MISS]
GO

CREATE PROCEDURE dbo.PU_TURNS_TO_MISS
	@VCL_ID		int,
	@VCL_TURNS_TO_MISS_NU int
AS

update VCL_VEHICLE
set VCL_TURNS_TO_MISS_NU = @VCL_TURNS_TO_MISS_NU
where
	VCL_ID = @VCL_ID
GO

GRANT  EXECUTE  ON [dbo].[PU_TURNS_TO_MISS]  TO [public]
GO

