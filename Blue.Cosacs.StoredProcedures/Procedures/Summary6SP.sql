
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].Summary6SP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE Summary6SP
END
GO

CREATE PROCEDURE dbo.Summary6SP

/*****************************************************************************************************************
** Author	: K. E Fernandez
** Date		: 11-MAR-2004
** Version	: 1.0
** Name		: Summary6_sql.sql
** Details	: CR504 - Script to produce figures for average agreement length by
**                termstype - based on script written by Mike Collins at year end 2003
** Modified	:
** Who	When	  Description
** ---  ----      -----------
** KEF  15/04/04  CR597 Reports need to be split between securitised and non-securitised accounts
** KEF  21/06/04  Changed where clause as datedel can be set to 1910.
******************************************************************************************************************/

/***** Summary6_sec *****/

 @return     int OUTPUT
AS

set @return = 0

TRUNCATE TABLE summary6_sec
--GO

/* Create skeleton table with accounts settled and date settled
** Exclude all accounts with Null delivery dates or delivery dates outside 10 year */
select	a.acctno, dateacctopen, datedel, convert(money,0) as SettledInMths,
	max(datestatchge) as datesettled, s.statuscode, a.termstype, a.instalno
into	#stat_sec
from	summary1 a, status s, agreement ag
where	securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	statuscode = 'S'
and	a.acctno = s.acctno 
and	a.acctno = ag.acctno 
and	a.currstatus = 'S' 
and	isnull(datedel,'') > '01-jan-1910' --KEF Changed from <> 1900
and	(datediff(year,datedel, getdate())  < 10
	OR datediff(year,datedel, getdate())  > -10)
and	accttypegroup in ('HP', 'RF', 'IFC')		--Don't want other account types as don't have proper agreements 
group by a.acctno, dateacctopen, datedel, s.statuscode, a.termstype, a.instalno
having	isnull(max(datestatchge) ,'') <> '01-jan-1900'	--Exclude accounts with no valid settlement date
--GO

-- Find the settlement period for each account
update	#stat_sec
set	settledInMths = datediff(month,datedel, datesettled)
--GO


-- Calculate the settlement percentage ratio on actual over full term  
insert into summary6_sec
	(term, termstype, description, SettledInMths, 
	Settled_Accts, datesettled, dateacctopen, TermPcent, asatdate)
select	instalno, t.termstype, t.description, SettledInMths, 
	s.acctno, datesettled, dateacctopen, ((SettledInMths*100)/instalno), getdate()
from 	#stat_sec s, termstype t--, intratehistory i	--SHOULD CHECK INTRATEHISTORY TOO AS THIS HAS HISTORIC DATA
where 	s.termstype = t.termstype
--and	s.termstype = i.termstype
--and	i.inspcent > 0					--Don't want to exclude termstypes with no insurance
and	instalno between 1 and 100
--AND	(dateacctopen between i.datefrom and i.dateto 
--        OR (dateacctopen >= i.datefrom and i.dateto = CONVERT(DATETIME,'01 jan 1900',106)) )
--GO


/***** Summary6_non *****/


TRUNCATE TABLE summary6_non
--GO

/* Create skeleton table with accounts settled and date settled
** Exclude all accounts with Null delivery dates or delivery dates outside 10 year */
select	a.acctno, dateacctopen, datedel, convert(money,0) as SettledInMths,
	max(datestatchge) as datesettled, s.statuscode, a.termstype, a.instalno
into	#stat_non
from	summary1 a, status s, agreement ag
where	securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	statuscode = 'S'
and	a.acctno = s.acctno 
and	a.acctno = ag.acctno 
and	a.currstatus = 'S' 
and	isnull(datedel,'') > '01-jan-1910' 	 --KEF Changed from <> 1900
and	(datediff(year,datedel, getdate())  < 10
	OR datediff(year,datedel, getdate())  > -10)
and	accttypegroup in ('HP', 'RF', 'IFC')		--Don't want other account types as don't have proper agreements 
group by a.acctno, dateacctopen, datedel, s.statuscode, a.termstype, a.instalno
having	isnull(max(datestatchge) ,'') <> '01-jan-1900'	--Exclude accounts with no valid settlement date
--GO

-- Find the settlement period for each account
update	#stat_non
set	settledInMths = datediff(month,datedel, datesettled)
--GO


-- Calculate the settlement percentage ratio on actual over full term  
insert into summary6_non
	(term, termstype, description, SettledInMths, 
	Settled_Accts, datesettled, dateacctopen, TermPcent, asatdate)
select	instalno, t.termstype, t.description, SettledInMths, 
	s.acctno, datesettled, dateacctopen, ((SettledInMths*100)/instalno), getdate()
from 	#stat_non s, termstype t--, intratehistory i	--SHOULD CHECK INTRATEHISTORY TOO AS THIS HAS HISTORIC DATA
where 	s.termstype = t.termstype
--and	s.termstype = i.termstype
--and	i.inspcent > 0					--Don't want to exclude termstypes with no insurance
and	instalno between 1 and 100
--AND	(dateacctopen between i.datefrom and i.dateto 
--        OR (dateacctopen >= i.datefrom and i.dateto = CONVERT(DATETIME,'01 jan 1900',106)) )
--GO

--End of Summary6 script

SET @Return = @@ERROR
--GO

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End 