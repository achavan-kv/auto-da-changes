SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EODSetReRunStatusSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODSetReRunStatusSP]
GO

CREATE PROCEDURE DN_EODSetReRunStatusSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_EODOptionGetStatusSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Sets the re-run status for a configuration
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
	@status		 VARCHAR(6),
    @return      INT OUTPUT

AS  --DECLARE
    -- Local variables

BEGIN
    SET @return = 0

	UPDATE	EodConfiguration
	SET		status = @status
	WHERE	ConfigurationName = @configname
  
    SET @return = @@ERROR
    RETURN @return
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

