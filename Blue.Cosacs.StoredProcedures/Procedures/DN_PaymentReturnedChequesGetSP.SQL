/****** Object:  StoredProcedure [dbo].[DN_PaymentReturnedChequesGetSP]    ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_PaymentReturnedChequesGetSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_PaymentReturnedChequesGetSP]
GO

-- =============================================
-- Author:		Peter Chong
-- Create date: 26-Sep-2006
-- Description:	Returns a list of returned cheques for a customer if they have a greater number of returned 
--				cheques than is a allowed in the defined period.  Applies to payments when paying by cheque on 
--				a cash or HP account.  [CR 543]
-- =============================================
CREATE Procedure [dbo].[DN_PaymentReturnedChequesGetSP]
(
	@custid varchar(20),
	@returnchequeperiod   int	= 0	,		--  How many months back to search for returned cheques
	@returnchequeallowedcount int	,		--  How many returned cheques are allowed
	@authorisationrequired bit = 0 OUT,			--  Set to 1 if greater than the allowable amount of returned cheques 
	@return	int = 0 OUT
) 
AS 
SET NOCOUNT ON

-- Functionality is disabled if returned cheque period is 0
IF @returnchequeperiod = 0 
	RETURN

DECLARE @ReturnChequeMinDate		datetime, -- How far back to search for returned cheques
	    @ReturnedChequeCount		int		  -- How many returned cheques this customer has

SET @ReturnChequeMinDate = DateAdd(m, @returnchequeperiod * -1, GetDate())

-- Get a count of returned cheques for this customer
SELECT @ReturnedChequeCount = COUNT(*) 
FROM  FinTrans F INNER JOIN 
	CustAcct C ON  F.acctno= C.acctno
WHERE f.TRANSTYPECODE = 'RET' 
	AND c.hldorjnt ='H'
	AND f.acctno = c.acctno
	AND c.custid = @custid
	AND F.DateTrans > @ReturnChequeMinDate

-- The customer has greater than the allowed numner of returned cheques
IF @ReturnedChequeCount > @returnchequeallowedcount BEGIN
	
	SET @authorisationrequired = 1
	
	-- Return a list of returned cheques
	SELECT F.acctno  
		, AC.currstatus	 
		, transvalue 
		, datetrans  
	FROM  FinTrans F INNER JOIN 
		CustAcct C	ON F.acctno = C.acctno LEFT OUTER JOIN 
		acct AC		ON AC.Acctno = C.acctno 
	WHERE f.transtypecode = 'RET' 
		AND c.hldorjnt ='H'
		AND f.acctno = c.acctno
		AND c.custid = @custid
		AND F.DateTrans > @ReturnChequeMinDate
END

SET @return = @@error
	

GO