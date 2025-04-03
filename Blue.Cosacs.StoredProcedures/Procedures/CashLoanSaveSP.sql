SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].CashLoanSaveSP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE CashLoanSaveSP
END
GO

CREATE PROCEDURE dbo.CashLoanSaveSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : CashLoanSaveSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save Cash Loan details
-- Author       : John Croft
-- Date         : 30 September 2011
--
-- This procedure will save the Cash Loan details
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 06/10/11  IP  #3913 - CR1232 - Save TermsType
-- 03/11/11 jec #8491 - add dateprinted
-- 24/02/12  IP  #9598 - UAT 87 - Save Referral Reasons to the CashLoan table for a referred Cash Loan
-- 21/06/19 KD	Added isamortized and isamortizedoutstandingbalance update flag to update in Acct table
-- ================================================
	-- Add the parameters for the stored procedure here
	@Custid		VARCHAR(20),
	@AcctNo		CHAR(12),
	@LoanAmount	MONEY,
	@Term		INT,
	@LoanStatus	CHAR(1),			-- RI
	@TermsType VARCHAR(2),
	@EmpeenoAccept INT = null,
	@EmpeenoDisburse INT = null,
	@DatePrinted DATETIME = null,			-- #8491
	@ReferralReasons VARCHAR(4000),			-- IP - 24/02/12 - #9598 - UAT 87
	@CashLoanPurpose VARCHAR(25) = null,	--#19337 - CR18568
    @AdminChargeWaived BIT,
    @AdminCharge MONEY,
    @EmpeenoAdminChargeWaived INT = NULL,
    @EmpeenoLoanAmountChanged INT = NULL,
    @Bank VARCHAR(6) = NULL,
    @BankAccountType CHAR(1) = NULL,
    @BankBranch VARCHAR(20) = NULL,
    @BankAcctNo VARCHAR(20) = NULL,
    @Notes VARCHAR(200) = NULL,
    @BankReferenceNo VARCHAR(10) = NULL,
    @BankAccountName VARCHAR(30) = NULL,
	@return INT output

As
	set @return=0
	
	UPDATE CashLoan
		set LoanAmount=@LoanAmount,LoanStatus=@LoanStatus,Term=@Term, TermsType=@TermsType, EmpeenoAccept=@EmpeenoAccept, EmpeenoDisburse=@EmpeenoDisburse,
		Dateprinted=@DatePrinted,			-- #8491
		ReferralReasons = @ReferralReasons,	-- IP - 24/02/12 - #9598 - UAT 87
		CashLoanPurpose = @CashLoanPurpose,	--#19337 - CR18568
        AdminChargeWaived = @AdminChargeWaived,
        AdminCharge = @AdminCharge,
        EmpeenoAdminChargeWaived = @EmpeenoAdminChargeWaived,
        EmpeenoLoanAmountChanged = @EmpeenoLoanAmountChanged,
        Bank = @Bank,
        BankAccountType = @BankAccountType,
        BankBranch = @BankBranch,
        BankAcctNo = @BankAcctNo,
        Notes = @Notes,
        BankReferenceNo = @BankReferenceNo,
        BankAccountName = @BankAccountName
	Where Custid=@Custid and AcctNo=@AcctNo
	
	-- Insert if Loan does not exist
	if @@ROWCOUNT=0
	BEGIN
		Insert into CashLoan (Custid, AcctNo, LoanAmount, Term, LoanStatus, TermsType, EmpeenoAccept, EmpeenoDisburse,DatePrinted,ReferralReasons, 
            CashLoanPurpose, AdminChargeWaived, AdminCharge, EmpeenoAdminChargeWaived, EmpeenoLoanAmountChanged, Bank, BankAccountType, BankBranch, BankAcctNo, Notes, BankReferenceNo, BankAccountName)	-- IP - 24/02/12 - #9598 - UAT 87	-- #8491
		values (@Custid,@AcctNo,@LoanAmount,@Term,@LoanStatus,@TermsType,@EmpeenoAccept,@EmpeenoDisburse,@DatePrinted, @ReferralReasons,
            @CashLoanPurpose, @AdminChargeWaived, @AdminCharge, @EmpeenoAdminChargeWaived, @EmpeenoLoanAmountChanged, @Bank, @BankAccountType, @BankBranch, @BankAcctNo, @Notes, @BankReferenceNo, @BankAccountName) --#19337 - CR18568-- IP - 24/02/12 - #9598 - UAT 87			-- #8491
	
		IF exists(Select value from countrymaintenance where codename='CL_Amortized' and Value = 'True')
		BEGIN
			UPDATE ACCT SET isAmortized=1 WHERE ACCTNO = @AcctNo
		END
		IF exists(Select value from countrymaintenance where codename='CL_NewOutstandingCalculation' and Value = 'True')
		BEGIN
		UPDATE ACCT SET IsAmortizedOutStandingBal=1 WHERE ACCTNO = @AcctNo
		END

	END
	
	----If customer was referred from the Cash Loan Qualification then refer the proposal.
	--IF(@LoanStatus = 'R')
	--BEGIN
	
		
	--	UPDATE Proposal 
	--		SET	propresult = 'R'
	--		WHERE acctno = @AcctNo
			
	--	IF NOT EXISTS(select * from propsalflag where acctno = @AcctNo and checktype = 'R' and datecleared is null)
	--	BEGIN
		
	--		declare @dateProp datetime
	--		select @dateProp = dateprop from proposal where acctno = @AcctNo
		
	--		INSERT INTO ProposalFlag (origbr, custid, dateprop, checktype, datecleared, empeenopflg, unclearedby, acctno)
	--		SELECT 0,@Custid, @dateProp, 'R', NULL, @EmpeenoAccept, NULL, @AcctNo
	--	END 
	--END
	
	set @return=@@ERROR
	
go	
-- end end end end end end end end end end end end end end end end end end end end end end end 