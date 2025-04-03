SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_EODConfigurationOptionsSaveSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_EODConfigurationOptionsSaveSP
END
GO

CREATE PROCEDURE DN_EODConfigurationOptionsSaveSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_EODConfigurationOptionsSaveSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Add new options to an EOD configuration
-- Author       : P Njie
-- Date         : 3 Apr 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 05/04/11 jec  Add ReRunNo to Configuration option
--------------------------------------------------------------------------------

    -- Parameters
	@configname    VARCHAR(12),
	@optioncode    VARCHAR(16),
	@user          INT,
	@sortorder	   SMALLINT,
	@reRunNo		SMALLINT,		-- jec 05/04/11
    @return        INT OUTPUT

AS  --DECLARE
    -- Local variables

BEGIN

    SET @return = 0

	Declare @canReRun char(1) = null
    
	if @optioncode = 'STStatementsAUTO'
	begin
		set @canReRun = 'A'
		set @optioncode  = 'STStatements'
	end

    INSERT INTO EodConfigurationOption(ConfigurationName, OptionCode, CurrentStep,
        							   TotalSteps, LastEditBy, SortOrder, Status, CanReRun, ReRunNo)
    VALUES (@configname, @optioncode, 0,
        	1, @user, @sortorder, '', @canReRun, @reRunNo)		
    
    SET @return = @@ERROR
    RETURN @return
END

GO
GRANT EXECUTE ON DN_EODConfigurationOptionsSaveSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End
