SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountCancelSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountCancelSP]
GO

CREATE PROCEDURE 	dbo.DN_AccountCancelSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_AccountCancelSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Account Cancel
-- Author       : ??
-- Date         : ??
--
-- This procedure will Cancel an account
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 13/10/11  jec CR1232 set cancelled status on CashLoan table
-- 30/11/11  jec #8768 Unblock Cashloan if Blocked
-- ================================================
	-- Add the parameters for the stored procedure here
			@acctno varchar(12),
			@custid varchar(20),	
			@user int,
			@datecancelled DateTime,
			@code varchar(4),
			@notes varchar(300),
			@cancelled smallint OUT,
			@return int OUTPUT

AS
 SET NOCOUNT ON
	SET 	@return = 0			--initialise return code
	SET	@cancelled = 0

	DECLARE	@currstatus char(1)
	DECLARE	@agreementtotal money
	DECLARE	@outsbal money
	
	SELECT	@currstatus  = currstatus,
			@agreementtotal = agrmttotal,
			@outsbal = outstbal
	FROM		acct
	WHERE	acctno = @acctno
	
	IF(abs(@agreementtotal) < 0.01)
	BEGIN
		SELECT	@agreementtotal = ISNUll(agrmttotal, 0)
		FROM	agreementbfcollection
		WHERE	acctno = @acctno
	END

	--IF(@outsbal <= 0)
	IF(abs(@outsbal) < 0.01)
	BEGIN
		UPDATE	acct
		SET		currstatus = 'S',
				agrmttotal = 0,
				lastupdatedby = @user
		WHERE	acctno = @acctno
		
		UPDATE	agreement
		SET		agrmttotal = 0
		WHERE	acctno = @acctno
		
		-- Remove delauthorise record  - in orders for delivery
		DELETE FROM delauthorise WHERE acctno = @acctno

		/* not clearing out instalment or service charge on agreement as would cause problems if account was uncancelled
      UPDATE	instalplan
		SET		instalamount = 0,
				fininstalamt = 0,
				instaltot = 0
		WHERE	acctno = @acctno

		UPDATE	agreement
		SET		servicechg = 0
		WHERE	acctno = @acctno*/

		--put a record in the cancellation table with a code of 'B' == Unable to meet credit
		INSERT	
		INTO	cancellation
			(origbr, acctno, agrmtno, datecancel,
			empeenocanc, code, agrmttotal, notes)
		VALUES
			(0, @acctno, 1, @datecancelled, @user, @code, @agreementtotal, @notes)
		
		-- Update Cashloan status  - 	
		UPDATE CashLoan
		set LoanStatus='C'
		where AcctNo=@acctno
		
		-- if Cash Loan account
		UPDATE	customer
		SET		CashLoanBlocked = case when CashLoanBlocked='B' then ' ' else CashLoanBlocked end		-- #8768
		from customer c INNER JOIN CashLoan cl on c.custid=cl.Custid and cl.AcctNo=@acctno				
		--WHERE	c.custid = @custid

		SET	@cancelled = 1	
	END
	ELSE
	BEGIN
		RAISERROR('Outstanding balance on account %s must be refunded before account can be cancelled', 16, 1, @acctno)
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End