--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
--
--NOTE: If this calculation changes then the SP DN_CustomerGetRFLimitSP will have to be changed as well
--
--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
--- remove 100  happy with result
go
if  exists (select * from sysobjects  where name =  'dn_CustomerCalculateAvailableSpendAll' )
drop PROCEDURE 	dbo.dn_CustomerCalculateAvailableSpendAll
go

-- 11/Jan/2006 Update calc to use addtos [PC]
create PROCEDURE 	dbo.dn_CustomerCalculateAvailableSpendAll
/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : dn_CustomerCalculateAvailableSpendAll.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : Procedure that calculates the available limit for all customers
-- Author       : ?
-- Date         : ?
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
 
  13/10/11   jec CR1232 exclude CLD transtypecode from payments
************************************************************************************************************/
@return int =0 output 
AS
set @return = 0
       select A.acctno, A.agrmttotal, A.outstbal, isnull (AG.servicechg, 0) as Servicecharge, ca.custid,
             isnull (AG.deposit, 0) as deposit, rfcreditlimit,convert(money,0) as available,convert(money,0) as payments,
             convert(money,0) as nonstockvalue, convert(money,0) as used
       into #rflimit
	FROM	acct A INNER JOIN custacct CA ON A.acctno = CA.acctno 
         INNER JOIN customer C on c.custid =CA.custid
			INNER JOIN agreement AG ON A.acctno = AG.acctno
			INNER JOIN accttype AT ON A.accttype = AT.genaccttype	-- added 27/11/03 JJ because H is actually O in acct table.
	WHERE	AT.accttype NOT IN ('C', 'S') and c.rfcreditlimit >0 --and c.custid like 'S7403%' 
    and  CA.hldorjnt ='H'
	AND		a.currstatus<>'S'
	-- exclude cash Loan accounts that have not been disbursed  jec CR1232
	AND NOT EXISTS (select * from CashLoan CL							
							where CL.acctno = ca.acctno
							AND CL.LoanStatus = '')
							
      create clustered index ix_payments on #rflimit(acctno)

      update #rflimit set payments =isnull((select sum(transvalue) from fintrans f
      where f.acctno= #rflimit.acctno and f.transtypecode not in ('DEL','GRT', 'ADD', 'CLD')),0) -- jec CR1232 Cash Loan Disbursement

      update #rflimit set nonstockvalue =isnull(
		(SELECT	SUM(ordval)
		FROM	lineitem l, stockitem s
		WHERE	l.acctno = #rflimit.acctno
		AND		l.itemtype = 'N'
		--AND		l.itemno NOT IN('DT', 'SD', 'ADDDR','ADDCR','LOAN') --Addition of addtos [PC]
		AND		s.iupc NOT IN('DT', 'SD', 'ADDDR','ADDCR','LOAN') --Addition of addtos [PC]					--IP - 02/08/11 - RI
		--AND		l.itemno = s.itemno
		AND		l.ItemID = s.ID										--IP - 02/08/11 - RI
		AND		l.stocklocn = s.stocklocn
		--AND		s.category NOT IN(36,37,38,46,47,48,86,87,88)) -- exclude discounts
		AND		s.category NOT IN (select code from code where category = 'PCDIS')) -- exclude discounts --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
		
,0)

	update #rflimit set payments = 0 /*has not yet paid the deposit */
                 	where payments + deposit >= 0

	update #rflimit  
			SET payments =payments + deposit
                      	where payments + deposit < 0

	update #rflimit  
	SET payments =payments * ((agrmttotal-servicecharge-nonstockvalue)/agrmttotal)
   where agrmttotal > 0

	update #rflimit 
   set used = used + agrmttotal - serviceCharge - deposit + payments 
   where agrmttotal >0

	update #rflimit 
   set used = used - nonstockvalue 
   where agrmttotal >0

	
  select custid, sum(used) as used, Max (rfcreditlimit) as rfcreditlimit,convert (money, 0) as available
  into #rflimitcust
  from #rflimit group by custid
   
   update #rflimitcust set available = rfcreditlimit - used

  update customer set availablespend =#rflimitcust.available 
   from #rflimitcust where customer.custid = #rflimitcust.custid
  -- customers with no open credit accounts should have limit set to credit limit
   	UPDATE customer SET availablespend = customer.rfcreditlimit
	WHERE NOT EXISTS (SELECT * FROM custacct ca, acct a ,instalplan i
	WHERE a.acctno = ca.acctno AND ca.hldorjnt = 'H' AND i.acctno= a.acctno 
	AND a.currstatus !='S' AND ca.custid = customer.custid)
	AND RFCreditLimit >0 and availablespend !=rfcreditlimit -- don't update if no change

go

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
