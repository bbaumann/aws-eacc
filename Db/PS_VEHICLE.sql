USE [EACC]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PS_VEHICLE]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[PS_VEHICLE]
GO

CREATE PROCEDURE dbo.PS_VEHICLE
	@VCL_ID		int
AS

select
	VCL_ID,
	v.TEA_ID,
	VCL_NAME_CH,
	VCL_SQUARE_NU,
	VCL_DOMAIN_CH,
	DEL_BL,
	VCL_TURNS_TO_MISS_NU,
	t.TEA_SECRET_CH
from
	VCL_VEHICLE v
	inner join TEA_TEAM t on t.TEA_ID = v.TEA_ID
where
	VCL_ID = @VCL_ID
	
select 	m.MOD_ID,
		m.MOD_NAME_CH,
		m.MOD_TYPE_NU,
		m.VCL_ID
from
	MOD_MODULE m
where 
	m.VCL_ID = @VCL_ID


GO

GRANT  EXECUTE  ON [dbo].[PS_VEHICLE]  TO [public]
GO

