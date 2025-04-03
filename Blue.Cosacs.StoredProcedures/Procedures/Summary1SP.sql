SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].Summary1SP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE Summary1SP
END
GO

CREATE PROCEDURE dbo.Summary1SP
 
 /*
		New SQL Reporting - Summary1 view
	Author:	John Croft
	Date:	2nd April 2007

-- Modifications
-- 09/10/07	JEC 	UAT 12 Remove unwanted columns
-- 22/10/07 jec     Acctstatusdescr now held on Summary1AcctStatus 
-- 23/10/07 AA speeding up by only refreshing live and recently settled accounts if full run has not been selected.
-- 13/02/08 jec     Correct monthsarrears & Baldue12mths calculation & include 'M' accttype in HP accttypegroup 
-- 14/02/08 jec     Correct datedel determination.
-- 15/12/08 jec Add StatusCodeBand column  for CR984 reports
-- 28/01/09 jec correct statuscodeband St
-- 30/01/09 jec modified for instalplan missing UAT89
-- 15/07/10 jec Correct error in code and truncate Summary1AcctStatus
--				UAT206 Add DAD Application status.
--				UAT208 Correct INP Application status.
-- 27/07/10 jec	UAT208 Correct error - INP Application status
-- 28/07/10 jec UAT205 Duplicate statuses
--				UAT229 Removed from Load
-- 29/07/10 jec UAT231 SCH status
-- 30/07/10 jec UAT210 Correct INP/SCH Application status.	
-- 02/03/11 ip  Sprint 5.11 - #2811 - CR1090 - First Instalment Waiver changes. Also added ADI status for Instant Credit accounts.	
-- 31/03/11 jec above change #2811 removed for 2.2.1.0 release
-- 10/08/11 ip  RI - System Integration Changes
-- 11/08/11 jec Avoid duplicate Account Statuses when not full refresh	
-- 20/03/12 ip  #9805 - Changed calculation for Store Card - MonthsArrears, DaysArrears
-- 23/05/12 jec #10134 LW75033 - Cosacs EOD Error - Dominica
-- 16/01/13 ip  #11763 Singer Summary Changes - Merged from CoSACS 6.5
-- 30/05/13 jec #13760 CR12949 - Summary Changes
-- 07/08/13 jec #14285 - Outstanding Bookings Report - add new statuses to AcctStatus 
-- 23/07/14 IP  #19510 - CR15594 - Remove AST code for Ready Assist
*/
-- values required for indexed views
 @return     int OUTPUT
AS

SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET CONCAT_NULL_YIELDS_NULL ON
SET NUMERIC_ROUNDABORT OFF
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON

set @return = 0

--GO
exec ScriptRunTimeSp 'S','Summary1 view','Script'
exec ScriptRunTimeSp 'S','Summary1 view','step01'

insert into status
select 0,a.acctno,GETDATE(),99999,currstatus,0,'Y' 
from acct a,status s,
(select acctno, MAX(datestatchge) as datestatuschge from status
group by acctno) s2
where a.acctno=s.acctno
and s.acctno=s2.acctno
and s.datestatchge=s2.datestatuschge
and a.currstatus!=s.statuscode

update custacct set hldorjnt='H'
where not exists
(select 'x' from custacct b
where custacct.acctno=b.acctno
and hldorjnt='h')
and custid= 
(select top 1 custid from proposal c
where custacct.acctno=c.acctno
)
--IP - 02/11/11 - Moved view to outside procedure
-- view must be dropped before table can be truncated - this view must be included in summary 1 processing
--if exists(select * FROM dbo.sysobjects 
--               WHERE ID = object_id('dbo.vw_Summary1_G') AND OBJECTPROPERTY(id, 'IsView') = 1)
--	drop view dbo.vw_Summary1_G
--GO 


IF NOT EXISTS(SELECT * FROM sysobjects WHERE NAME ='temp_Summary1_MR')
   SELECT * INTO temp_Summary1_MR
   FROM Summary1_MR WHERE acctno = 'bob'
--GO
   TRUNCATE TABLE temp_Summary1_MR
--GO
if (not exists
(select * from sys.columns a inner join sys.objects b
 on  a.object_id=b.object_id
 where a.name like 'Repoamt' and b.name='temp_Summary1_MR'))
 begin
 alter table temp_Summary1_MR add repoamt money
 end
--GO
IF NOT EXISTS(SELECT * FROM sysobjects WHERE NAME ='temp_Summary1_MR_acctno')
   ALTER TABLE temp_Summary1_MR ADD CONSTRAINT temp_Summary1_MR_acctno PRIMARY KEY  CLUSTERED ( acctno)
--GO
if (not exists
(select * from sys.columns a inner join sys.objects b
 on  a.object_id=b.object_id
 where a.name like 'Repoamt' and b.name='Summary1_MR'))
 begin
 alter table Summary1_MR add repoamt money
 end
--GO
DECLARE @isfullrun SMALLINT
	SELECT @isfullrun = ISfullrun FROM  summaryrun 
IF @isfullrun = 1
BEGIN
   truncate table Summary1_MR
   truncate table Summary1AcctStatus	-- 15/07/10 jec 
END
ELSE
BEGIN -- we are only removing live or recently settled accounts - this should speed up the update
   DELETE FROM summary1_MR WHERE outstbalcorr <>0 OR EXISTS (SELECT * FROM acct a WHERE a.acctno = summary1_mr.acctno 
   AND (a.outstbal <>0 OR a.currstatus !='S' ))
   OR EXISTS (SELECT * FROM status s WHERE s.acctno = summary1_mr.acctno AND s.datestatchge >dateadd(MONTH,-2,GETDATE()))
   OR EXISTS (SELECT * FROM fintrans f WHERE f.acctno = summary1_mr.acctno AND f.datetrans >dateadd (MONTH,-2,GETDATE()))
END


exec ScriptRunTimeSp 'F','Summary1 view','step01'

exec ScriptRunTimeSp 'S','Summary1 view','step02'
	
-- Create index and load table
if not exists(select * FROM dbo.sysindexes 
               WHERE name = 'summary1_MR_idx' )
	CREATE UNIQUE clustered  INDEX summary1_MR_idx on Summary1_MR (acctno) 
--GO
 
insert into temp_Summary1_MR(acctno,acctchar4,accttypegroup,acctlife,outstbalcorr,dayssincereposs,spaflag,delamount,monthsarrears,
			baldue12mths,baldueafter12mths,payamount,fullydeliveredflag,monthsarrearsnew,outstbalcorrnew,bdw,
			cancelledflag,servicechg,deposit,acctstatusdescr,deliveryflag)

select a.acctno,substring(a.acctno,4,1),isnull(vB.accttypegroup,' '),0,isnull(outstbal,0),0,'N',0,0,
		0,0,0,'N',0,isnull(outstbal,0),0,
		'N',0,0,' ','Y'

From acct a,vw_Summary1_B vB
where a.acctno=vB.acctno
AND NOT EXISTS (SELECT * FROM Summary1_MR m WHERE m.acctno = a.acctno)
--go

exec ScriptRunTimeSp 'F','Summary1 view','step02'

exec ScriptRunTimeSp 'S','Summary1 view','step03'

--drop table #mydates
IF object_id('tempdb..#mydates') IS NOT NULL 
DROP TABLE [dbo].#mydates

DECLARE @now datetime 
DECLARE @today datetime

