SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_EODConfigurationDeleteSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_EODConfigurationDeleteSP
END
GO


CREATE PROCEDURE DN_EODConfigurationDeleteSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_EODConfigurationDeleteSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Delete an EOD configuration
-- Author       : P Njie
-- Date         : 3 Apr 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
	@configname    VARCHAR(12),
    @return        INT OUTPUT

AS  --DECLARE
    -- Local variables

BEGIN

    SET @return = 0
    
    DELETE 
	FROM 	EodConfiguration
	WHERE 	ConfigurationName = @configname
    
    SET @return = @@ERROR
    RETURN @return
END

GO
GRANT EXECUTE ON DN_EODConfigurationDeleteSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO


