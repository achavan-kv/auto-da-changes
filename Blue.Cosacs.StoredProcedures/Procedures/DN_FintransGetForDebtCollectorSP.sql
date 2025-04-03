SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DN_FintransGetForDebtCollectorSP]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetForDebtCollectorSP]
GO

CREATE PROCEDURE dbo.DN_FintransGetForDebtCollectorSP
-- **********************************************************************
-- Title: dbo.DN_FintransGetForDebtCollectorSP.PRC
-- Developer: ??
-- Date: ??

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/04/11  IP  #3387 - LW73356 - INST code no longer used, therefore select the last COM
--				 to be used to display the Instructions on the Debt Collectors Action Sheet.
-- **********************************************************************
			@acctno char(12), 
			@privilege int OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	DECLARE	@custid varchar(20)

	CREATE TABLE #accts
	(
		acctno varchar(12), charges money, bdwbalance money, bdwcharges money, 
		arrearslevel money, arrears money, arrearsexcharges money, instalamount money, datedel datetime,
		notes varchar(700)
	)

	SELECT	@custid = custid
	FROM	custacct
	WHERE	acctno = @acctno
	AND		hldorjnt = 'H'

	INSERT INTO #accts(acctno, charges, bdwbalance, bdwcharges, arrearslevel, 
			   arrears, arrearsexcharges, instalamount, datedel, notes)
	SELECT	@acctno, 0,0, 0, 0, 0, 0, 0, null, ''

	UPDATE	#accts
	SET	arrears = a.arrears,
      arrearsexcharges = a.arrears,
		bdwbalance = a.bdwbalance,
		bdwcharges = a.bdwcharges
	FROM	acct a
	WHERE	a.acctno = #accts.acctno

	UPDATE	#accts
	SET	datedel = a.datedel
	FROM	agreement a
	WHERE	a.acctno = #accts.acctno

	UPDATE	#accts
	SET	instalamount = ISNULL(i.instalamount, 0)
	FROM	instalplan i
	WHERE	i.acctno = #accts.acctno

	UPDATE	#accts
	SET    	charges = ISNULL((SELECT SUM(f.TransValue)
                           FROM   FinTrans f
                           WHERE  f.AcctNo = #accts.acctno
                           AND    f.TransTypeCode IN ('INT','ADM')),0)

	UPDATE	#accts set arrearsexcharges = arrears - charges

	UPDATE	#accts
	SET	arrearslevel = ISNULL(arrearsexcharges/instalamount, 0)
	WHERE	instalamount > 0
	
	DECLARE @notes TABLE (acctno CHAR(12) PRIMARY KEY, dateadded DATETIME NOT NULL )
	-- update latest instructions
	INSERT INTO @notes (		acctno,		dateadded)
	SELECT a.acctno,MAX( b.dateadded) 
	FROM #accts a, bailaction b 
	WHERE  a.acctno = b.acctno AND b.code = 'COM' --IP - 18/04/11 - #3387 - LW73356 - Changed from INST to COM
	GROUP BY a.acctno
	UPDATE	#accts
	SET		notes = ISNULL(b1.notes, '')
	FROM	bailaction b1, @notes n 
	WHERE	b1.acctno = #accts.acctno
	AND n.acctno = b1.acctno AND n.dateadded = b1.dateadded
	AND		b1.code = 'COM' --IP - 18/04/11 - #3387 - LW73356 - Changed from INST to COM
	
	SELECT	@privilege = ISNULL(COUNT(*), 0)
	FROM	custcatcode
	WHERE	custid = @custid
	AND	code IN('CLAC', 'PCHP', 'PCRF', 'PCC', 'TIR1', 'TIR2')

	SELECT	acctno,
		charges,
		bdwbalance,
		bdwcharges,
		arrearslevel,
		datedel,
		notes
	FROM	#accts


GO 
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
