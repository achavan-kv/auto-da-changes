SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_EODConfigurationSaveSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_EODConfigurationSaveSP
END
GO


CREATE PROCEDURE DN_EODConfigurationSaveSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_EODConfigurationOptionsSaveSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save a new EOD configuration
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
    @FACT2000date  datetime,
    @startdate	   datetime,
    @frequency	   int,	 	
    @return        INT OUTPUT

AS  --DECLARE
    -- Local variables

BEGIN

    SET @return = 0
    
	UPDATE	EodConfiguration
	SET		IsActive = 0

    INSERT INTO EodConfiguration(ConfigurationName, IsActive, fact2000date, 
									startdate, frequency)
	VALUES (@configname, 1, @FACT2000date, @startdate, @frequency)
    
    SET @return = @@ERROR
    RETURN @return
END

GO
GRANT EXECUTE ON DN_EODConfigurationSaveSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO


