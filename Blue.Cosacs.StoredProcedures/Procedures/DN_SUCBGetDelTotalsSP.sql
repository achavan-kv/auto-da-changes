SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SUCBGetDelTotalsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SUCBGetDelTotalsSP]
GO
/* Gets the delivery totals for summary update control report
AA 14 Feb 05 UAT Issue - 1. Totals were not matching as were including other transaction types in the @deltotal amount.
  2. Include a range of FACT 2000 deliveries as Mauritius have been running the Cosacs to FACT export daily, but the summary only weekly*/

CREATE PROCEDURE 	dbo.DN_SUCBGetDelTotalsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_SUCBGetDelTotalsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
--
--
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 20/02/12  IP #9423 - CR8262 - Now checking for transactions between UPDSMRY runs
-- 05/10/12  jec #10138 - LW75030 - SUCR - Cash Loan 
-- ================================================
			@runno int,
			@deltotal money OUTPUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	--DECLARE	@maxcosFactRunno int,	@mincosFactRunno int
	
	DECLARE	@minUpdsmryRunDate datetime, @maxUpdsmryRunDate datetime	

	SELECT	@maxUpdsmryRunDate = max(i.datefinish)
	FROM	interfacecontrol i
	WHERE	i.interface = 'UPDSMRY'
	AND		i.runno = @runno
	
	SELECT @minUpdsmryRunDate = i.datefinish
	FROM   interfacecontrol i
	WHERE	i.interface = 'UPDSMRY'
	AND		i.runno = @runno - 1

	--SELECT	@maxcosFactRunno = MAX(i.runno)
	--FROM	interfacecontrol i
	--WHERE	i.interface = 'COS FACT'
	--AND	i.datestart <= 
	--		(SELECT	i2.datestart
	--		 FROM	interfacecontrol i2
	--		 WHERE	i2.runno = @runno
	--		 AND 	i2.interface = 'UPDSMRY')


	--SELECT @mincosFactRunno = min(i.runno)
	--FROM	interfacecontrol i
	--WHERE	i.interface = 'COS FACT'
	--AND	i.datestart >= 
	--		(SELECT	i2.datestart
	--		 FROM	interfacecontrol i2
	--		 WHERE	i2.runno = @runno -1
			 --AND 	i2.interface = 'UPDSMRY')

	--if @mincosFactRunno>@maxcosFactRunno
	--begin
	--	set @maxcosFactRunno=@mincosFactRunno
	--end

	SELECT	@deltotal = SUM(transvalue)
	FROM	fintrans
	WHERE	runno = @runno 
   and Transtypecode In ('DEL', 'GRT','ADD','REB','RDL','REP','CLD')			-- #10138


--  if @mincosFactRunno = @maxcosFactRunno -- the same value I think equals may improve performance.
--  begin
--	SELECT	SUBSTRING(acctno,1,3) as branchno, 
--		SUM(transvalue) as transvalue,
--		runno,  convert(varchar,datetrans,101) [Trans Date]
--	FROM	delivery 
--	WHERE	runno = @mincosFactRunno 
--	GROUP BY SUBSTRING(acctno,1,3), runno, convert(varchar,datetrans,101)
--order by branchno
--  end
--  else
--  begin
--	SELECT	SUBSTRING(acctno,1,3) as branchno, 
--		SUM(transvalue) as transvalue,
--		runno, convert(varchar,datetrans,101) [Trans Date]
--	FROM	delivery 
--	WHERE	runno between @mincosFactRunno and @maxcosFactRunno
--	GROUP BY SUBSTRING(acctno,1,3), runno, convert(varchar,datetrans,101)
--order by branchno
--  end

	--IP - 20/02/12 - #9423 - CR8262 - Replaces the above
	SELECT	SUBSTRING(acctno,1,3) as branchno, 
	SUM(transvalue) as transvalue,
	convert(varchar,datetrans,101) [Trans Date]
	FROM	delivery 
	WHERE	datetrans between @minUpdsmryRunDate and @maxUpdsmryRunDate
	GROUP BY SUBSTRING(acctno,1,3), convert(varchar,datetrans,101)
	order by branchno

  IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

