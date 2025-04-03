
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_SummaryRptDataSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_SummaryRptDataSP
END
GO

CREATE PROCEDURE DN_SummaryRptDataSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_SummaryRptDataSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Create Summary Report Data
-- Author       : Rupal Desai
-- Date         : 02 May 2006
--
-- Change Control
-- --------------
-- Date      	By  Description
-- ----      	--  -----------
--

-- Store procedure to set isfullrun in summaryrun table if full refresh is selected

--declare
   @isfullrun			smallint,
   @return             int OUTPUT
AS

BEGIN

	SET 	@return = 0			--initialise return code

	UPDATE summaryrun SET isfullrun = @isfullrun
	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
	
END
GO

GRANT EXECUTE ON DN_SummaryRptDataSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
