SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_InterfaceLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_InterfaceLoadSP]
GO

CREATE PROCEDURE 	[dbo].[DN_InterfaceLoadSP]
-- ============================================================================================
-- Author:		Alex Ayscough
-- Create date: ?
-- Description:	This procedure updates the steps and processes the actions for the steps for
--				the accounts in a strategy.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 09/07/10 jec UAT1010 Cater for Letters generated in Collections
-- 01/04/11 jec RI Integration - Code.Additional column signifies ReRun allowed for EDC category
-- ============================================================================================

			@interface varchar(12),
			@interface2 VARCHAR(12),	--UAT1010
			@allruns bit,
			@return int OUTPUT

AS
	SET @return = 0		--initialise return code
	DECLARE @rows int

	-- lw69180 rdb 24/09/07 I have changed the result for W from 'PASS' to 'PASS(W)'
	--  this possibly may affect code elsewhere so if required change back to 'PASS'
	IF(@allruns = 1)
		SET @rows = 600
	ELSE
		SET @rows = 10	
	
	--UAT1010	
	select interface, runno, datestart, datefinish, CAST(result as VARCHAR(10)) as result,CAST('' as CHAR(1)) as CanReRun
	into #temp from dbo.interfacecontrol where interface='x'

    declare @statement sqltext
    set @statement = 
    'Insert into #temp ' +
	'SELECT TOP ' + convert(varchar,@rows) +
			 'interface, ' +
			 'runno, ' +
			 'datestart, ' +
			 'datefinish, ' +
			 'CASE ISNULL(result,'''') WHEN ''P'' THEN ''PASS'' WHEN ''F'' THEN ''FAIL'' WHEN ''W'' THEN ''PASS(W)'' ELSE '''' END AS result, ' +
			 'isnull(c.Additional,0) ' +
		     'FROM	interfacecontrol i inner join Code c on c.Code=i.Interface and c.category=''EDC'' ' +
		     'WHERE	interface = ''' + @interface + ''' ' 
	if @interface !='CHARGES' 
		SET @statement =@statement + 'ORDER BY 	runno desc '
	else -- letters and charges run number can loop round so doing by datestart instead of maximum runno
		SET @statement =@statement + 'ORDER BY 	datestart desc '
		
	-- second interface	--UAT1010
	SET @statement =@statement + 'Insert into #temp ' +
	'SELECT TOP ' + convert(varchar,@rows) +
			 ' interface, ' +
			 'runno, ' +
			 'datestart, ' +
			 'datefinish, ' +
			 'CASE ISNULL(result,'''') WHEN ''P'' THEN ''PASS'' WHEN ''F'' THEN ''FAIL'' WHEN ''W'' THEN ''PASS(W)'' ELSE '''' END AS result, ' +
			 'isnull(c.Additional,0)' +		     
		     'FROM	interfacecontrol i inner join Code c on c.Code=i.Interface and c.category=''EDC'' ' +		     		     
		     'WHERE	interface = ''' + @interface2 + ''' ' 
	if @interface2 !='CHARGES' 
		SET @statement =@statement + 'ORDER BY 	runno desc '
	else -- letters and charges run number can loop round so doing by datestart instead of maximum runno
		SET @statement =@statement + 'ORDER BY 	datestart desc '
        exec sp_executesql @statement
        
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
	
	Select * from #temp		--UAT1010
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End