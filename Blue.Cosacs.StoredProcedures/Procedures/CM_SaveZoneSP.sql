SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_SaveZoneSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_SaveZoneSP]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 03/02/2009
-- Description:	Saves zones
-- =============================================

CREATE PROCEDURE [dbo].[CM_SaveZoneSP] 
    @zone varchar(4),
	@zoneDescription varchar(64),
	@return INT OUTPUT

AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
	
	UPDATE CMZone
	SET 
	ZoneDescription = @zoneDescription
	WHERE 
	zone = @zone

	IF @@ROWCOUNT = 0

    INSERT INTO CMZone(
		zone ,
		ZoneDescription 
	) VALUES ( 
		@zone ,
		@ZoneDescription
	) 

	SET @return = @@ERROR

	RETURN @return

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
