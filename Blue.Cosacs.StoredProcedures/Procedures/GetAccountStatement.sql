SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

---------------------------------------------------------------------------------------------------

IF exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetAccountStatement]') and objectproperty(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetAccountStatement]
GO

CREATE PROCEDURE [dbo].[GetAccountStatement] 	
	@AcctNo Char(12),
	@Date DateTime,
	@return Int OUTPUT
AS

SET @return = 0


if datepart(year, @date) = '1900'

select @date = dateacctopen
FROM	acct 
WHERE	AcctNo = @AcctNo


SELECT	agrmttotal, outstbal = CASE ISNULL(bdwbalance,0) WHEN 0 THEN outstbal ELSE bdwbalance + ISNULL(bdwcharges,0) END , acctno
FROM	acct 
WHERE	AcctNo = @AcctNo

SELECT	fintrans.acctno, 
		fintrans.transrefno, 
		fintrans.datetrans, 
		fintrans.transtypecode, 
		fintrans.transvalue 
FROM fintrans 
WHERE fintrans.acctno=@AcctNo and fintrans.datetrans >= @Date
UNION 
SELECT fa.LinkedAcctNo,
		f.transrefno, 
		f.datetrans, 
		f.transtypecode, 
		f.transvalue 
FROM fintrans f
join FinTransAccount FA ON f.acctno = FA.AcctNo AND f.datetrans = FA.DateTrans AND f.branchno = FA.BranchNo AND f.transrefno = FA.TransRefNo
AND fa.LinkedAcctNo = @AcctNo and f.datetrans >= @Date
ORDER BY datetrans DESC

SELECT customer.name, 
	   customer.firstname,
	   customer.title,
	   customer.RFCreditLimit, 
	   customer.AvailableSpend, 
	   custaddress.cusaddr1, 
	   custaddress.cusaddr2, 
	   custaddress.cusaddr3, 
	   custaddress.cuspocode 
FROM customer
JOIN custaddress ON customer.custid=custaddress.custid
WHERE custaddress.addtype='H' and customer.custid=(SELECT MAX(custid) FROM custacct WHERE acctno=@AcctNo AND hldorjnt='H')

SET @return = @@ERROR

RETURN @return


GO 
declare @p3 int
set @p3=0
exec GetAccountStatement @acctno=N'640081613171',@date='2011-06-26 12:54:25',@return=@p3 output
select @p3