SET @now = getdate()
SELECT	@today = CONVERT(DATETIME, CONVERT(VARCHAR(10), GETDATE(), 105), 105)
SELECT @now as 'now', @today as 'today' into #mydates
--GO
--DECLARE @today datetime
SET @today = (select today from #mydates)
--select * from #mydates

-- Update temp_Summary1_MR table - acctlife 
UPDATE	temp_Summary1_MR
	SET	acctlife = datediff (day, dateadd(month,-1,datefirst), @today)
	from 	temp_Summary1_MR s, instalplan b
	WHERE	isnull(datefirst,'')	> '01-jan-1910'		
	AND	s.acctno = b.acctno

;with fin (acctno, datetrans)
as (select f.acctno, min(datetrans)
		from fintrans f
		where RIGHT(LEFT(acctno, 4), 1) = '9'
		group by f.acctno)
-- Update temp_Summary1_MR table - acctlife for storecard
UPDATE	temp_Summary1_MR
	SET	acctlife = datediff (day, b.datetrans, @today)
	from 	temp_Summary1_MR s, fin b
	WHERE	isnull(datetrans,'')	> '01-jan-1910'		
	AND	s.acctno = b.acctno



exec ScriptRunTimeSp 'F','Summary1 view','step03'

exec ScriptRunTimeSp 'S','Summary1 view','step04'

-- Update temp_Summary1_MR table - Outstbalcorr
Update temp_Summary1_MR
	set outstbalcorr = CASE WHEN a.accttype = 'T' THEN outstbalcorr ELSE outstbalcorr - intcharges END
		from temp_Summary1_MR s, vw_Summary1_E_Fintrans f, acct a
	where s.acctno=f.acctno
		and s.acctno=a.acctno
		and outstbalcorr!=0		-- initially set to outstbal
-- Update temp_Summary1_MR table - Accttypegroup

Update temp_Summary1_MR
	set accttypegroup=vB.accttypegroup
		from temp_Summary1_MR s, vw_Summary1_B vB
	where s.acctno=vB.acctno 

exec ScriptRunTimeSp 'F','Summary1 view','step04'

Update temp_Summary1_MR
	set repoamt=isnull(vB.transvalue,0)
		from temp_Summary1_MR s, vw_Summary1_C2_Fintrans vB
	where s.acctno=vB.acctno 
	
exec ScriptRunTimeSp 'S','Summary1 view','step05'

/* determine the days since account was repossessed (if at all)
** - begin by determining earliest transaction date for repossession */
--drop table #fintrans_reposs_date
IF object_id('tempdb..#fintrans_reposs_date') IS NOT NULL 
DROP TABLE [dbo].#fintrans_reposs_date

Select acctno,
        min(datetrans) AS 'datereposs',
		convert(integer,0) AS 'dayssincereposs'
	Into #fintrans_reposs_date
	From vw_Summary1_C_Fintrans
	Where transtypecode in ('REP','RPO')
	
	Group by acctno

--Create index for #fintrans_reposs_date
CREATE CLUSTERED INDEX #ix_fintrans_reposs_date on #fintrans_reposs_date (acctno)
--GO

--DECLARE @today datetime
SET @today = (select today from #mydates)
UPDATE  temp_Summary1_MR
	set dayssincereposs = datediff (day , datereposs, @today)
	From temp_Summary1_MR s,#fintrans_reposs_date f
	Where s.acctno=f.acctno

-- 38s Jamaica
--go


exec ScriptRunTimeSp 'F','Summary1 view','step05'

exec ScriptRunTimeSp 'S','Summary1 view','step06'

-- Update temp_Summary1_MR table - datespaadded
	Update	temp_Summary1_MR
		Set	datespaadded = CONVERT(SMALLDATETIME,vF.datespaadded), spaflag='Y'

	From	temp_Summary1_MR s,dbo.vw_Summary1_BA vF
	WHERE	s.acctno = vF.acctno
-- 1m 1s Jamaica

-- Update temp_Summary1_MR table - delamount
	Update	temp_Summary1_MR
		Set	delamount = vA.delamount
	
	From	temp_Summary1_MR s,vw_Summary1_A vA
	Where	s.acctno = vA.acctno
-- 1m 8s Jamaica

--update delamount for storecard

;WITH fin(acctno, transvalue)				-- #10134
as (
		select acctno, sum(transvalue)
		from fintrans 
		where RIGHT(LEFT(acctno, 4), 1) = '9'
			and transtypecode = 'SCT'
		group by acctno 
	)
-- Update temp_Summary1_MR table - delamount
	Update	temp_Summary1_MR
		Set	delamount = vA.transvalue
	
	From	temp_Summary1_MR s,fin vA
	Where	s.acctno = vA.acctno

	--Remove dynamic sql

UPDATE	temp_Summary1_MR  
		set servicechg = a.servicechg, 
			deposit = a.deposit 
	From temp_Summary1_MR s,agreement a	 
	Where s.acctno=a.acctno 


exec ScriptRunTimeSp 'F','Summary1 view','step06'

exec ScriptRunTimeSp 'S','Summary1 view','step07'


Update	temp_Summary1_MR  
		set	datedel1 = a.datedel 
	from 	temp_Summary1_MR s 
	INNER JOIN acct ac 
		on s.acctno = ac.acctno
		AND	ac.accttype not in ( 'C', 'T') 
	inner join agreement a  
		on	s.acctno  = a.acctno  
		AND	a.agrmtno = 1  
		


	Update	temp_Summary1_MR 
		set	datedel1 = (select max(datetrans) from vw_Summary1_C_Fintrans f 
				where f.acctno = s.acctno and f.transtypecode in ('SCT', 'DEL','CLD'))					-- #10138
	from 	temp_Summary1_MR s INNER JOIN acct ac on s.acctno = ac.acctno		
					INNER JOIN vw_Summary1_C_Fintrans f on ac.acctno = f.acctno		-- jec 14/02/08
	Where	ac.accttype in ('T', 'C')			-- jec 14/02/08
-- 5m 20s Jamaica


Update	temp_Summary1_MR  
		set	datedel1 = 	a.datedel 
	from 	temp_Summary1_MR s 
	INNER JOIN acct ac 
		on s.acctno = ac.acctno
		AND	ac.accttype	in ('T', 'C')
	INNER JOIN agreement a 
		on s.acctno = a.acctno 
		AND	a.agrmtno = 1 
		AND	datedel		<> 	'' 
		AND	datedel		IS NOT NULL 
		AND	datedel		!=	datedel1 
	

-- 2m 1s Jamaica
--GO

/* CJB 16/06/00 - derive delivery flag from delivery date */
/* RD FR143525 09/07/03 Changed = 01-jan-1900 to  < 01-jan-1910 for .NET */
	Update	temp_Summary1_MR 
		Set	deliveryflag	= 'N'
	From custacct ca
	Where	temp_Summary1_MR.acctno   = 	ca.acctno
		and ca.hldorjnt='H'
		and custid not like 'PAID%'                 /* FR 104142 RD 14/08/02 */ /* KEF 09/06/03 changed to PAID% */
		AND	(datedel1 	is null
			OR  datedel1  < '01-jan-1910'		/* KEF 28/08/02 FR107914/FR93111 */
			OR	datedel1 = '')					/* CJB 28/07/00 */
--GO

exec ScriptRunTimeSp 'F','Summary1 view','step07'

exec ScriptRunTimeSp 'S','Summary1 view','step08'

-- Update temp_Summary1_MR table - monthsarrears Cash accounts
--DECLARE @today datetime
SET @today = (select today from #mydates)

	Update	temp_Summary1_MR 
		set	 monthsarrears = isnull(cast(1.0 + datediff(month,datedel1, @today ) as integer ),0),
			 daysarrears = isnull(cast(1.0 + datediff(month,datedel1, @today ) as money ),0)* 30	-- jec 12/12/08 
	Where	accttypegroup = 'C'  	
		AND	outstbalcorr > 0
		AND	isnull(datedel1,'') > '01-jan-1910' 

	/*******************************************************************************/
	/*
	** Update monthsarrears column for HP accounts
	**
	** The calculation is done as a float and then converted
	** to a smallint which truncates the number i.e. 1.867 becomes 1.
	** It is then required to be 'half-adjusted' i.e. rounded up, so
	** if the modulous of the calculation is not equal to 0 then 
	** 1 is added to the result.  It is done in two update statements 
	** to cater for this.
	**
	** The calculation also depends on the system indicator of each account 
	** (i.e. the 4th digit of the acctno), and the delivered amount.
	**
	** The calculation is converted by using a minus because if an
	** account is in arrears the number of months is stored as a negative value.
	**  CJB 06/06/00 following update amended to apply correct rounding 
	*/
	/*******************************************************************************/

	Update	temp_Summary1_MR 
		set monthsarrears = isnull(cast(0.5 + ((arrears -isnull(repoamt,0)- ISNULL(vE.intcharges,0)) / instalamount) as integer),0),	-- jec 13/02/08
			daysarrears = isnull(cast(((arrears -isnull(repoamt,0)- ISNULL(vE.intcharges,0)) / instalamount) as money),0) * 30 -- jec 12/12/08
	From temp_Summary1_MR s LEFT outer join vw_Summary1_E_Fintrans vE on vE.acctno = s.acctno,instalplan i,acct a	-- jec 13/02/08
	Where  cast(0.5 + ((arrears -isnull(repoamt,0)- ISNULL(vE.intcharges,0)) / instalamount) as float) between -1000 and  1000		-- jec 13/02/08
		AND 	instalamount > 1 
		AND	accttypegroup in ('HP', 'RF', 'CLN', 'SGR') --#19510 ,'AST')		-- #13760 Assist  Singer account
		--AND vE.acctno = s.acctno		-- jec 13/02/08
		and s.acctno=a.acctno
		and s.acctno=i.acctno
		and i.agrmtno=1
		and s.outstbalcorr>0			-- jec 13/02/08
-- 10m 3s Jamaica

--Update months and days arrears for Store card
	Update	temp_Summary1_MR 
		set monthsarrears =  isnull(CASE WHEN datedue> GETDATE()
					THEN (outstminpay+payments)/prevminpay
					--ELSE (currminpay+outstminpay+payments)/prevminpay
					ELSE (currminpay+payments)/prevminpay						--IP - 20/03/12
					END , 0),
			daysarrears = isnull( CASE WHEN datedue> GETDATE()
					THEN (outstminpay+payments)/prevminpay*30
					--ELSE (currminpay+outstminpay+payments)/prevminpay*30
					ELSE (currminpay+payments)/prevminpay*30					--IP - 20/03/12
					END, 0)
	From temp_Summary1_MR s ,vw_storecard_arrears a	
	WHERE prevminpay > 0
		AND	accttypegroup = 'SC'
		and s.acctno=a.acctno			

	Update	temp_Summary1_MR 
	set monthsarrears =  0,
		daysarrears  = 0
	where daysarrears < 0

exec ScriptRunTimeSp 'F','Summary1 view','step08'

exec ScriptRunTimeSp 'S','Summary1 view','step09'

--This code - taking over 20 minutes - why?????
	declare @delpcent as float	
	set @delpcent = (select globdelpcent from country)

	Update	temp_Summary1_MR 
		set monthsarrears = isnull(cast(0.5 + ((arrears -isnull(repoamt,0)-ISNULL(vE.intcharges,0)) / instalamount) as integer),0)	-- jec 13/02/08
	From temp_Summary1_MR s LEFT outer join vw_Summary1_E_Fintrans vE on vE.acctno = s.acctno,instalplan i,acct a	-- jec 13/02/08
	Where  cast(0.5 + ((arrears -isnull(repoamt,0)- ISNULL(vE.intcharges,0)) / instalamount) as float) between -1000 and  1000		-- jec 13/02/08
		AND 	instalamount >1 
		--AND     vE.acctno = s.acctno		-- jec 13/02/08
		and s.acctno=a.acctno
		and s.acctno=i.acctno
		and i.agrmtno=1
		and s.outstbalcorr>0			-- jec 13/02/08
		AND	accttypegroup in ('HP', 'RF', 'CLN', 'SGR') --#19510,'AST')		-- #13760 Assist Singer account
		AND	(
			  (substring(s.acctno,4,1) = '0' and
			   ( agrmttotal =  0 OR
				( agrmttotal <> 0 AND ((delamount / isnull(agrmttotal,1)) * 100) > @delpcent)	/* KEF 16/06/03 */
			   )
			  )
				OR
			  (substring(s.acctno,4,1) in ('1','2','3') AND
			   ( agrmttotal =  0 OR
				(agrmttotal <> 0 AND ((delamount / isnull(agrmttotal,1)) * 100) > 50)
			   )
			  )
			)
-- 35m 18s Jamaica
exec ScriptRunTimeSp 'F','Summary1 view','step09'

exec ScriptRunTimeSp 'S','Summary1 view','step10'

/* Double monthsarrears for those HP accounts with system indicator = 2.
** (System Indicator is the 4th character of the account number). */
	Update	temp_Summary1_MR 
		set monthsarrears = isnull((monthsarrears * 2),0)
	Where accttypegroup in ('HP', 'IFC', 'RF')
		AND	substring(acctno,4,1) = '2'

--Zeroise monthsarrears for status 1 to 5 where months arrears is null -mac 02/04/01
	Update	temp_Summary1_MR 
		set monthsarrears = 0
	From temp_Summary1_MR s,acct a
	Where accttypegroup in ('HP','IFC', 'RF', 'SGR') --#19510,'AST')		-- #13760 Assist Singer account
		AND     currstatus in ('1','2','3','4','5')
		AND	monthsarrears is null

--GO
exec ScriptRunTimeSp 'F','Summary1 view','step10'

exec ScriptRunTimeSp 'S','Summary1 view','step11'

/* UPDATE payamount COLUMN 14/03/02 KEF FR58845 */
	Update	temp_Summary1_MR 
		set payamount=payments
	From temp_Summary1_MR s,vw_Summary1_D_Fintrans vD
	Where s.acctno=vD.acctno
-- 7m 5s Jamaica	
exec ScriptRunTimeSp 'F','Summary1 view','step11'

exec ScriptRunTimeSp 'S','Summary1 view','step12'

-- UAT 26 Need to know what database 'cosacs_archive' is pointing to - JH 27/09/2007

	Update	temp_Summary1_MR  
		set fullydeliveredflag     = 'Y'
	From temp_Summary1_MR s,acct a 
	Where agrmttotal <> 0 
		and	delamount - a.agrmttotal = 0 
		and s.acctno=a.acctno 
		and	not exists (select * from lineitem, stockitem 
		    where lineitem.acctno = s.acctno 
		    and lineitem.quantity > 0 
		    and stockitem.ID = lineitem.ItemID 
                    and stockitem.stocklocn=lineitem.stocklocn 
		    and stockitem.category in (select code from code where category = 'FGC') 
			and s.acctchar4 != '5' 
			and not exists (select * from delivery d 
				where d.acctno = lineitem.acctno 
				and d.ItemID = lineitem.ItemID
				and d.stocklocn = lineitem.stocklocn)) 
   
    
-- 80m 56s Jamaica

exec ScriptRunTimeSp 'F','Summary1 view','step12'

exec ScriptRunTimeSp 'S','Summary1 view','step13'

--GO


exec ScriptRunTimeSp 'F','Summary1 view','step13'

exec ScriptRunTimeSp 'S','Summary1 view','step14'

-- Outstbalcorrnew
--update outstbalcorrnew column
	Update	temp_Summary1_MR 
		set outstbalcorrnew = CASE WHEN a.accttype = 'T' THEN outstbalcorrnew ELSE outstbalcorrnew - intcharges END
	From temp_Summary1_MR s,vw_Summary1_F_Fintrans f, acct a
	where s.acctno=f.acctno
		and s.acctno=a.acctno
		and outstbalcorrnew != 0
-- 1m 42s Jamaica
--GO


exec ScriptRunTimeSp 'F','Summary1 view','step14'

exec ScriptRunTimeSp 'S','Summary1 view','step15'

-- BDW
--update BDW column
	Update	temp_Summary1_MR 
		set BDW = BDW_Total
	From temp_Summary1_MR s,vw_Summary1_Fintrans_BDW f
	Where s.acctno=f.acctno
		
-- 0m 50s Jamaica
--GO
exec ScriptRunTimeSp 'F','Summary1 view','step15'

exec ScriptRunTimeSp 'S','Summary1 view','step16'

	Update	s  
		set cancelledflag='Y'
	From temp_Summary1_MR s,cancellation cn
	where s.acctno=cn.acctno
-- 0m 26s Jamaica
--GO


exec ScriptRunTimeSp 'F','Summary1 view','step16'

exec ScriptRunTimeSp 'S','Summary1 view','step17'
-- Balance due in 12 months and after 12 months
-- modified for instalplan missing jec 30/01/09
	Update	temp_Summary1_MR  
		set baldue12mths=case
			when isnull(i.instalamount,0)=0 then s.outstbalcorr
			when s.outstbalcorr=0 then 0
			when a.currstatus='S' then s.outstbalcorr		-- jec 13/02/08
			when ((isnull(i.instalamount,0) * 12) + a.arrears)> s.outstbalcorr then s.outstbalcorr
			when ((isnull(i.instalamount,0) * 12) + a.arrears)<0 then (isnull(i.instalamount,0) * 12) + a.arrears	-- jec 13/02/08		-- more than 12 months in adv
			else ((isnull(i.instalamount,0) * 12) + a.arrears)
			end,
			baldueafter12mths=case
			when isnull(i.instalamount,0)=0 then 0
			when s.outstbalcorr=0 then 0
			when a.currstatus='S' then 0			-- jec 13/02/08
			when (s.outstbalcorr - ((i.instalamount * 12) + a.arrears))<0 then 0
			when ((isnull(i.instalamount,0) * 12) + a.arrears) <0 then 
						s.outstbalcorr + ABS(((isnull(i.instalamount,0) * 12) + a.arrears))	-- jec 13/02/08	-- more than 12 months in adv 
			else (s.outstbalcorr - ((isnull(i.instalamount,0) * 12) + a.arrears))
			end
	From dbo.temp_Summary1_MR s left outer join dbo.instalplan i on s.acctno=i.acctno
			INNER JOIN acct a on s.acctno = a.acctno
	
-- dateagrmtrevised	
	Update	temp_Summary1_MR  
		set dateagrmtrevised = (select CONVERT(SMALLDATETIME,max(datechange)) from agreementaudit a where temp_Summary1_MR.acctno = a.acctno)
		
-- StatusCodeBand
	
	Update temp_Summary1_MR
		set StatusCodeBand=
		case 
			When accttype = 'S'  then 'Sp'
			WHEN accttype = 'T' then 'StC'
			When currstatus ='S' AND outstbal != 0   then 'St'		-- jec 28/01/09
			When currstatus !='S' AND deliveryflag != 'Y' then 'Nd'
			When (currstatus in ('','U','0') or currstatus is null) 
						AND outstbalcorr > 0 and accttype != 'S' AND deliveryflag = 'Y' then 'Sc'
			When currstatus !='S' AND outstbalcorr <= 0 and accttype != 'S' AND deliveryflag = 'Y' then 'Cr'
			When currstatus IN ('1','2') AND outstbalcorr > 0 AND accttype <> 'S' AND deliveryflag = 'Y' and currstatus!='S'then '1-2'
			When currstatus IN ('3') AND outstbalcorr > 0 AND accttype != 'S' and currstatus!='S' AND deliveryflag = 'Y' then '3'
			When currstatus IN ('4') AND outstbalcorr > 0 AND accttype != 'S' and currstatus!='S' AND deliveryflag = 'Y' then '4'
			When currstatus IN ('5') AND outstbalcorr > 0 AND accttype != 'S' and currstatus!='S' AND deliveryflag = 'Y' then '5'
			When currstatus ='6' AND outstbalcorr > 0 and accttype != 'S' and currstatus!='S' AND deliveryflag = 'Y' then '6'
			When currstatus ='7' AND outstbalcorr > 0 and accttype != 'S' and currstatus!='S' AND deliveryflag = 'Y' then '7'
			When currstatus IN ('8') AND outstbalcorr > 0 and accttype != 'S' and currstatus!='S'  AND deliveryflag = 'Y' then '8'
			When currstatus ='9' AND outstbalcorr > 0 and accttype != 'S' and currstatus!='S' AND deliveryflag = 'Y' then '9'

		End
		from temp_summary1_mr m INNER JOIN acct a on m.acctno=a.acctno
	
			
   INSERT INTO summary1_MR SELECT * FROM temp_summary1_Mr

exec ScriptRunTimeSp 'F','Summary1 view','step17'

exec ScriptRunTimeSp 'S','Summary1 view','step18'
-- check for full deposit paid
	Select	s1.acctno,isnull(sum(transvalue),0)	as 'paidamount',isnull(sum(deposit),0) as 'deposit',  --UAT210
					cast (0.00 as money) as 'uncleared','N' as 'flag'		-- jec 30/10/07
	Into	#tmppa
	From	Summary1_MR s1 LEFT OUTER JOIN fintrans f on s1.acctno = f.acctno	-- jec 30/10/07
					and transtypecode in('PAY','COR','REF','RET','SCX','REB','XFR')
	Group by s1.acctno

--Create index for #tmppa
CREATE CLUSTERED INDEX #ix_tmppa on #tmppa (acctno)
--GO

-- UAT 26 Need to know what database 'cosacs_archive' is pointing to - JH 27/09/2007
--insert into Summary1AcctStatus (acctno,itemno,status)
insert into Summary1AcctStatus (acctno,itemno,status, ItemID)								--IP - 10/08/11 - RI

--select distinct s.acctno,l.itemno,'CAN'			-- UAT205 
select distinct s.acctno,'','CAN',l.ItemID			-- UAT205								--IP - 10/08/11 - RI
	From	Summary1 s INNER JOIN cancellation ca on s.acctno = ca.acctno 
					INNER JOIN lineitem l on s.acctno=l.acctno
	Where s.currstatus!='S' and l.itemtype='S' and s.accttypegroup <> 'PT'
		and not exists(select 1 from Summary1AcctStatus z where s.acctno=z.acctno and l.ItemId=z.ItemId and z.status='CAN')		-- 11/08/11 jec

-- Status - Awaiting Cheque clearance
--insert into Summary1AcctStatus (acctno,itemno,status)
insert into Summary1AcctStatus (acctno,itemno,status, ItemID)								--IP - 10/08/11 - RI
--LW 72269 RM changing to Match APP
--select distinct s.acctno,l.itemno,'CHQ'		-- UAT205
select distinct s.acctno,'','CHQ', l.ItemID		-- UAT205									--IP - 10/08/11 - RI
	From	Summary1 s INNER JOIN #tmppa t on s.acctno = t.acctno and t.uncleared<0
					INNER JOIN lineitem l on s.acctno=l.acctno
	Where	0-t.paidamount<t.deposit
	AND		s.currstatus = 'U'
	AND		s.accttype not in ('S', 'C')
	and not exists(select 1 from Summary1AcctStatus z where s.acctno=z.acctno and l.ItemId=z.ItemId and z.status='CHQ')		-- 11/08/11 jec

 
-- Status - Details Required
--insert into Summary1AcctStatus (acctno,itemno,status)
insert into Summary1AcctStatus (acctno,itemno,status, ItemID)							--IP - 10/08/11 - RI
--LW 72269 RM changing to Match APP
select distinct s.acctno,'','REQ', l.ItemID		-- UAT205								--IP - 10/08/11 - RI
FROM Summary1 s INNER JOIN lineitem l on s.acctno=l.acctno		-- 15/07/10 jec	
where s.currstatus = '0'
	and not exists(select 1 from Summary1AcctStatus z where s.acctno=z.acctno and l.ItemId=z.ItemId and z.status='REQ')		-- 11/08/11 jec


-- Status - Awaiting Referral
--insert into Summary1AcctStatus (acctno,itemno,status)
insert into Summary1AcctStatus (acctno,itemno,status, ItemID)							--IP - 10/08/11 - RI

--select distinct s.acctno,l.itemno,'REF'		-- UAT205
select distinct s.acctno,'','REF', l.ItemID		-- UAT205								--IP - 10/08/11 - RI
	From	Summary1 s INNER JOIN proposal p on s.acctno = p.acctno 
					INNER JOIN lineitem l on s.acctno=l.acctno
	Where s.currstatus!='S' and l.itemtype='S' and s.accttypegroup <> 'PT'
			AND	p.propresult = 'R'
			AND	s.accttype  != 'S'
			AND	s.accttype  != 'C'
			and not exists(select 1 from Summary1AcctStatus z where s.acctno=z.acctno and l.ItemId=z.ItemId and z.status='REF')		-- 11/08/11 jec

-- Status - Awaiting Sanction
--insert into Summary1AcctStatus (acctno,itemno,status)
insert into Summary1AcctStatus (acctno,itemno,status, ItemID)							--IP - 10/08/11 - RI

--select distinct s.acctno,l.itemno,'SAN'		-- UAT205
select distinct s.acctno,'','SAN', l.ItemID		-- UAT205								--IP - 10/08/11 - RI
	From	Summary1 s INNER JOIN proposal p on s.acctno = p.acctno 
					INNER JOIN lineitem l on s.acctno=l.acctno
	Where s.currstatus!='S' and l.itemtype='S' and s.accttypegroup <> 'PT'
			AND	p.propresult = ''                        
			AND	s.accttype  != 'S'
			AND	s.accttype  != 'C'
			and not exists(select 1 from Summary1AcctStatus z where s.acctno=z.acctno and l.ItemId=z.ItemId and z.status='SAN')		-- 11/08/11 jec

-- Status - Deposit/Instal pre-del not paid
--insert into Summary1AcctStatus (acctno,itemno,status)
insert into Summary1AcctStatus (acctno,itemno,status, ItemID)							--IP - 10/08/11 - RI
--LW 72269 RM changing to Match APP

--select distinct s.acctno,l.itemno,'INP'		-- UAT205
select distinct s.acctno,'','INP', l.ItemID		-- UAT205								--IP - 10/08/11 - RI
	From	Summary1 s INNER JOIN #tmppa t on s.acctno = t.acctno 
					INNER JOIN lineitem l on s.acctno=l.acctno and l.itemtype='S'		--UAT208
					INNER JOIN termstype tt on s.termstype=tt.termstype					--UAT208
					--LEFT OUTER JOIN instalplan i on t.acctno = i.acctno		--IP - 02/03/11 - #2811 - CR1090
		Where	(0-t.paidamount<t.deposit or (0-t.paidamount<s.instalamount and tt.instalpredel='Y'))	--UAT208
			and not exists(select 1 from Summary1AcctStatus z where s.acctno=z.acctno and l.ItemId=z.ItemId and z.status='INP')		-- 11/08/11 jec
		--and i.InstalmentWaived = 0 --IP - 02/03/11 - #2811 - CR1090


-- Status - Credit Refusal
--insert into Summary1AcctStatus (acctno,itemno,status)
insert into Summary1AcctStatus (acctno,itemno,status, ItemID)							--IP - 10/08/11 - RI

--select distinct s.acctno,l.itemno,'CRF'		-- UAT205
select distinct s.acctno,'','CRF', l.ItemID		-- UAT205								--IP - 10/08/11 - RI
	From	Summary1 s INNER JOIN proposal p on s.acctno = p.acctno 
					INNER JOIN lineitem l on s.acctno=l.acctno
	Where s.currstatus!='S' and l.itemtype='S' and s.accttypegroup <> 'PT'
			AND	s.accttype  != 'S'
			AND	s.accttype  != 'C'
			AND p.propresult = 'X'
			and not exists(select 1 from Summary1AcctStatus z where s.acctno=z.acctno and l.ItemId=z.ItemId and z.status='CRF')		-- 11/08/11 jec

-- Status - Awaiting D.A.
--insert into Summary1AcctStatus (acctno,itemno,status)
insert into Summary1AcctStatus (acctno,itemno,status, ItemID)							--IP - 10/08/11 - RI

--select distinct s.acctno,l.itemno,'ADA'		-- UAT205
select distinct s.acctno,'','ADA', l.ItemID		-- UAT205								--IP - 10/08/11 - RI
	From	Summary1 s INNER JOIN agreement ag on s.acctno = ag.acctno 
					INNER JOIN lineitem l on s.acctno=l.acctno
					-- LEFT OUTER JOIN instalplan i on s.acctno = i.acctno		--IP - 02/03/11 - #2811 - CR1090
	Where s.currstatus!='S' and l.itemtype='S' and s.accttypegroup <> 'PT'
			AND	ag.holdprop = 'Y'
			and not exists(select 1 from Summary1AcctStatus z where s.acctno=z.acctno and l.ItemId=z.ItemId and z.status='ADA')		-- 11/08/11 jec
			-- AND i.instantcredit!='Y' --IP - 02/03/11 - #2811 - CR1090
			

--declare @sqltext nvarchar(max)
-- Status - Awaiting Delivery Scheduling
insert into Summary1AcctStatus (acctno,itemno,status, ItemID)

select distinct s.acctno,'','SCH', l.ItemID		-- UAT205
			
	From	Summary1 s INNER JOIN lineitem l on s.acctno=l.acctno		-- UAT231
	WHERE (select isnull(COUNT(*),0)
       	 FROM 		SCHEDULE Sh 
       	 WHERE 	Sh.acctno    = s.acctno)>0
	 and not exists(SELECT	* FROM SCHEDULE Sh 
						INNER JOIN branch b	on sh.buffbranchno=b.branchno	--UAT81 jec 15/04/10 
       						WHERE S.acctno = Sh.acctno
       								-- scheduled and Thirdparty
       							and ((l.deliveryprocess='S' and (ISNULL(sh.VanNo,'')='DHL' or thirdpartywarehouse='Y'))
									or (l.deliveryprocess='I' and thirdpartywarehouse!='Y') 
       								) )
     and not exists(select 1 from Summary1AcctStatus z where s.acctno=z.acctno and l.ItemId=z.ItemId and z.status='DAD')		
     and s.currstatus!='S' and l.itemtype='S'	--UAT210

insert into Summary1AcctStatus (acctno,itemno,status, ItemID)										--IP - 10/08/11 - RI

select distinct s.acctno,'','DAD', sh.ItemID		-- UAT205										--IP - 10/08/11 - RI
	From	Summary1 s INNER JOIN lineitem l on s.acctno=l.acctno
						--INNER JOIN schedule sh on l.acctno=sh.acctno and l.itemno=sh.itemno
						INNER JOIN schedule sh on l.acctno=sh.acctno and l.ItemID=sh.ItemID			--IP - 10/08/11 - RI	
	Where s.currstatus!='S' and l.itemtype='S' and s.accttypegroup <> 'PT'
		and not exists(select 1 from Summary1AcctStatus z where s.acctno=z.acctno and l.ItemId=z.ItemId and z.status='DAD')		-- 11/08/11 jec			
			
insert into Summary1AcctStatus (acctno,itemno,status, ItemID)										--IP - 10/08/11 - RI

select distinct s.acctno,'','DCH',sh.ItemID		-- UAT205											--IP - 10/08/11 - RI
	From	Summary1 s INNER JOIN schedule sh on s.acctno=sh.acctno
	Where s.currstatus!='S' and s.accttypegroup <> 'PT'
			and sh.loadno>0			-- jec 30/10/07
			and not exists(select 1 from Summary1AcctStatus z where s.acctno=z.acctno and sh.ItemId=z.ItemId and z.status='DCH')		-- 11/08/11 jec	

-- Status - Awaiting Immediate Pickup of item by Customer
--insert into Summary1AcctStatus (acctno,itemno,status)
insert into Summary1AcctStatus (acctno,itemno,status,ItemID)										--IP - 10/08/11 - RI

--select distinct s.acctno,l.itemno,'AWP'		-- UAT205
select distinct s.acctno,'','AWP',l.ItemID		-- UAT205											--IP - 10/08/11 - RI
	From	Summary1 s INNER JOIN lineitem l on s.acctno=l.acctno 
					   --INNER JOIN schedule sh on l.acctno=sh.acctno and l.itemno=sh.itemno
					     INNER JOIN schedule sh on l.acctno=sh.acctno and l.ItemID=sh.ItemID		--IP - 10/08/11 - RI
	Where s.currstatus!='S' and l.itemtype='S' and s.accttypegroup <> 'PT'
			and l.deliveryprocess= 'I'
			and not exists(select 1 from Summary1AcctStatus z where s.acctno=z.acctno and l.ItemId=z.ItemId and z.status='AWP')		-- 11/08/11 jec	

-- Status - Goods Delivered
--insert into Summary1AcctStatus (acctno,itemno,status)
insert into Summary1AcctStatus (acctno,itemno,status,ItemID)										--IP - 10/08/11 - RI

--select distinct s.acctno,l.itemno,'DEL'		-- UAT205
select distinct s.acctno,'','DEL',l.ItemID		-- UAT205											--IP - 10/08/11 - RI
	From	Summary1 s INNER JOIN lineitem l on s.acctno=l.acctno
	Where s.currstatus!='S' and l.itemtype='S' and s.accttypegroup <> 'PT'
			and l.quantity>0 and l.delqty>0
			and s.fullydeliveredflag = 'Y'
			and not exists(select 1 from Summary1AcctStatus z where s.acctno=z.acctno and l.ItemId=z.ItemId and z.status='DEL')		-- 11/08/11 jec

-- Status - Delivery Note Rescheduled/Reloaded
--insert into Summary1AcctStatus (acctno,itemno,status)
insert into Summary1AcctStatus (acctno,itemno,status,ItemID)										--IP - 10/08/11 - RI

--select distinct s.acctno,l.itemno,'DNR'		-- UAT205
select distinct s.acctno,'','DNR',l.ItemID		-- UAT205											--IP - 10/08/11 - RI
	From	Summary1 s INNER JOIN lineitem l on s.acctno=l.acctno
						inner join schedule sh ON l.acctno = sh.acctno AND l.ItemID = sh.ItemID	AND	l.stocklocn = sh.stocklocn				--IP - 10/08/11 - RI						
						INNER JOIN scheduleremoval sr ON s.acctno = sr.acctno AND l.ItemID = sr.ItemID	AND	l.stocklocn = sr.stocklocn		--IP - 10/08/11 - RI
	WHERE	 l.deliveryprocess = 'S'
	AND		 sh.loadno != 0
	and not exists(select 1 from Summary1AcctStatus z where s.acctno=z.acctno and l.ItemId=z.ItemId and z.status='DNR')		-- 11/08/11 jec
	
-- remove from load
--insert into Summary1AcctStatus (acctno,itemno,status)
insert into Summary1AcctStatus (acctno,itemno,status, ItemID)										--IP - 10/08/11 - RI

--select distinct s.acctno,l.itemno,'RFL'		-- UAT205
select distinct s.acctno,'','RFL',l.ItemID		-- UAT205											--IP - 10/08/11 - RI
	From	Summary1 s INNER JOIN lineitem l on s.acctno=l.acctno
					   inner join schedule sh ON l.acctno = sh.acctno AND l.ItemID = sh.ItemID	AND	l.stocklocn = sh.stocklocn				--IP - 10/08/11 - RI					   
					   INNER JOIN scheduleremoval sr ON s.acctno = sr.acctno AND l.ItemID = sr.ItemID	AND	l.stocklocn = sr.stocklocn		--IP - 10/08/11 - RI
	WHERE	 l.deliveryprocess = 'S'
	AND		 sh.loadno = 0	
	AND isnull(sh.vanno,'') !='DHL'
	and not exists(select 1 from Summary1AcctStatus z where s.acctno=z.acctno and l.ItemId=z.ItemId and z.status='RFL')		-- 11/08/11 jec				

-- #14285 Scheduled status
insert into Summary1AcctStatus (acctno,itemno,status, ItemID)
select distinct s.acctno,'','SCD',l.ItemID
From	Summary1 s INNER JOIN lineitem l on s.acctno=l.acctno		
				inner join lineitembooking lb on l.id = lb.LineItemID
				inner join lineitembookingschedule ls on lb.ID = ls.BookingId
				where ls.quantity !=0

-- #14285 Failed status	
insert into Summary1AcctStatus (acctno,itemno,status, ItemID)
select distinct s.acctno,'','FLD',l.ItemID
From	Summary1 s INNER JOIN lineitem l on s.acctno=l.acctno
				inner join lineitembooking lb on l.id = lb.LineItemID
				inner join lineitembookingfailures lf on lf.OriginalBookingID = lb.ID
				where  lf.Actioned is null	
		

-- Update runtime status
exec ScriptRunTimeSp 'F','Summary1 view','step18'

--alter table Summary1_MR ADD cancelledflag char(1) null
exec ScriptRunTimeSp 'F','Summary1 view','Script'
-- 105m 29s Jamaica - complete script


SET @Return = @@ERROR
--GO

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End 

