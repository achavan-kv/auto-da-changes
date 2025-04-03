SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_EODConfigurationOptionsGetSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_EODConfigurationOptionsGetSP
END
GO


CREATE PROCEDURE DN_EODConfigurationOptionsGetSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_EODConfigurationOptionsGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load the list of options for an EOD configuration
-- Author       : D Richardson
-- Date         : 1 Mar 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @configurationName         VARCHAR(12),
    @return                    INT OUTPUT

AS  --DECLARE
    -- Local variables

BEGIN

    SET @return = 0
    
    SELECT OptionCode, SortOrder, Status
    FROM   EODConfigurationOption
    WHERE  ConfigurationName = @configurationName
    ORDER BY SortOrder
    
    SET @return = @@ERROR
    RETURN @return
END

GO
GRANT EXECUTE ON DN_EODConfigurationOptionsGetSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO


