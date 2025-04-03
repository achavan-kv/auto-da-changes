SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_EODOptionGetStatusSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_EODOptionGetStatusSP
END
GO


CREATE PROCEDURE DN_EODOptionGetStatusSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_EODOptionGetStatusSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Gets the status for a particular option
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
	@configname  VARCHAR(12),
	@option      VARCHAR(12),
	@status		 CHAR(1) OUTPUT,
    @return      INT OUTPUT

AS  --DECLARE
    -- Local variables

BEGIN
    SET @return = 0

	SELECT	@status = ISNULL(status,'')
	FROM	EodConfigurationOption
	WHERE	ConfigurationName = @configname
	AND		OptionCode = @option
  
    SET @return = @@ERROR
    RETURN @return
END

GO
GRANT EXECUTE ON DN_EODOptionGetStatusSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO