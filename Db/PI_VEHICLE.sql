USE [EACC]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PI_VEHICLE]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[PI_VEHICLE]
GO

CREATE PROCEDURE dbo.PI_VEHICLE
	@VCL_ID								INT,
	@VCL_DOMAIN_CH						nvarchar(2000),
	@VCL_NAME_CH						nvarchar(2000),
	@VCL_SQUARE_NU						INT,
	@TEA_ID								INT,
	@DEL_BL								BIT,
	@TVP_MODULES						TVP_INT_INT_CHAR readonly,
	@VCL_TURNS_TO_MISS_NU				INT,
	@VCL_FIGHT_WON_NU					INT,
	@OUTPUT_VCL_ID						INT output
AS

if @VCL_ID is null
begin
	insert into VCL_VEHICLE(TEA_ID, VCL_NAME_CH, VCL_SQUARE_NU, VCL_DOMAIN_CH, DEL_BL, VCL_TURNS_TO_MISS_NU, VCL_FIGHT_WON_NU)
	values (@TEA_ID, @VCL_NAME_CH, @VCL_SQUARE_NU, @VCL_DOMAIN_CH, @DEL_BL, @VCL_TURNS_TO_MISS_NU,@VCL_FIGHT_WON_NU)
	
	set @OUTPUT_VCL_ID = SCOPE_IDENTITY()
end
else
begin
	update VCL_VEHICLE
	set
		TEA_ID 			= @TEA_ID,
		VCL_NAME_CH 	= @VCL_NAME_CH,
		VCL_SQUARE_NU 	= @VCL_SQUARE_NU,
		VCL_DOMAIN_CH 	= @VCL_DOMAIN_CH,
		DEL_BL 			= @DEL_BL,
		VCL_TURNS_TO_MISS_NU   = @VCL_TURNS_TO_MISS_NU,
		VCL_FIGHT_WON_NU   = @VCL_FIGHT_WON_NU
	where VCL_ID = @VCL_ID
	
	set @OUTPUT_VCL_ID = @VCL_ID
	
	-- delete associations
	delete MOD_MODULE
	where VCL_ID = @OUTPUT_VCL_ID
		
end

insert into MOD_MODULE(MOD_NAME_CH, MOD_TYPE_NU, VCL_ID)
select tvp.LABEL1, tvp.ID2,@OUTPUT_VCL_ID
from @TVP_MODULES tvp

GO

GRANT  EXECUTE  ON [dbo].[PI_VEHICLE]  TO [public]
GO

