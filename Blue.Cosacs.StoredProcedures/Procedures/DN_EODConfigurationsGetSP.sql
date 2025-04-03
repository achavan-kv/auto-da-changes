SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_EODConfigurationsGetSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_EODConfigurationsGetSP
END
GO


CREATE PROCEDURE DN_EODConfigurationsGetSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_EODConfigurationOptionsGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load all existing configurations
-- Author       : P Njie
-- Date         : 29 Mar 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @return                    INT OUTPUT

AS  --DECLARE
    -- Local variables

BEGIN

    SET @return = 0
    
    SELECT ConfigurationName, IsActive
    FROM   EodConfiguration
    ORDER BY ConfigurationName
    
    SET @return = @@ERROR
    RETURN @return
END

GO
GRANT EXECUTE ON DN_EODConfigurationsGetSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO


