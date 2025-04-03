SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EODIsReRunSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODIsReRunSP]
GO

CREATE PROCEDURE DN_EODIsReRunSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_EODIsReRunSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Gets the re-run status for a configuration
-- Author       : P Njie
-- Date         : 21 Jun 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
	@configname  VARCHAR(12),
	@isrerun	 INT OUT,
    @return      INT OUTPUT

AS  --DECLARE
    -- Local variables

BEGIN
    SET @return = 0

	SELECT	@isrerun = COUNT(*)
	FROM	EodConfiguration
	WHERE	ConfigurationName = @configname
	AND		Status = 'RERUN'
  
    SET @return = @@ERROR
    RETURN @return
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

