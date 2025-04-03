-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

insert into CashLoan (Custid, AcctNo, LoanAmount, Term, LoanStatus, TermsType, EmpeenoAccept, EmpeenoDisburse)
select ca.custid,a.acctno,SUM(l.ordval),i.instalno,case when l.delqty>=l.quantity then 'D' else ' ' end,a.termstype,ag.createdby,ag.empeenoauth
from acct a INNER JOIN termstype t on a.termstype=t.TermsType
					INNER JOIN custacct ca on a.acctno=ca.acctno and ca.hldorjnt='H'
					INNER JOIN lineitem l on l.acctno=a.acctno and l.itemno='Loan'
					INNER JOIN instalplan i on i.acctno=a.acctno
					INNER JOIN agreement ag on ag.acctno=a.acctno
where t.isloan=1
	and not exists(select * from dbo.CashLoan cl where a.acctno=cl.acctno)
	and l.quantity=1
	and accttype='R'
	and l.ordval > 0
group by ca.custid,a.acctno,i.instalno,case when l.delqty>=l.quantity then 'D' else ' ' end,a.termstype,ag.createdby,ag.empeenoauth
	
