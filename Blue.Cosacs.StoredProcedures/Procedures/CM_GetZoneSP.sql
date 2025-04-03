SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_GetZoneSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_GetZoneSP]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 10/02/2009
-- Description:	Selects all zones
-- =============================================
CREATE PROCEDURE [dbo].[CM_GetZoneSP] 
	@return int output
AS
BEGIN
    SET @return = 0    --initialise return code

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT zone, zoneDescription, (zone + ' ' + zoneDescription) as concatDesc FROM cmzone 

    IF (@@error <> 0)
    BEGIN
        SET @return = @@error
    END

END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
