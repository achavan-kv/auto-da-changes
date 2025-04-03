SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EODRenameAdHocScriptsSP]')
                                          and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODRenameAdHocScriptsSP]
GO


CREATE PROCEDURE DN_EODRenameAdHocScriptsSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_EODRunAdHocScriptsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Rename one off Ad-Hoc sripts as part of the EOD run
-- Author       : P Njie
-- Date         : 24 May 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @return  INT OUTPUT

AS 
	DECLARE @cmdstring VARCHAR(300)
	DECLARE @scriptname VARCHAR(150)
	DECLARE @fullscriptname VARCHAR(60)

    SET @return = 0

	CREATE TABLE #files(scriptname varchar(150),
						fullscriptname varchar(150) not null default '',
						processed int not null default 0)
			
	SET @cmdstring = 'dir d:\cosprog\eod\before\once\*.sql /b'
	INSERT #files (scriptname)
	EXEC master.dbo.xp_cmdshell @cmdstring
	UPDATE	#files
	SET		fullscriptname = 'd:\cosprog\eod\before\once\' + scriptname,
			processed = 1
	WHERE	processed = 0
	AND		scriptname IS NOT NULL

	SET @cmdstring = 'dir d:\cosprog\eod\after\once\*.sql /b'
	INSERT #files (scriptname)
	EXEC master.dbo.xp_cmdshell @cmdstring
	UPDATE	#files
	SET		fullscriptname = 'd:\cosprog\eod\after\once\' + scriptname,
			processed = 1
	WHERE	processed = 0
	AND		scriptname IS NOT NULL
			
	DECLARE rename_cursor CURSOR FOR 
	SELECT scriptname, fullscriptname 
	FROM #files 
	WHERE scriptname IS NOT NULL
	
	OPEN rename_cursor
	
	FETCH NEXT FROM rename_cursor 
	INTO @scriptname, @fullscriptname
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @cmdstring = 'call rename ' + @fullscriptname + ' ' + left(@scriptname, len(@scriptname) -4 ) + '.old'
		EXEC master.dbo.xp_cmdshell @cmdstring

		FETCH NEXT FROM rename_cursor 
		INTO @scriptname, @fullscriptname
	END
	CLOSE rename_cursor
	DEALLOCATE rename_cursor
    
	SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO