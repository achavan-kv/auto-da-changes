SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EODAdHocScriptsGetSP]')
                                          and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODAdHocScriptsGetSP]
GO


CREATE PROCEDURE DN_EODAdHocScriptsGetSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_EODAdHocScriptsGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Retrieve ad hoc scripts to be run as part of EOD job
-- Author       : P Njie
-- Date         : 7 April 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
	@type    CHAR(1), 		
    @return  INT OUTPUT

AS 
	DECLARE @cmdstring varchar(100)

BEGIN

    SET @return = 0

	CREATE TABLE #files(scriptname varchar(256))

	IF(@type = 'D')
	BEGIN
		SET @cmdstring = 'dir d:\cosprog\eod\before\daily\*.sql /b'
		INSERT #files
		EXEC master.dbo.xp_cmdshell @cmdstring
		
		SET @cmdstring = 'dir d:\cosprog\eod\after\daily\*.sql /b'
		INSERT #files
		EXEC master.dbo.xp_cmdshell @cmdstring
	END	
	ELSE
	BEGIN
		SET @cmdstring = 'dir d:\cosprog\eod\before\once\*.sql /b'
		INSERT #files
		EXEC master.dbo.xp_cmdshell @cmdstring

		SET @cmdstring = 'dir d:\cosprog\eod\after\once\*.sql /b'
		INSERT #files
		EXEC master.dbo.xp_cmdshell @cmdstring
	END	

	SELECT scriptname 
	FROM #files 
	WHERE scriptname != 'NULL'
	AND scriptname NOT LIKE 'File Not Found%'
    
	SET @return = @@error
    RETURN @return
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

