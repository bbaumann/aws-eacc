USE [EACC]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PS_ALL_VEHICLES]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[PS_ALL_VEHICLES]
GO

CREATE PROCEDURE dbo.PS_ALL_VEHICLES
	@DEL_BL	bit = NULL
AS

select
	vcl.VCL_ID,
	vcl.TEA_ID,
	vcl.VCL_NAME_CH,
	vcl.VCL_SQUARE_NU,
	vcl.VCL_DOMAIN_CH,
	vcl.DEL_BL,
	m.MOD_ID,
	m.MOD_NAME_CH,
	m.MOD_TYPE_NU,
	vcl.VCL_TURNS_TO_MISS_NU,
	vcl.VCL_FIGHT_WON_NU,
	t.TEA_SECRET_CH
from
	VCL_VEHICLE vcl
	inner join MOD_MODULE m on m.VCL_ID = vcl.VCL_ID
	inner join TEA_TEAM t on t.TEA_ID = vcl.TEA_ID
where
	@DEL_BL IS NULL OR vcl.DEL_BL = @DEL_BL
order by vcl.VCL_ID
	


GO

GRANT  EXECUTE  ON [dbo].[PS_ALL_VEHICLES]  TO [public]
GO

