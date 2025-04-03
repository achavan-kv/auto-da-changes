-- Author; John Croft  

/*
   This script will generate the Application data to be sent to Scorex 
   The Application table is exported using BCP to ScorexApp.csv

	Requisites: Stored procedures:- App_TransactData
					ScorexAppexport
	Modifications
	John Croft 11/04/05 	Value columns for Madagascar to be muliplied by 5
	John Croft 09/02/06 	Include Reasons 2 to 6 from proposal table
				Accept/Refer scores now on Scorexdata table
    John Croft 29/03/06    Mods for .Net EOD CR781, error checking & save Application
	John Croft 21/02/07	   Fix Arithmetic overflow Application
	jec 15/10/12 - #10298 LW75093 - Copy 6.3 changes to 6.4 (missed when branched) & correct "With" statements
	
*/

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[ScorexApplication]') 
            and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure ScorexApplication
Go

Create Procedure ScorexApplication

        @return int OUTPUT
as 

SET NOCOUNT ON

--DBCC TRACEON (1204)

DECLARE @status integer

SET 	@return = 0			--initialise return code
SET 	@status = 0

IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
	   WHERE  table_schema = 'dbo' and table_name = 'Application')
    truncate table .dbo.Application        -- drop table Application
else
-- Create Application table 
Begin
create table .dbo.Application
(
CountryCode char(2),	-- id10
Region char(3),
BranchNo smallint,
ProposalDate char(8),
Acctno char(12),
AppType char(6),
AppNumber varchar(12),
AccountType char(1),
Staff char(1),
PrivClub char(1),
RInd char(1),
WInd char(1),
NCInd char(1),
GuaranteeInd char(1),
RepoInd char(1),
RefinanceCard char(1),
Addto char(1),
KLetterCount smallint,
AgrmtTotal numeric(12,2),
DepositPaid char(1),
DepositPct numeric(5,2),
NumInstal numeric(2,0),
Instalment numeric(12,2),
InstIncomePct numeric(5,2),
Termstype varchar(2),
Paymethod varchar(1),
--GCode numeric(12,2),
NoAccounts smallint,
AgrtotAccts numeric(12,2),
BalanceAccts numeric(12,2),
TotalArrears numeric(12,2),
OtherPayments numeric(12,2),
OtherCourtsInst numeric(12,2),
RFSpendLimit numeric(12,2),
OldRFSpendLimit numeric(12,2),
AvailSpend numeric(12,2),
--ProductCode varchar(8),
ProductCat smallint,
Itemcount smallint,
LocationOfGoods smallint,	--Oct 2004
CustomerId varchar(20),
DateOfBirth char(8),
Age smallint,
Title varchar(25),
MaritalStat varchar(1),
Sex varchar(1),
Ethnicity varchar(1),
Nationality char(4),
Dependants smallint,
PreviousCustomer char(1),
SpouseCount smallint,
RelativeCount smallint,
TimeCurrentAddr smallint,
ResidentStatus varchar(1),
PropertyType char(4), 		-- Sept 2004
PostCode varchar(10),
PostalArea varchar(10),
--District varchar(10),
TimePrevAddr smallint,
PrevResStatus varchar(1),
HomeTel varchar(1),
WorkTel varchar(1),
MobileTel varchar(1),
PrevAddr varchar(1),
TimeCurEmploy smallint,
EmployStat varchar(1),
Occupation varchar(2),
TimePrevEmploy smallint,
JobCount smallint,
--SpouseEmpStat varchar(1),	-- Sept 2004 error
--SpouseOccupation varchar(2),	-- Sept 2004 error
PayFreq varchar(1),
SpouseEmpStat varchar(1),	-- Sept 2004
SpouseOccupation varchar(2),	-- Sept 2004
TimeatBank smallint,
BankCode varchar(6),
BankAccInd varchar(1),
BankAcctCode varchar(1),
MonthlyIncome numeric(12,2),	-- Oct 2004  
DisposeIncome numeric(12,2),	-- Oct 2004 was MonthlyIncome 
MonthlyRent numeric(12,2),
JointMnthlyInc numeric(12,2),
WorstCurrStat varchar(1),
WorstCurrStatEver varchar(1),
WorstSettStat varchar(1),
WorstSettStatEver varchar(1),
WeightAveCurr varchar(1),
WeightAveSett varchar(1),
NoSettAccts smallint,
StatusLgeSettAcct varchar(1),
LgeAgrSize varchar(1),
LgeSettAgrSize varchar(1),
DateLastDel char(8),
TimeLastDel smallint,
Bankruptcies smallint,       -- now selected from creditbureau
TimeLawsuit smallint,        -- now selected from creditbureau    
NoLawsuits smallint,        -- now selected from creditbureau    
AcceptScore smallint,
ReferScore smallint,
ScoreCardNo smallint,
--InitialScore smallint,
FinalScore smallint,
--InitDecision varchar(1),
FinalDecision varchar(1),
ReasonCode1 varchar(2),
ReasonCode2 varchar(2),
ReasonCode3 varchar(2),
ReasonCode4 varchar(2),
ReasonCode5 varchar(2),
ReasonCode6 varchar(2),
--ReasonCode7 varchar(2),
--ReasonCode8 varchar(2),
--ReasonCode9 varchar(2),
--ReasonCode10 varchar(2),
Overide varchar(1),
RefEmployeeNo INT,
CreditPercentUplift TINYINT  

)

End     -- Begin

-- Create Save Application table if does not exist and add extra column
-- This will improve data recovery if errors occur
IF not EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
	   WHERE  table_schema = 'dbo' and table_name = 'zz_SaveApplication')
    Begin
        select *,convert(datetime,null) as ExtractDate into .dbo.zz_SaveApplication
        from Application

    End
--go
--LW 71155 remove duplicates from proposal - latest prop for last month
delete from proposal 
where exists
(select 'x' from proposal p2
where proposal.acctno=p2.acctno
and proposal.custid=p2.custid
and proposal.dateprop < p2.dateprop
and dateadd(month,1,proposal.dateprop) >p2.dateprop)


-- fix to stop Arithmetic overflow error		jec 08/08/07
Update scorexdata
set instpcincome=999
where instpcincome>999

Update scorexdata
set depositpcent=999
where depositpcent>999
-- end fix 
insert into .dbo.Application 
(
CountryCode,
Region,
BranchNo,
ProposalDate,
Acctno,
AppType,
AppNumber,
AccountType,
Staff,
PrivClub,
RInd,
WInd,
NCInd,
GuaranteeInd,
RepoInd,
RefinanceCard,
Addto,
KLetterCount,
AgrmtTotal,
DepositPaid,
DepositPct,
NumInstal,
Instalment,
InstIncomePct,
Termstype,
Paymethod,
--GCode numeric(12,2),
NoAccounts,
AgrtotAccts,
BalanceAccts,
TotalArrears,
OtherPayments,
OtherCourtsInst,
RFSpendLimit,
OldRFSpendLimit,
AvailSpend,
--ProductCode varchar(8),
ProductCat,
Itemcount,
LocationOfGoods,	-- Oct 2004
CustomerId,
DateOfBirth,
Age,
Title,
MaritalStat,
Sex,
Ethnicity,
Nationality,
Dependants,
PreviousCustomer,
SpouseCount,
RelativeCount,
TimeCurrentAddr,
ResidentStatus,
PropertyType,		-- Sept 2004
PostCode,
PostalArea,
--District varchar(10),
TimePrevAddr,
PrevResStatus,
HomeTel,
WorkTel,
MobileTel,
PrevAddr,
TimeCurEmploy,
EmployStat,
Occupation,
TimePrevEmploy,
JobCount,
--SpouseEmpStat varchar(1),	-- Sept 2004 error
--SpouseOccupation varchar(2),	-- Sept 2004 error
PayFreq,
SpouseEmpStat,		-- Sept 2004
SpouseOccupation,	-- Sept 2004
TimeatBank,
BankCode,
BankAccInd,
BankAcctCode,
MonthlyIncome,		-- Oct 2004
DisposeIncome,		-- Oct 2004 was MonthlyIncome 
MonthlyRent,
JointMnthlyInc,
WorstCurrStat,
WorstCurrStatEver,
WorstSettStat,
WorstSettStatEver,
WeightAveCurr,
WeightAveSett,
NoSettAccts,
StatusLgeSettAcct,
LgeAgrSize,
LgeSettAgrSize,
DateLastDel,
TimeLastDel,
Bankruptcies,        -- now selected from creditbureau
TimeLawsuit,        -- now selected from creditbureau
NoLawsuits,        -- now selected from creditbureau
AcceptScore,
ReferScore,
ScoreCardNo,
--InitialScore smallint,
FinalScore,
--InitDecision,
FinalDecision,
ReasonCode1,
ReasonCode2,
ReasonCode3,
ReasonCode4,
ReasonCode5,
ReasonCode6,
Overide,
RefEmployeeNo,
CreditPercentUplift 

)

Select distinct s.Countrycode,
b.region,
s.branchno,
replace (convert (varchar, s.Dateprop,105),'-',''), --ddmmyyy s.Dateprop,
s.Acctno,
p.A2Relation,
s.appnumber, -- may 2005 '999999', --null, --AppNumber,--??
a.Accttype,
s.Staffacct,
s.PrivClub,
s.RIndicset,
s.WIndicset,
s.NCIndicset,
s.GuarIndic,
s.RepoIndic,
RefinanceCard=case
	when s.countrycode='S' and a.termstype in('FG','FN','FP','RF') then 'Y' else 'N'
	end,
s.Addtoflag,
s.KLettrCount,
-- s.AgrmtTotal,
AgrmtTotal=case		-- Madagascar
	when s.countrycode='C' then s.AgrmtTotal * 5 else s.AgrmtTotal
	end,
t.DepositPaid,
DepositPct=case			-- error occurs when >999	(jec 21/02/07	68809)
	when s.DepositPcent >0 and s.DepositPcent <= 999 then s.DepositPcent else 0
	end,
s.Instalno,
--s.Instalamount,
Instalment=case		-- Madagascar
	when s.countrycode='C' then s.Instalamount * 5 else s.Instalamount
	end,
InstpcIncome=case
	when s.InstpcIncome >0 and s.InstpcIncome<100 then s.InstpcIncome else 0
	end,
t.Termstype,
s.Paymethod,
--GCode numeric(12,2),
s.NoofAccts,
--s.AgrtotAccts,
AgrtotAccts=case		-- Madagascar
	when s.countrycode='C' then s.AgrtotAccts * 5 else s.AgrtotAccts
	end,
--s.BalofAccts,
BalanceAccts=case		-- Madagascar
	when s.countrycode='C' then s.BalofAccts * 5 else s.BalofAccts
	end,
--s.TotArrears,
TotalArrears=case		-- Madagascar
	when s.countrycode='C' then s.TotArrears * 5 else s.TotArrears
	end,
--s.OtherPmnts,
OtherPayments=case		-- Madagascar
	when s.countrycode='C' then s.OtherPmnts * 5 else s.OtherPmnts
	end,
--s.OthCrtInstal,
OtherCourtsInst=case		-- Madagascar
	when s.countrycode='C' then s.OthCrtInstal * 5 else s.OthCrtInstal
	end,
--c.RFcreditLimit,
RFSpendLimit=case		-- Madagascar
	when s.countrycode='C' then c.RFcreditLimit * 5 else c.RFcreditLimit
	end,
null, --c.OldRFcreditlimit,
null, --c.AvailableSpend,
--ProductCode varchar(8),
s.bigitemcat,
s.Itemcount,
0,			-- Oct 2004 LocationOfGoods 0=Home >0=delivery addr
p.Custid,
replace (convert (varchar, s.DateBorn,105),'-',''),-- ddmmyyyy s.DateBorn,
s.Age,
s.Title,
s.MaritalStat,
s.Sex,
s.Ethnicity,
p.Nationality,
s.Dependants,
s.PrevCustInd,
s.SpouseCount,
s.RelCount,
s.TimeCurrAddr,
s.CurrResStat,
ad.PropType,		-- Sept 2004
s.PostCode,
s.PostalArea,
--District varchar(10),
s.TimePrevAddr,
s.PrevResStat,
s.HomeTel,
s.WorkTel,
s.Mobile,
s.PrevAddrInd,
s.TimeCurrEmpl,
s.EmpmtStatus,
s.WorkType,
s.TimePrevEmpl,
s.JobCount,
--null, --SpouseEmpStat,	-- Sept 2004 error 
--s.SpouseOccupation,		-- Sept 2004 error
s.PayFreq,
null, --SpouseEmpStat,	-- Sept 2004
s.SpouseOccupation,	-- Sept 2004
s.TimeBank,
s.BankCode,
s.BankAcctInd,
s.BankAcctCode,
--p.MthlyIncome,		-- Oct 2004
MonthlyIncome=case		-- Madagascar
	when s.countrycode='C' then p.MthlyIncome * 5 else p.MthlyIncome
	end, 
--s.MthlyIncome,
DisposeIncome=case		-- Madagascar
	when s.countrycode='C' then s.MthlyIncome * 5 else s.MthlyIncome
	end, 		
--s.MthlyRent,
MonthlyRent=case		-- Madagascar
	when s.countrycode='C' then s.MthlyRent * 5 else s.MthlyRent
	end, 	
--s.JntMthIncome,
JointMnthlyInc=case		-- Madagascar
	when s.countrycode='C' then s.JntMthIncome * 5 else s.JntMthIncome
	end, 
s.WrstCurrStat,
null, --WorstCurrStatEver, SP App_TransactData
s.WrstSettStat,
null, --WorstSettStatEver, SP App_TransactData
null, --WeightAveCurr, SP App_TransactData
null, --WeightAveSett, SP App_TransactData
s.SettledAccts,
null, --StatusLgeSettAcct, SP App_TransactData
s.AgrmtSizCode,
s.SetAgrmtSiz,
replace (convert (varchar, s.DateLastDel,105),'-',''),-- ddmmyyyy s.DateLastDel,
s.TimeLastDel,
p.Bankruptcies,
p.TimesinceLawsuit,
p.NumberLawsuits,
s.AcceptScore,	-- 09/02/06
s.ReferScore,	-- 09/02/06
s.ScoreCardNo,
--InitialScore smallint,
s.Points,
-- init decision
s.Decision,
--r.reflresult,	-- Final decision
p.Reason,
p.Reason2,
p.Reason3,
p.Reason4,
p.Reason5,
p.Reason6,
s.Override,
s.RefEmpeeNo,
p.CreditPercentUplift 
from scorexdata s,proposal p,
	termstype t,acct a,customer c,branch b,custacct ca,
	custaddress ad		-- Sept 2004

where s.acctno=ca.acctno
and s.acctno=a.acctno
and s.acctno=p.acctno
and ca.custid=p.custid
and ca.hldorjnt='H' -- Holder
and a.termstype=t.termstype
and c.custid=ca.custid
and c.custid=ad.custid 		-- Sept 2004
and ad.addtype='H'		-- Sept 2004
and ad.datemoved is null	-- Sept 2004
and b.branchno=s.branchno
and s.decision in('A','X') -- Accepted,Rejected

order by s.acctno
-- Update Application with Worst status codes etc.
----------------------------------
-- Generate temp data
----------------------------------
  DECLARE @NumMonthsStatustocheck INT 
  
  SELECT @NumMonthsStatustocheck = CONVERT(INT,value) FROM CountryMaintenance 
  WHERE codename ='WorstStatusPeriod'  
  
  IF @NumMonthsStatustocheck = 0 
	SET @NumMonthsStatustocheck = 500	

SELECT ca.custid, a.AcctNo, a.CurrStatus, a.AgrmtTotal, a.OutStBal,
            MAX(isnull(s.StatusCode,1)) AS StatusCode,
           s.DateStatChge,
           CAST(0.0 AS FLOAT) AS Days
    INTO   #temp_AvgStatus
    FROM   [application] ap
			inner join CustAcct ca 
				on ca.custid = ap.Customerid 
				AND ca.HldOrJnt     = 'H'
			inner join acct a 
				on a.acctno = ca.acctno
			LEFT outer join Agreement ag 
				on ca.acctno = ag.acctno	-- UAT153 
			inner join status s 
				on s.AcctNo = A.AcctNo 
				and s.statuscode not in ('S','0','U')
				AND  s.datestatchge > DATEADD(MONTH,-@NumMonthsStatustocheck,GETDATE())        
	WHERE ISNULL(ag.DeliveryFlag,'Y') = 'Y'	-- UAT153
    AND    a.AcctType     != 'C'        -- CR396 Exclude CASH accounts
    GROUP BY ca.custid, a.AcctNo, a.CurrStatus,  a.AgrmtTotal, a.OutStBal ,
             s.DateStatChge
	
    
    -- remove accounts which were ealier than number of months to check and have no balance so should not be included as not current
    delete FROM #temp_AvgStatus WHERE datestatchge IS NULL 
    AND outstbal < 1
    
	-- need to insert previous status from just before date as within period
	INSERT INTO #temp_AvgStatus ( custid, acctno ,
		StatusCode,
		DateStatChge,
		Days,
		currstatus 
	) SELECT ca.custid, s.acctno,s.statuscode,s.datestatchge,0  ,a.currstatus
	FROM status s
	inner join acct a on a.acctno = s.acctno
	inner join custacct ca on ca.acctno = a.acctno and hldorjnt = 'H'
	WHERE s.datestatchge = (SELECT MAX(ss.datestatchge) FROM status ss 
	WHERE ss.acctno = s.acctno AND s.datestatchge < DATEADD(MONTH,-@NumMonthsStatustocheck,GETDATE())
	and ss.statuscode not in ('S','0','U'))
	AND EXISTS (SELECT * FROM #temp_AvgStatus t WHERE t.acctno= s.acctno)
	
    
    --updating this for those accounts which are missing a record from the status table
    update #temp_AvgStatus 
    set DateStatChge = datelastpaid
    from acct where acct.acctno =#temp_AvgStatus.acctno and DateStatChge is null

        -- Calculate the number of days in each Status Code
        -- The last Status Code is up to today unless it is Settled 'S'
        -- Each Settled Status will be left as zero days so it is ignored
    
        UPDATE #temp_AvgStatus
        SET Days = (SELECT DATEDIFF(Day,#temp_AvgStatus.DateStatChge,ISNULL(MIN(b.DateStatChge),GETDATE()))
                    FROM   #temp_AvgStatus b
                    WHERE  b.AcctNo       = #temp_AvgStatus.AcctNo
                    AND    b.DateStatChge > #temp_AvgStatus.DateStatChge)
        WHERE StatusCode <> 'S'

        -- Only the last 18 months (546 days) excluding settled periods 
        -- will be used for the average
    
        UPDATE #temp_AvgStatus
        SET Days = ISNULL((SELECT 546 - SUM(b.Days)
                           FROM   #temp_AvgStatus b
                           WHERE  b.AcctNo       = #temp_AvgStatus.AcctNo
                           AND    b.DateStatChge > #temp_AvgStatus.DateStatChge
                           HAVING (546 - SUM(b.Days)) < #temp_AvgStatus.Days),Days)


;with avgstat (custid, acctno, statuscode)
as (select a.custid, a.acctno, ISNULL(MAX(b.StatusCode),0)
from  #temp_AvgStatus a
inner join #temp_avgstatus b
	on a.custid = b.custid
	and a.acctno != b.acctno
WHERE  b.CurrStatus = 'S'
group by a.custid, a.acctno )
      
update [Application]
set WorstSettStatEver=isnull(a.statuscode, 0) -- Highest status ever (during period). 
FROM   avgstat a
WHERE  [Application].CustomerId = a.custid
		and [Application].acctno = a.acctno

;with avgstat (custid, acctno, statuscode)
as (select a.custid, a.acctno, ISNULL(MAX(b.StatusCode),0)
from  #temp_AvgStatus a
inner join #temp_avgstatus b
	on a.custid = b.custid
	and a.acctno != b.acctno
WHERE  b.CurrStatus = 'S'
	AND b.datestatchge = (SELECT MAX(datestatchge) FROM #temp_AvgStatus t 
						WHERE t.acctno= b.acctno
						AND t.currstatus = 'S')
group by a.custid, a.acctno )


update [Application]
set WorstSettStat=isnull(a.statuscode, 0) -- Highest status when settled (during period). 
FROM   avgstat a
WHERE  [Application].CustomerId = a.custid
		and [Application].acctno = a.acctno




;with avgstat (custid, acctno, statuscode)
as (select a.custid, a.acctno, ISNULL(MAX(b.StatusCode),0)
from  #temp_AvgStatus a
inner join #temp_avgstatus b
	on a.custid = b.custid
	and a.acctno != b.acctno
WHERE  b.CurrStatus = 'S'
	AND b.AgrmtTotal = (SELECT MAX(AgrmtTotal)
                               FROM   #temp_AvgStatus t
                               WHERE  t.acctno = b.acctno
								and CurrStatus = 'S')
group by a.custid, a.acctno )


update [Application]
set StatusLgeSettAcct=isnull(a.statuscode, 0) -- Highest status of largest settled account
FROM   avgstat a
WHERE  [Application].CustomerId = a.custid
		and [Application].acctno = a.acctno


;with avgstat (custid, acctno, statuscode, currstatus)
as (select a.custid, a.acctno, ISNULL(MAX(b.StatusCode),0), ISNULL(MAX(b.CurrStatus),0)
from  #temp_AvgStatus a
inner join #temp_avgstatus b
	on a.custid = b.custid
	and a.acctno != b.acctno
WHERE  b.CurrStatus != 'S'
group by a.custid, a.acctno )


update [Application]
set WorstCurrStatEver=isnull(a.statuscode, 0), 
	WorstCurrStat = isnull(a.currstatus, 0)
FROM   avgstat a
WHERE  [Application].CustomerId = a.custid
		and [Application].acctno = a.acctno


;WITH cte (custid, acctno, statuscode)
	as (select  a.custid, a.acctno, ISNULL(ROUND(SUM(b.StatusCode * b.[Days]) / SUM(b.[Days]),1),0)
		from  #temp_AvgStatus a
		inner join #temp_avgstatus b
			on a.custid = b.custid
			and a.acctno != b.acctno
		WHERE  b.CurrStatus != 'S'
			and ISNUMERIC(b.StatusCode) = 1
			AND    b.[Days] > 0
		group by a.custid, a.acctno ),
avgstat (custid, acctno, statuscode)
as (select custid, acctno, max(statuscode)
	from cte
	group by custid, acctno
)

update [Application]
set WeightAveCurr=cast(isnull(a.statuscode, 0) as int)
FROM   avgstat a
WHERE  [Application].CustomerId = a.custid
		and [Application].acctno = a.acctno

;WITH cte (custid, acctno, statuscode)
	as (select a.custid, a.acctno, ISNULL(ROUND(SUM(b.StatusCode * b.[Days]) / SUM(b.[Days]),1),0)
		from  #temp_AvgStatus a
		inner join #temp_avgstatus b
			on a.custid = b.custid
			and a.acctno != b.acctno
		WHERE  b.CurrStatus = 'S'
			and ISNUMERIC(b.StatusCode) = 1
			AND    b.[Days] > 0
		group by a.custid, a.acctno  ),
avgstat (custid, acctno, statuscode)
as (select custid, acctno, max(statuscode)
	from cte
	group by custid, acctno
)

update [Application]
set WeightAveSett=cast(isnull(a.statuscode, 0) as int)
FROM   avgstat a
WHERE  [Application].CustomerId = a.custid
		and [Application].acctno = a.acctno

    
-- Update Application with Spouse Employment Status
IF @status = 0 
    Begin
 
 ;with spouseempstat(acctno, empmtstatus)
 as (

select a.acctno,max(empmtstatus)
from employment e,custacct ca,application a
where a.acctno=ca.acctno
and hldorjnt='S'
and ca.custid=e.custid
and dateleft is null
group by e.custid,a.acctno)

update a
set SpouseEmpStat=s.empmtstatus
from [Application] a
inner join spouseempstat s
	on s.acctno = a.acctno

SET @status =@@error

End    -- Begin


-- Update LocationOfGoods
IF @status = 0 
    Begin

;with custadd (custid, noadd)
as (


select custid,Count(*) 
        from custaddress ad where 
        addtype in ('D','D1', 'D2','D3')
        and   (datemoved is null or datemoved =N'1-Jan-1900')
	group by custid)
	
update a
set LocationOfGoods=s.noadd
from [Application] a
inner join custadd s
	on s.custid = a.CustomerId

SET @status =@@error
	
End    -- Begin

-- creditbureau data -- Only used in Singapore

update application

    set Bankruptcies = b.Bankruptcies,
        TimeLawsuit = b.LawsuitTimeSinceLast,
        NoLawsuits = b.Lawsuits

from application a, creditbureau b
where a.customerid=b.custid


-- change ownership of table - testing only 
-- sp_changeobjectowner 'Application', 'dbo'


-- replace "," in postcode and postalarea with "." to avoid corruption of .csv file

update Application set postcode= replace(postcode,',','.'), postalarea= replace(postalarea,',','.')
where postcode like('%,%') or postalarea like('%,%')

SET @status = @@error

IF @status = 0 
    Begin
-- Save Application data
    insert into .dbo.zz_SaveApplication
    select CountryCode, Region, BranchNo, ProposalDate, Acctno, AppType, 
    AppNumber, AccountType, Staff, PrivClub, RInd, WInd, NCInd, GuaranteeInd, RepoInd, 
    RefinanceCard, Addto, KLetterCount, AgrmtTotal, DepositPaid, DepositPct, NumInstal, 
    Instalment, InstIncomePct, Termstype, Paymethod, NoAccounts, AgrtotAccts, BalanceAccts,
     TotalArrears, OtherPayments, OtherCourtsInst, RFSpendLimit, OldRFSpendLimit, AvailSpend, 
     ProductCat, Itemcount, LocationOfGoods, CustomerId, DateOfBirth, Age, Title, MaritalStat,
      Sex, Ethnicity, Nationality, Dependants, PreviousCustomer, SpouseCount, RelativeCount,
       TimeCurrentAddr, ResidentStatus, PropertyType, PostCode, PostalArea, TimePrevAddr, 
       PrevResStatus, HomeTel, WorkTel, MobileTel, PrevAddr, TimeCurEmploy, EmployStat, 
       Occupation, TimePrevEmploy, JobCount, PayFreq, SpouseEmpStat, SpouseOccupation, 
       TimeatBank, BankCode, BankAccInd, BankAcctCode, MonthlyIncome, DisposeIncome, MonthlyRent,
        JointMnthlyInc, WorstCurrStat, WorstCurrStatEver, WorstSettStat, WorstSettStatEver, 
        WeightAveCurr, WeightAveSett, NoSettAccts, StatusLgeSettAcct, LgeAgrSize, LgeSettAgrSize, 
        DateLastDel, TimeLastDel, Bankruptcies, TimeLawsuit, NoLawsuits, AcceptScore, ReferScore, 
        ScoreCardNo, FinalScore, FinalDecision, ReasonCode1, ReasonCode2, ReasonCode3, ReasonCode4, 
        ReasonCode5, ReasonCode6, Overide, RefEmployeeNo,getdate(),CreditPercentUplift from Application

-- Save extracted Scorexdata before deleting
-- now saving to zz_ScorexData2
IF not EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
	   WHERE  table_schema = 'dbo' and table_name = 'zz_SaveScorexdata3')
-- create the table if not exist - zz_SaveScorexdata1 for new column appnumber
select * into .dbo.zz_SaveScorexdata3
from Scorexdata
where decision in('A','X')
-- else insert new data
Else
insert into .dbo.zz_SaveScorexdata3
select * from Scorexdata
where decision in('A','X')

-- Delete data from Scorexdata 
delete from scorexdata
where decision in('A','X')
END

IF @@error != 0
	BEGIN
		SET @return = @@error
	END

Go

-- End End End End End End End End End End End End End End End End End End End End End End End End 