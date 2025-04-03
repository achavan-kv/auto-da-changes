SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

---------------------------------------------------------------------------------------------------

IF exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GetCustomerStatement]') and objectproperty(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetCustomerStatement]
GO

CREATE PROCEDURE [dbo].[GetCustomerStatement] 	
	@CustID Char(12),
	@Date DateTime,
	@onlyHolderAccounts BIT,
	@return Int OUTPUT
AS

SET @return = 0

select top 0 acct.agrmttotal, acct.outstbal, acct.acctno
into #accts
from acct

if @onlyHolderAccounts = 1
begin
	insert into #accts	
	select acct.agrmttotal, outstbal = CASE ISNULL(bdwbalance,0) WHEN 0 THEN outstbal ELSE bdwbalance + ISNULL(bdwcharges,0) END , acct.acctno
	FROM acct 
	join custacct on acct.acctno = custacct.acctno 
	where custacct.custid = @CustID
	and hldorjnt = 'H'
end
else
begin
	insert into #accts
	select acct.agrmttotal, acct.outstbal, acct.acctno
	FROM	acct 
	join custacct on acct.acctno = custacct.acctno 
	where custacct.custid = @CustID
end

SELECT * FROM #accts

SELECT	fintrans.acctno, 
		fintrans.transrefno, 
		fintrans.datetrans, 
		fintrans.transtypecode, 
		fintrans.transvalue 
FROM	acct 
join #accts on acct.acctno = #accts.acctno 
join fintrans on fintrans.acctno = acct.acctno
where fintrans.datetrans >= @Date
UNION 
SELECT fa.LinkedAcctNo,
		f.transrefno, 
		f.datetrans, 
		f.transtypecode, 
		f.transvalue 
FROM fintrans f
join FinTransAccount FA ON f.acctno = FA.AcctNo AND f.datetrans = FA.DateTrans AND f.branchno = FA.BranchNo AND f.transrefno = FA.TransRefNo
JOIN #accts A ON a.acctno= fa.LinkedAcctNo
and f.datetrans >= @Date
ORDER BY datetrans DESC


SELECT top 1 customer.name, 
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
WHERE custaddress.addtype='H' and customer.custid=@CustID

SET @return = @@ERROR

RETURN @return
GO 