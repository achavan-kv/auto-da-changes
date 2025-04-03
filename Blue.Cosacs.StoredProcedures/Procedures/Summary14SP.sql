SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].Summary14SP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE Summary14SP
END
GO

CREATE PROCEDURE dbo.Summary14SP
/*
** Author	: K. E. Fernandez (Strategic Thought)
** Date		: 17-Aug-2004
** Version	: 1.0
** Name		: Summary Table 14
** Details	: Created to generate summary table specifically for the Housekeeping Report - AOB Breakdown
**
** Who  Date     Change
** ---	----	 ------								
** KEF  13/01/05 66582 dded extra clause to exclude special accounts from SC figure, so it matches change done in Report 11
*/
--========================================================================================
			@return int
as 

set @return=0

TRUNCATE TABLE summary14
--go

--ST
insert into summary14 (report, acctno, custid, outstbal, outstbalcorr, charges, currstatus, delamount,
	 	 deposit, outstbal_less_deposit, securitised, asatdate)
select	'ST', acctno, custid, outstbal, outstbalcorr, outstbal - outstbalcorr, currstatus, delamount,
	deposit, outstbal-deposit, securitised, getdate()
from	summary1
where 	currstatus = 'S'
AND	outstbal <> 0
--go



--SP
insert into summary14 (report, acctno, custid, outstbal, outstbalcorr, charges, currstatus, delamount,
	 	 deposit, outstbal_less_deposit, securitised, asatdate)
select	'SP', acctno, custid, outstbal, outstbalcorr, outstbal - outstbalcorr, currstatus, delamount,
	deposit, outstbal-deposit, securitised, getdate()
from	summary1
where	accttype = 'S'
AND	currstatus	<> 'S'
and outstbal not between -0.00999999 and 0.009999999
--go



--SC
insert into summary14 (report, acctno, custid, outstbal, outstbalcorr, charges, currstatus, delamount,
	 	 deposit, outstbal_less_deposit, securitised, asatdate)
select	'SC', acctno, custid, outstbal, outstbalcorr, outstbal - outstbalcorr, currstatus, delamount,
	deposit, outstbal-deposit, securitised, getdate()
from	summary1
where	(currstatus in ('U','','0') or currstatus is null)
and	outstbalcorr > 0
and	deliveryflag = 'Y'
and	accttype <> 'S' /* KEF 13/01/05 Added to avoid duplication */
--go



--ND
insert into summary14 (report, acctno, custid, outstbal, outstbalcorr, charges, currstatus, delamount,
	 	 deposit, outstbal_less_deposit, securitised, asatdate)
select	'ND', acctno, custid, outstbal, outstbalcorr, outstbal - outstbalcorr, currstatus, delamount,
	deposit, outstbal-deposit, securitised, getdate()
from	summary1
where	deliveryflag 	<> 'Y'	
AND	currstatus	<> 'S'
and accttype!='S'
and outstbal not between -0.00999999 and 0.009999999
--go



--CR
insert into summary14 (report, acctno, custid, outstbal, outstbalcorr, charges, currstatus, delamount,
	 	 deposit, outstbal_less_deposit, securitised, asatdate)
select	'CR', acctno, custid, outstbal, outstbalcorr, outstbal - outstbalcorr as charges, currstatus, delamount,
	deposit, outstbal-deposit, securitised, getdate()
from	summary1
where	outstbalcorr 	<= 	0
AND	deliveryflag 	= 	'Y'
AND	accttype	<> 	'S'	/* CJB 14/09/00 - exclude specials */
AND	currstatus	<> 	'S'
--AND	currstatus	<>	'6'
--go



--AD
--load interest and admin totals by account
select	acctno, transtypecode, convert(money,sum(transvalue)) as transvalue
into	#intadmddf
from	fintrans
where	transtypecode in ('int','adm','ddf')
group by acctno, transtypecode
--go

--unsettled breakdown by acctno
select	acctno, custid, outstbal, outstbalcorr, outstbal - outstbalcorr as charges, currstatus,
	delamount, deposit, outstbal-deposit as outstbal_less_deposit, securitised,
	convert(money,0.0) as interest, convert(money,0.0) as admin,
	convert(money,0.0) as direct_debit_fee
into	#unsettled
from	summary1 a
WHERE   currstatus <> 'S'
--go
update	#unsettled set interest = transvalue
from	#unsettled a, #intadmddf b
where	transtypecode = 'int'
and	a.acctno = b.acctno
--go
update	#unsettled set admin = transvalue
from	#unsettled a, #intadmddf b
where	transtypecode = 'adm'
and	a.acctno = b.acctno
--go
update	#unsettled set direct_debit_fee = transvalue
from	#unsettled a, #intadmddf b
where	transtypecode = 'ddf'
and	a.acctno = b.acctno
--go
insert into summary14 (report, acctno, custid, outstbal, outstbalcorr, charges, currstatus, delamount,
	 	 deposit, outstbal_less_deposit, securitised, interest, admin, direct_debit_fee, asatdate)
select	'AD', *, getdate()
from	#unsettled
where	not (admin = 0 and interest = 0 and direct_debit_fee = 0)
--go

--settled breakdown by acctno
select	acctno, custid, outstbal, outstbalcorr, outstbal - outstbalcorr as charges, currstatus,
	delamount, deposit, outstbal-deposit as outstbal_less_deposit, securitised,
	convert(money,0.0) as interest, convert(money,0.0) as admin,
	convert(money,0.0) as direct_debit_fee
into	#settled
from	summary1 a
WHERE   currstatus = 'S'
AND     outstbal <> 0
--go
update	#settled
set	interest = transvalue
from	#settled a, #intadmddf b
where	transtypecode = 'int'
and	a.acctno = b.acctno
--go
update	#settled
set	admin = transvalue
from	#settled a, #intadmddf b
where	transtypecode = 'adm'
and	a.acctno = b.acctno
--go
update	#settled
set	direct_debit_fee = transvalue
from	#settled a, #intadmddf b
where	transtypecode = 'ddf'
and	a.acctno = b.acctno
--go
insert into summary14 (report, acctno, custid, outstbal, outstbalcorr, charges, currstatus, delamount,
	 	 deposit, outstbal_less_deposit, securitised, interest, admin, direct_debit_fee, asatdate)
select	'AD', *, getdate()
from	#settled
where	not (admin = 0 and interest = 0 and direct_debit_fee = 0)
--go

--update securitised column with N
update	summary14
set	securitised = 'N'
where	isnull(securitised,'') <> 'Y'
--go

--drop temporary tables
drop table #intadmddf
drop table #unsettled
drop table #settled
--go


set @return=@@ERROR

GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End

