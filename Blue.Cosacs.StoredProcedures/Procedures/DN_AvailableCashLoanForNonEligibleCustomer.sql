
IF  EXISTS (SELECT 1 
	FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DN_AvailableCashLoanForNonEligibleCustomer]') 
	AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DN_AvailableCashLoanForNonEligibleCustomer]

GO

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DN_AvailableCashLoanForNonEligibleCustomer]		
@CustomerId VARCHAR(20),
@ExistCashLoanAmt DECIMAL OUTPUT ,
@return int OUTPUT
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_AvailableCashLoanForNonEligibleCustomer
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DN_AvailableCashLoanForNonEligibleCustomer
-- Author       : ??
-- Date         : 28/7/2020
-- Version		: 002
-- Created for CR10.7
-- This procedure Calculate all the Exist Loan
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 28/7/2020 Rahul Sonawane 10.7 Feature CR - Calculate all the Exist Loan
-- ================================================			
				
AS
SET @return = 0               --initialise return code
BEGIN
	SET NOCOUNT ON;

		DECLARE @TotalPayments DECIMAL(10,3);

		SET @TotalPayments=	(	
								SELECT SUM(fn.transvalue) 
								FROM fintrans fn
								WHERE acctno IN ( SELECT acctno FROM Cashloan WHERE custid=@CustomerId )
								AND fn.transtypecode NOT IN ('DEL','GRT','ADD','CLD')
							)

		SELECT @ExistCashLoanAmt=(		
									SELECT 
										ISNULL(
											SUM(ac.agrmttotal) - (SUM(ag.servicechg) + (@TotalPayments * (-1)) * ((SUM(ac.agrmttotal) - SUM(ag.servicechg)) / SUM(ac.agrmttotal)))
											,0)
									FROM CashLoan cl 
									INNER JOIN custacct ca ON cl.custid=ca.custid AND cl.acctno=ca.acctno
									LEFT OUTER JOIN acct ac ON cl.acctno=ac.acctno
									LEFT OUTER JOIN agreement ag ON cl.acctno=ag.acctno
									WHERE cl.custid=@CustomerId AND cl.LoanStatus='D' AND ac.currstatus!='S'
								 );
END
IF (@@error != 0)
BEGIN
    SET @return = @@error
END
   