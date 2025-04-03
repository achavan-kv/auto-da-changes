
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].Summary16SP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE Summary16SP
END
GO

CREATE PROCEDURE dbo.Summary16SP
/*
** Author	: K. E. Fernandez (Strategic Thought)
** Date		: 16-Aug-2004
** Version	: 1.0
** Name		: Summary Table 16
** Details	: Created to generate summary table specifically for the Housekeeping Report - clean-up
**
** Who  Date     Change
** ---	----	 ------								
** KEF  08/09/04 Instal amount report is not showing Instalno or Outstbalcorr data
*/
--========================================================================================
			@return int
as 

set @return=0

TRUNCATE TABLE summary16
--go

--report 1 - ltv (servicechg)
insert into summary16 (report, acctno, agrmttotal, servicechg,
	 	     ltv, asatdate)
select	'ltv', a.acctno, ag.agrmttotal, ag.servicechg,
	round((ag.agrmttotal/(ag.agrmttotal-ag.servicechg)),2), getdate()
from	agreement ag, acct a
where 	(ag.agrmttotal-ag.servicechg) <> 0
and 	a.acctno = ag.acctno
and 	a.agrmttotal >0
and		a.accttype not in ('S', 'C', 'T')
and 	a.outstbal != 0
and 	a.currstatus !='S'
and 	round((ag.agrmttotal/(ag.agrmttotal-ag.servicechg)),2) not between 1.01 and 2


--report 2 - maturity date (datelast)
insert into summary16 (report, acctno, datelast, agrmttotal, outstbalcorr, 
		      datelastpaid, asatdate)
select 	'mat', a.acctno, i.datelast, a.agrmttotal, s.outstbalcorr,
	s.datelastpaid, getdate()
from 	acct a, instalplan i, agreement ag, summary1 s
where 	a.acctno = i.acctno
and 	a.acctno = ag.acctno
and 	a.acctno = s.acctno
and 	a.agrmttotal >0
and		a.accttype not in ('S', 'C', 'T')
and 	a.outstbal != 0
and 	a.currstatus !='S'
--and 	i.datelast < '01-jun-03'


--report 3 - Agreement Total (Agrmttotal)
insert into summary16 (report, acctno, agrmttotal, asatdate)
select 	'agr', a.acctno, ag.agrmttotal, getdate()
from	acct a, agreement ag
where 	a.acctno = ag.acctno
and		a.accttype not in ('S', 'C', 'T')
and 	a.outstbal != 0
and 	a.currstatus !='S'

--report 4 - Agreement Term (Instalno)
insert into summary16 (report, acctno, instalno, agrmttotal, instalamount, asatdate)
select 	'term', a.acctno, i.instalno, a.agrmttotal, i.instalamount, getdate()
from 	instalplan i, acct a
where   a.acctno = i.acctno
and 	a.agrmttotal >0
and		a.accttype not in ('S', 'C', 'T')
and 	a.outstbal != 0
and 	a.currstatus !='S'
and	i.instalno not between 6 and 60


--report 5 - Deposit % (deposit)
insert into summary16 (report, acctno, Depositpcent, agrmttotal, deposit, asatdate)
select 	'dep', a.acctno, round((ag.deposit/ag.agrmttotal)*100,0), a.agrmttotal, ag.deposit, getdate()
from 	agreement ag, acct a
where 	a.acctno = ag.acctno
and     ag.agrmttotal > 0
and		a.accttype not in ('S', 'C', 'T')
and 	a.outstbal != 0
and 	a.currstatus !='S'
and	round((ag.deposit/ag.agrmttotal)*100,0) > 50
group by round((ag.deposit/ag.agrmttotal)*100,0), a.acctno, a.agrmttotal, ag.deposit
order by round((ag.deposit/ag.agrmttotal)*100,0)


--report 6 - Instalment Amount (instalamount)
insert into summary16 (report, acctno, agrmttotal, outstbalcorr, instalno, instalamount, asatdate)
select 	'instal', a.acctno, a.agrmttotal, s.outstbalcorr, i.instalno, i.instalamount, getdate()
from 	instalplan i, acct a, summary1 s
where 	a.acctno = i.acctno
and	a.acctno = s.acctno
and     a.agrmttotal > 0
and		a.accttype not in ('S', 'C', 'T')
and 	a.outstbal != 0
and 	a.currstatus !='S'
--and	instalamount <= 0
order by a.acctno
--go

set @return=@@ERROR

GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End