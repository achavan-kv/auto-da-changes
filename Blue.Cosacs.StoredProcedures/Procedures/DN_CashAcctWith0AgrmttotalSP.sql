SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_CashAcctWith0AgrmttotalSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_CashAcctWith0AgrmttotalSP
END
GO

CREATE PROCEDURE  DN_CashAcctWith0AgrmttotalSP

--------------------------------------------------------------------------------
--
-- Project      : eCoSACS Strategic Thought Ltd.
-- File Name    : DN_CashAcctWith0AgrmttotalSP.PRC.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : To check for cash account zero agreement 67825
-- Author       : Rupal Desai
-- Date         : 14 March 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
	@acctno varchar(50),
	@isCashZeroAgrmt char(1) OUTPUT,
	@return int OUTPUT
 
AS

    	
 	SET  @return = 0   --initialise return code

	-- Check to make sure that the cash account meets following cretria in order to settle the account
	-- Is cash account (accttype = 'C')
	-- Has Zero agreement total (agrmttotal = 0)
	-- Is Fully delivered (datefullydelivered is set) removed this as datefullydelivered is not set up until EOD	
	-- Is not settled (Currstatus != 'S')
	-- Has items with greater then zero quantity  (quantity > 0 )
	-- Order value is not zero (ordval != 0)
	-- Is not cancelled (Has no entry in cancellation table)
	-- Agreement total is same as sum of delivery transactions
	-- All the items in lineitem are delivered (has entry in delivery table)

	SELECT	@isCashZeroAgrmt = 'Y'
--	SELECT	a.acctno, a.accttype, a.currstatus, a.agrmttotal, a.outstbal, a.arrears, a.dateacctopen
	FROM 	acct a, agreement ag
	WHERE	a.acctno = ag.acctno
	AND	a.accttype = 'C'
	AND	a.agrmttotal = 0
	AND	a.currstatus != 'S'
	AND	a.agrmttotal = ag.agrmttotal
--	AND	(ag.datefullydelivered IS NOT NULL OR ag.datefullydelivered != '01-Jan-1900')
	AND	NOT EXISTS (SELECT * FROM cancellation c WHERE a.acctno = c.acctno)
	AND	a.agrmttotal = ISNULL((SELECT SUM(d.transvalue) FROM delivery d
				       WHERE  d.acctno = a.acctno),0)
	AND	EXISTS (SELECT * FROM Lineitem l , delivery d1 WHERE l.acctno = a.acctno AND l.quantity > 0
			AND	l.acctno = d1.acctno AND l.itemno = d1.itemno AND l.stocklocn = d1.stocklocn
			AND	l.ordval != 0)
	AND	a.acctno = @acctno
  
	-- Set iscashzeroagrmt to N where does not meet the above conditions
	IF @isCashZeroAgrmt IS NULL
	   SET @isCashZeroAgrmt = 'N'

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
