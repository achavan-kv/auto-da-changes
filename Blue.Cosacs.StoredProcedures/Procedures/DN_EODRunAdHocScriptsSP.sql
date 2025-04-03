SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EODRunAdHocScriptsSP]')
                                          and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODRunAdHocScriptsSP]
GO

CREATE PROCEDURE DN_EODRunAdHocScriptsSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_EODRunAdHocScriptsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Run Ad-Hoc sripts as part of the EOD run
-- Author       : P Njie
-- Date         : 24 May 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/11/10  jec CR1030 Check for error in execution of script
--------------------------------------------------------------------------------

    -- Parameters
	@before  BIT, 		
    @return  INT OUTPUT

AS 
	DECLARE @cmdstring VARCHAR(300)
	DECLARE @scriptname VARCHAR(150)
	DECLARE @logname VARCHAR(150)
	DECLARE @ret INT
	DECLARE @runno int

    SET @return = 0

	CREATE TABLE #files(scriptname varchar(150),
						logname varchar(150) not null default '',
						processed int not null default 0)
			
	IF(@before = 1)
	BEGIN
		SET @cmdstring = 'dir d:\cosprog\eod\before\once\*.sql /b'
		INSERT #files (scriptname)
		EXEC master.dbo.xp_cmdshell @cmdstring
		UPDATE	#files
		SET		scriptname = 'd:\cosprog\eod\before\once\' + scriptname,
				logname = 'd:\cosprog\eod\before\once\' + left(scriptname,len(scriptname) - 4) + '.log',
				processed = 1
		WHERE	processed = 0
		AND		scriptname IS NOT NULL
		
		SET @cmdstring = 'dir d:\cosprog\eod\before\daily\*.sql /b'
		INSERT #files (scriptname)
		EXEC master.dbo.xp_cmdshell @cmdstring
		UPDATE	#files
		SET		scriptname = 'd:\cosprog\eod\before\daily\' + scriptname,
				logname = 'd:\cosprog\eod\before\daily\' + left(scriptname,len(scriptname) - 4) + '.log',
				processed = 1
		WHERE	processed = 0
		AND		scriptname IS NOT NULL
	END
	ELSE
	BEGIN
		SET @cmdstring = 'dir d:\cosprog\eod\after\once\*.sql /b'
		INSERT #files (scriptname)
		EXEC master.dbo.xp_cmdshell @cmdstring
		UPDATE	#files
		SET		scriptname = 'd:\cosprog\eod\after\once\' + scriptname,
				logname = 'd:\cosprog\eod\after\once\' + left(scriptname,len(scriptname) - 4) + '.log',
				processed = 1
		WHERE	processed = 0
		AND		scriptname IS NOT NULL
		
		SET @cmdstring = 'dir d:\cosprog\eod\after\daily\*.sql /b'
		INSERT #files (scriptname)
		EXEC master.dbo.xp_cmdshell @cmdstring
		UPDATE	#files
		SET		scriptname = 'd:\cosprog\eod\after\daily\' + scriptname,
				logname = 'd:\cosprog\eod\after\daily\' + left(scriptname,len(scriptname) - 4) + '.log',
				processed = 1
		WHERE	processed = 0
		AND		scriptname IS NOT NULL
	END
	-- table to hold output from xp_cmdshell
	if not exists (select * from dbo.sysobjects 
	where id = object_id('dbo.XPCmdShellOutput') and OBJECTPROPERTY(id, 'IsUserTable') = 1)
		create table XPCmdShellOutput (OutputLine varchar(1000))	
	else
		truncate table XPCmdShellOutput
		
	DECLARE scripts_cursor CURSOR FOR 
	SELECT scriptname, logname 
	FROM #files 
	WHERE scriptname IS NOT NULL
	
	OPEN scripts_cursor
	
	FETCH NEXT FROM scripts_cursor 
	INTO @scriptname, @logname
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @cmdstring = 'call osql -S ' + @@SERVERNAME + ' -d ' + db_name() + ' -E < ' + @scriptname --+ ' > ' + @logname --CR1030 jec
		insert into XPCmdShellOutput EXEC @ret=master.dbo.xp_cmdshell @cmdstring			--CR1030 jec

		FETCH NEXT FROM scripts_cursor 
		INTO @scriptname, @logname
	END
	CLOSE scripts_cursor
	DEALLOCATE scripts_cursor

    SET @return = @@ERROR
	-- check for errors
	select @ret=COUNT(*) from XPCmdShellOutput where Outputline like '%Msg%' --CR1030 jec
-- If error in execution of script		--CR1030 jec
	If @ret>@return
	Begin
		set @return=@ret		
		select @runno=MAX(runno) from interfacecontrol where interface='ADHOC' 
		
		insert into interfaceerror (interface,runno,errordate,severity,errortext)
		select 'ADHOC',@runno,GETDATE(),'E', Outputline 
		from XPCmdShellOutput where Outputline like '%Msg%'
		
		UPDATE interfaceerror 
		set errortext=SUBSTRING(errortext,CHARINDEX('msg',errortext,0),LEN(errortext))
		where interface='ADHOC' and runno=@runno
		
	End
			
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End
