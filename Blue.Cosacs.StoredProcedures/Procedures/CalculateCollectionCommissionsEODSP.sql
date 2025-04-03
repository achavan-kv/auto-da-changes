SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (select * from sysobjects where name ='CalculateCollectionCommissionsEODSP')
drop procedure CalculateCollectionCommissionsEODSP
go

create  procedure [dbo].[CalculateCollectionCommissionsEODSP] -- 3,0 

/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : CalculateCollectionCommissionsEODSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : EOD Calculate Collection Commissions for Individuals / Team (Worklist)
--				  based on rules setup in the Collection Commission Maintenance screen.
-- Author       : Ilyas Parker
-- Date         : 15 June 2010
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 15/06/10	 IP  Creation.
-- 09/07/10  IP  UAT(1094) UAT 5.2 - Added distinct to prevent duplicate rows returned as previously was returning a row
--				 for every action for the rule. The incorrect Arrears value prior to the action was returned. Added a check 
--				 that the datefrom should be < the date action taken and not the beginning of the day the action was taken.
-- 09/07/10  IP  UAT(1095) UAT 5.2 - Fix divide by zero error.
-- 12/07/10  jec UAT1096 Incorrect Caller Commission
-- 15/07/10  IP  Added change to give commission based on percentage of a worklist completed.
-- 16/07/10  IP  UAT(1104) UAT5.2 - No Of Calls commission generated but with incorrect no of calls.
-- 19/07/10  IP  UAT(1110) UAT5.2 - Payments and FEE's were not being aggregated if more than one payment made.
-- 20/07/10  IP  UAT(1094) UAT5.2 - Subtracted FEE value from payment value for AmountCollectedTot.
-- 21/07/10  IP/JC - Include entries in the final CollectionCommn table for rules that an employee did not qualify on.
-- 11/11/10  jec UAT1129 "Max Days for Caller Commissions" Country Parameter
-- 19/11/10  jec UAT1129 additional checking
-- 21/02/11  IP  Sprint 5.11 - #2382 - CR1215 - Limit running Commissions EOD - Prevent the EOD job from running
--				 if it has been less that 7 days since the last run.
-- 25/02/11  IP  Sprint 5.11 - #2382 - CR1215 - Limit running Commissions EOD - Removed timestamp from the dates
--				 when comparing dates to check if the EOD should be run.
************************************************************************************************************/
--Parameters
@runNo int,
@return int out

as

set @return = 0

declare @LastRunDate datetime,
		@MaxComDate datetime,	--UAT1129 jec
		@CurrentRunDate datetime,
		@daysSinceLastRun int,
		@maxdays INT,
		@LastRunNo INT,		--UAT1129 jec
		@PrevMaxDays INT		

select @maxdays=Value*-1 from CountryMaintenance where codename='MaxDaysCallerComm'		--UAT1129 jec
--Select the datestart of the current Collection Commissions run
set @CurrentRunDate = (select datestart from interfacecontrol
						where interface = 'COLLCOMMNS'
						and runno = @runNo)
						
--Select the date of the last successfull run
set @LastRunDate = (select datestart from interfacecontrol i
						where i.interface = 'COLLCOMMNS'
						and i.datestart = (select max(i1.datestart) from interfacecontrol i1
											where i1.interface = i.interface
											and i1.datestart < @CurrentRunDate
											and result = 'P'))

--IP - 21/02/11 - Sprint 5.11 - #2382
--IP - 25/02/11 - Sprint 5.11 - #2382 - remove timestamp and only check on the date.
--If the job has been run less than 7 days ago, prevent from running and display a message for the run.
IF(convert(varchar(10),@LastRunDate,111) > convert(varchar(10),dateadd(d, -7, @CurrentRunDate),111))
BEGIN
			Declare @ProcessError varchar(100), @NoDays int
			
			set @NoDays = (7 - datediff(d, convert(varchar(10),@LastRunDate,111), convert(varchar(10),@CurrentRunDate,111))) --Work out number of days remaining until next valid run
			
			set @ProcessError = 'It has not been 7 days since the last successfull run. This job cannot be run for another ' + cast(@NoDays as varchar(2)) + ' days'
			INSERT INTO interfaceerror(interface,runno,errordate,severity,errortext)
			VALUES('COLLCOMMNS',@runno,GETDATE(),'W',@ProcessError)
			raiserror (@ProcessError,16,1)
			RETURN
END
-- Select previous successful RunNo  UAT1129 jec
select @LastRunNo= RunNo from interfacecontrol i
						where i.interface = 'COLLCOMMNS'
						and i.datestart = (select max(i1.datestart) from interfacecontrol i1
											where i1.interface = i.interface
											and i1.datestart < @CurrentRunDate
											and result = 'P')
-- Select previous MaxDays value UAT1129 jec
select @PrevMaxDays= MaxDays * -1 from CollectionCommnMaxDays 
	Where RunNo=@LastRunNo
-- Save parameter used in current run 	UAT1129 jec
insert into CollectionCommnMaxDays (RunNo,MaxDays)
select @RunNo,Value 
from CountryMaintenance where codename='MaxDaysCallerComm'

-- temp table for checking previous Commission date ranges
select r2.runno,r2.maxdays,ir2.datestart as rundate,DATEADD(dd,-r.maxdays,ir.datestart) as datefrom, 
	DATEADD(dd,-r2.maxdays,ir2.datestart) as dateto
into #Commdaterange
from CollectionCommnMaxDays r INNER JOIN interfacecontrol ir on r.runno=ir.runno and ir.interface='COLLCOMMNS',
CollectionCommnMaxDays r2 INNER JOIN interfacecontrol ir2 on r2.runno=ir2.runno and ir2.interface='COLLCOMMNS'
where r2.runno = r.runno+1
-- max date of commissions previously processed
select @MaxComDate =MAX(dateto) from #Commdaterange where runno<@runno
-- if previous selection date to >current selection date from adjust Previous max days 
if @maxComDate>dateadd(dd,@PrevMaxDays,@LastRunDate)
	set @PrevMaxDays=DATEDIFF(d,@LastRunDate,@maxComDate)
												
set @daysSinceLastRun = (select DATEDIFF(d, @LastRunDate, @CurrentRunDate))

--IP - 09/07/10 - UAT(1095) UAT5.2
set @daysSinceLastRun = case when @daysSinceLastRun = 0 then 1 else @daysSinceLastRun end


declare @CollectionCommn table
(
	RuleID int,
	RuleName varchar(50),
	CommissionType char(1),
	EmpeeNo int,
	EmpeeType char(1),
	Worklist varchar(6),
	NoOfCalls int,
	ArrearsTot money,
    ArrearsPaidTot money,
	AmountCollectedTot money,
	FeeTot money
	
)

--15/07/10 
/***Table to hold for each Employee, Worklist employee worked on, the total number of accounts in the
 worklist since the last run, and the number of accounts actioned in a worklist by an employee***/

declare @CallsByWorklist table
(	
	Worklist varchar(6),
	EmpeeNo int,
	EmpeeType char(1),
	TotAcctsInWorkList int,
	NoAcctsActioned int
)

--15/07/10
/***Table to hold for each Employee Type and worklist, the total number of accounts in the worklist 
since the last run, and the total number of accounts that were actioned in a worklist***/

declare @CallsByWorklistTeam table
(
	EmpeeType char(1),
	Worklist varchar(6),
	TotAcctsInWorkList int,
	TotNoAcctsActioned int
)


/*******************************************Base table select*********************************************************************************************/

/*Do an initial select, selecting accounts where there has been a following action since the last run
 and a previous action to this following action.*/

							  

if not exists ( select 'x' from sys.tables where name = 'CollectionCommnAccts_init')
begin
select top 0 * into CollectionCommnAccts_init from CollectionCommnAccts
create index cci on CollectionCommnAccts_init (acctno, previousactiondate, followingactiondate)
end
else truncate table CollectionCommnAccts_init
-- get all actions for empeetypes listed where either 
--previous action is since last run or last action is since 
--last run and previous action is before last run
insert into CollectionCommnAccts_init (RunDate,  AcctNo, EmpeeNo, EmpeeType, Worklist, PreviousAction, 
								  PreviousActionDate,  FollowingAction,  FollowingActionDate,
								  paymentvalue,feevalue,balancebefore,arrearsbefore,monthsinarrearsbefore,percentofarrearspaid,paymentdayssinceaction)
select
	   @CurrentRunDate, 	   bp.acctno,  bp.empeeno, ur.RoleId, isnull(w.Worklist,''),  bp.code,
	   bp.dateadded,  bf.code,   bf.dateadded,
	   0,0,0,0,0,0,0
from bailaction bp -- following action
    left outer join bailaction bf on bp.acctno = bf.acctno   AND bp.dateadded = (select max(dateadded) from bailaction bx where bx.acctno=bf.acctno and bx.dateadded <bf.dateadded) 
	left join CMWorklistsAcct w on bp.acctno = w.acctno inner join code on code.code= bp.code and code.category='FUA'
		and ( bp.dateadded between w.Datefrom and w.Dateto or (w.dateto is null and bp.dateadded > w.datefrom))
		and w.Worklist not in ('STF','SUP')
		INNER JOIN Admin.UserRole ur on ur.UserId = bf.empeeno
		INNER JOIN CollectionCommnRules ccr ON RoleId = ccr.EmpeeType
		  and bp.dateadded> =dateadd(dd,@PrevMaxDays,@LastRunDate) and bp.dateadded < dateadd(dd,@maxdays,@CurrentRunDate) -- UAT1129 jec
		 -- Previous action must be since the last successfull run. --IP - 19/07/10 UAT(1110) UAT5.2 
		 


-- no exxpected following action
update CollectionCommnAccts_init
set followingaction='EXP' 
where followingaction is null 
 
update CollectionCommnAccts_init
set followingactiondate=@CurrentRunDate where followingaction='EXP'

-- payment value is total payments between start and following action
update CollectionCommnAccts_init
set PaymentValue = isnull((select -sum(f.transvalue) from fintrans f	  --IP - 19/07/10 UAT(1110) UAT5.2  - Return sum of payments
							where f.acctno = CollectionCommnAccts_init.acctno
							and f.transtypecode in('PAY', 'COR', 'REF')
							and f.datetrans >= previousactiondate and f.datetrans <= followingactiondate),0),

Paymentdate = isnull((select max(f.datetrans) from fintrans f	  --IP - 19/07/10 UAT(1110) UAT5.2  - Return sum of payments
							where f.acctno = CollectionCommnAccts_init.acctno
							and f.transtypecode in('PAY', 'COR', 'REF')
							and f.datetrans >= previousactiondate and f.datetrans <= followingactiondate),0),
 
FeeValue = isnull((select sum(f.transvalue) from fintrans f	  --IP - 19/07/10 UAT(1110) UAT5.2  --Return sum of FEE's
							where f.acctno = CollectionCommnAccts_init.acctno
							and f.transtypecode ='FEE'
							and f.datetrans >= previousactiondate and f.datetrans <= followingactiondate),0),
							
BalanceBefore = isnull((select sum(f.transvalue) from fintrans f	  --IP - 19/07/10 UAT(1110) UAT5.2  --Return sum of FEE's
							where f.acctno = CollectionCommnAccts_init.acctno
							and f.datetrans <= previousactiondate ),0),
							
ArrearsBefore = isnull((select max(arrears) from ArrearsDaily ad 
                            where CollectionCommnAccts_init.acctno = ad.Acctno
							AND ( ISNULL(DATEADD(dd, DATEDIFF(dd,0,previousactiondate), 0),'1900-01-01') between  ISNULL(DATEADD(dd, DATEDIFF(dd,0,ad.datefrom), 0),'1900-01-01') and ISNULL(DATEADD(dd, DATEDIFF(dd,0,ad.dateto), 0),'1900-01-01') OR (previousactiondate>ad.datefrom and isnull(ad.dateto,'19000101')='19000101'))),
							(select arrears from acct where CollectionCommnAccts_init.acctno = acct.Acctno ))

update C

SET MonthsInArrearsBefore = case when isnull(newinstalment, 0) = 0 then 0.00 else  ArrearsBefore/newinstalment end,	
	
PercentOfArrearsPaid = case when arrearsbefore =0 then 0.00 when (paymentvalue-feevalue)/arrearsbefore>1 then 1 else (paymentvalue-feevalue)/arrearsbefore end,

PaymentDaysSinceAction = DATEDIFF(day,previousactiondate,paymentdate)				
FROM 
CollectionCommnAccts_init C
left outer join instalplanaudit ad on ad.acctno = C.acctno 
	and datechange = (select max(datechange) from instalplanaudit hist where hist.acctno = C.acctno and datechange <= PreviousActionDate)
 
insert into CollectionCommnAccts ( RunDate, AcctNo, EmpeeNo, EmpeeType, Worklist, PreviousAction, PreviousActionDate, FollowingAction, FollowingActionDate, PaymentValue, PaymentDate, FeeValue, BalanceBefore, ArrearsBefore, MonthsInArrearsBefore, PercentOfArrearsPaid, PaymentDaysSinceAction)
select RunDate, AcctNo, EmpeeNo, EmpeeType, Worklist, PreviousAction, PreviousActionDate, FollowingAction, FollowingActionDate, PaymentValue, PaymentDate, FeeValue, BalanceBefore, ArrearsBefore, MonthsInArrearsBefore, PercentOfArrearsPaid, PaymentDaysSinceAction
 from CollectionCommnAccts_init

/*******************************************Base table select*********************************************************************************************/
 

/*******************************************Update table variables for Percentage of worklist completed**************************************************/

--15/07/10
--Firstly select the worklists that we are interested in, that have been worked on since the last run.
insert into @CallsByWorklist
select distinct ca.Worklist,
				ca.EmpeeNo,
				ca.EmpeeType,
				0,
				0
from CollectionCommnAccts ca
where RunDate= @CurrentRunDate
and PreviousActionDate > @LastRunDate
 
--Update the total number of accounts that were in that worklist since the last run.
--Update the number of accounts actioned for each worklist and employee.
update @CallsByWorklist
set TotAcctsInWorkList = (select count(distinct cw1.acctno) from CMWorklistsAcct cw1
							where cw1.Worklist = cw.Worklist
							and (cw1.Dateto is null
							or cw1.Dateto > @LastRunDate)
							and cw1.Datefrom <= (SELECT dateadd(hh, 7,DATEADD(dd, DATEDIFF(dd,0,@CurrentRunDate), 0)))), --any accounts that enter the worklist after 7am on the run date should not be included.
    NoAcctsActioned = (select count(distinct ca.AcctNo) from CollectionCommnAccts ca
							where ca.Worklist = cw.Worklist
							and ca.EmpeeNo = cw.Empeeno
							and ca.RunDate = @CurrentRunDate
							and PreviousActionDate > @LastRunDate)
	
from @CallsByWorklist cw
 
--Sum by employee type and worklist which will later be used to determine what percentage of a worklist was completed if a Team rule.
insert into @CallsByWorklistTeam
select c.EmpeeType,
	   c.Worklist,
	   c.TotAcctsInWorkList,
	   sum(c.NoAcctsActioned)
from @CallsByWorklist c
group by c.EmpeeType, c.Worklist,c.TotAcctsInWorkList

/********************************************************************************************************************************************************/

 
/*******************************************Commission based on individual*********************************************************************************/

insert into @CollectionCommn
select cr.ID,					--IP - 09/07/10 - UAT(1094)UAT5.2
	   cr.RuleName,
	   cr.CommissionType,
	   ca.EmpeeNo,
	   ca.EmpeeType,
	   '',
	   sum(case when cr.NoOfDaysSinceAction>0 and cr.NoOfDaysSinceAction<ca.PaymentDaysSinceAction then 0 else 1 end),
	   sum(ca.ArrearsBefore), --select the max arrears for an account and divide amongst number of rows for that account
	   sum (case when cr.NoOfDaysSinceAction>0 and cr.NoOfDaysSinceAction<ca.PaymentDaysSinceAction then 0 else (case when (ca.PaymentValue-ca.FeeValue) <= ca.ArrearsBefore and ca.ArrearsBefore > 0 then ca.PaymentValue-ca.FeeValue	--UAT1096  jec  
				when ca.ArrearsBefore > 0 then ca.ArrearsBefore else 0 end)end),--Commission on Arrears % will be based on this, take the lesser of the arrears or payment
	   sum  (case when cr.NoOfDaysSinceAction>0 and cr.NoOfDaysSinceAction<ca.PaymentDaysSinceAction 
	   then 0 else (ca.PaymentValue - ca.FeeValue) end), --IP - 20/07/10 - UAT(1094) UAT5.2
	   sum(case when cr.NoOfDaysSinceAction>0 and cr.NoOfDaysSinceAction<ca.PaymentDaysSinceAction then 0 else ca.FeeValue end)
	    from  CollectionCommnRules cr join 
	   CollectionCommnAccts ca on 1=1
	  -- inner join view_CollCommns_MaxArrears vcm on vcm.AcctNo = ca.acctno and ca.EmpeeNo = vcm.EmpeeNo 
	   
where (
		 (cr.MinBal = 0 or (cr.MinBal > 0 and ca.BalanceBefore >= cr.MinBal))
			and (cr.MaxBal = 0 or (cr.MaxBal > 0 and ca.BalanceBefore <= cr.MaxBal))
			and (cr.MinValColl = 0 or (cr.MinValColl > 0 and ca.PaymentValue >= cr.MinValColl))
			and (cr.MaxValColl = 0 or (cr.MaxValColl > 0 and ca.PaymentValue <= cr.MaxValColl))
			and (cr.MinMnthsArrears = 0 or (cr.MinMnthsArrears > 0 and ca.MonthsInArrearsBefore >= cr.MinMnthsArrears))
			and (cr.MaxMnthsArrears = 0 or (cr.MaxMnthsArrears > 0 and ca.MonthsInArrearsBefore <= cr.MaxMnthsArrears)))
	
	
	and (exists (select ra.ActionTaken from CollectionCommnRuleActions ra where cr.ID = ra.ParentID and ra.ActionTaken='All' and
	 ca.previousaction in  (select code from code c --If rule states ALL actions, then check this action exists in FUA category
																	where c.category = 'FUA')) or 
		( ca.previousaction in (select ra.ActionTaken from CollectionCommnRuleActions ra where cr.ID = ra.ParentID)  ))
	
	and ca.EmpeeType = cr.EmpeeType
	and cr.CommissionType = 'I'
	and ca.RunDate = @CurrentRunDate
	and cr.PcentOfWorklist = 0 --IP - 15/07/10 - Do not include the rule which looks at percentage of worklist completed as this is handled below.
	
group by cr.ID,  cr.RuleName, cr.CommissionType, ca.EmpeeNo, ca.EmpeeType
order by cr.ID, cr.RuleName, cr.CommissionType, ca.EmpeeNo
 

--Select Number Of Calls made by an employee since the last EOD run as a total
;with NumberOfCalls
as
(

	select ca1.empeeno, rp.RoleId, count(*) as [NoOfCalls]
	from CollectionCommnAccts ca1 
    inner join Admin.[User] u on ca1.EmpeeNo = u.Id
    inner join Admin.UserRole rp on u.Id = rp.UserId
	where ca1.RunDate = @CurrentRunDate
	group by ca1.empeeno, rp.RoleId
	
)


 
insert into CollectionCommn(RunDate,
							RuleID,
							RuleName,
							CommissionType,
							EmpeeNo,
							EmpeeType,
							Worklist,
							ArrearsTot,
							ArrearsPaidTot,
							AmtCollectedTot,
							FeeTot,
							NoOfCalls,
							CommnOnArrears,
							CommnOnAmtPaid,
							CommnOnFee,
							CommnSetVal)
							
select @CurrentRunDate as RunDate,
	   c.RuleID,
	   c.RuleName,
	   c.CommissionType,
	   c.Empeeno,
	   c.EmpeeType,
	   '' as Worklist,
	   c.ArrearsTot,
	   c.ArrearsPaidTot,
	   c.AmountCollectedTot,
	   c.FeeTot,
	   c.NoOfCalls,
	   case when cr.PcentCommOnArrears = 0 then 0 else round(c.ArrearsPaidTot * (cr.PcentCommOnArrears/100),2) end, --Commission on the arrears paid
	   case	when cr.PcentCommOnAmtPaid = 0 then 0 else round(c.AmountCollectedTot * (cr.PcentCommOnAmtPaid/100),2) end,  --Commission on amount paid
	   case when cr.PcentCommOnFee = 0 then 0 else round((c.FeeTot * (cr.PcentCommOnFee)/100),2) end as [CommissionOnFee], --Commission on fee
	   case	when cr.CommSetVal = 0 then 0 else cr.CommSetVal end as [CommissionSetVal] --Set value commission
from @CollectionCommn c
	inner join CollectionCommnRules cr on cr.ID = c.RuleID
	and cr.EmpeeType = c.EmpeeType
	and cr.CommissionType = 'I'
	--inner join CollectionCommnRuleActions cra on cr.ID = cra.ParentID		--UAT1096 jec stop row for every rule action 
	inner join NumberOfCalls n on c.EmpeeNo = n.EmpeeNo

where((cr.PcentArrearsColl = 0 or (cr.PcentArrearsColl > 0 and case when c.ArrearsTot = 0 then 0 else ((c.ArrearsPaidTot/c.ArrearsTot)*100)end>=cr.PcentArrearsColl)) --Has more than the defined % of arrears collected been met.
			and (cr.NoOfCalls = 0 or (cr.NoOfCalls > 0 and case when @daysSinceLastRun=0 then 0 else (floor((Cast(c.NoOfCalls as float)/ CAST(@daysSinceLastRun as float)) * cr.TimeFrameDays)) end >=cr.NoOfCalls)) --Has more than the NoOfCalls for the defined Time Frame been met.
			and (cr.PcentOfCalls = 0 or (cr.PcentOfCalls > 0 and case when n.noofcalls = 0 then 0 else (round((cast(c.NoOfCalls as float)/cast(n.NoOfCalls as float))*100,2)) end>= cr.PcentOfCalls))) --
		
order by c.Empeeno

 
 
--15/07/10
--Seperate insert into CollectionCommns for Percentage of worklist completed
insert into CollectionCommn(RunDate,
							RuleID,
							RuleName,
							CommissionType,
							EmpeeNo,
							EmpeeType,
							Worklist,
							ArrearsTot,
							ArrearsPaidTot,
							AmtCollectedTot,
							FeeTot,
							NoOfCalls,
							CommnOnArrears,
							CommnOnAmtPaid,
							CommnOnFee,
							CommnSetVal)
							
select @CurrentRunDate as RunDate,
	   cr.ID,
	   cr.RuleName,
	   cr.CommissionType,
	   c.Empeeno,
	   c.EmpeeType,
	   c.Worklist,
	   0,
	   0,
	   0,
	   0,
	   c.NoAcctsActioned,
	   0,
	   0,
	   0,
	   case	when cr.CommSetVal = 0 then 0 else cr.CommSetVal end as [CommissionSetVal] --Set value commission
from @CallsByWorklist c
	inner join CollectionCommnRules cr 
	on cr.EmpeeType = c.EmpeeType
	where (cr.PcentOfWorklist > 0 and case when c.TotAcctsInWorkList=0 then 0 else ((cast(c.NoAcctsActioned as float)/cast(c.TotAcctsInWorkList as float)) * 100) end >= cr.PcentOfWorklist)
	and cr.CommissionType = 'I'
	

 
/*************************Commission based on Team*********************************************************************************************************************/

insert into @CollectionCommn
select cr.ID, 
	   cr.RuleName,
	   cr.CommissionType,
	   0,
	   ca.EmpeeType,
	   ca.Worklist,
	  sum(case when cr.NoOfDaysSinceAction>0 and cr.NoOfDaysSinceAction<ca.PaymentDaysSinceAction then 0 else 1 end),
	   sum(ArrearsBefore), --select the max arrears for an account and divide amongst number of rows for that account
	   sum (case when cr.NoOfDaysSinceAction>0 and cr.NoOfDaysSinceAction<ca.PaymentDaysSinceAction then 0 else (case when (ca.PaymentValue-ca.FeeValue) <= ca.ArrearsBefore and ca.ArrearsBefore > 0 then ca.PaymentValue-ca.FeeValue	--UAT1096  jec  
				when ca.ArrearsBefore > 0 then ca.ArrearsBefore else 0 end)end),--Commission on Arrears % will be based on this, take the lesser of the arrears or payment
	   sum  (case when cr.NoOfDaysSinceAction>0 and cr.NoOfDaysSinceAction<ca.PaymentDaysSinceAction 
	   then 0 else (ca.PaymentValue - ca.FeeValue) end), --IP - 20/07/10 - UAT(1094) UAT5.2
	   sum(case when cr.NoOfDaysSinceAction>0 and cr.NoOfDaysSinceAction<ca.PaymentDaysSinceAction then 0 else ca.FeeValue end)
	   
	   from  CollectionCommnRules cr
	   inner join CollectionCommnRuleActions ra on cr.ID = ra.ParentID, 
	   CollectionCommnAccts ca 
	   
where (
			(cr.MinBal = 0 or (cr.MinBal > 0 and ca.BalanceBefore >= cr.MinBal))
			and (cr.MaxBal = 0 or (cr.MaxBal > 0 and ca.BalanceBefore <= cr.MaxBal))
			and (cr.MinValColl = 0 or (cr.MinValColl > 0 and ca.PaymentValue >= cr.MinValColl))
			and (cr.MaxValColl = 0 or (cr.MaxValColl > 0 and ca.PaymentValue <= cr.MaxValColl))
			and (cr.MinMnthsArrears = 0 or (cr.MinMnthsArrears > 0 and ca.MonthsInArrearsBefore >= cr.MinMnthsArrears))
			and (cr.MaxMnthsArrears = 0 or (cr.MaxMnthsArrears > 0 and ca.MonthsInArrearsBefore <= cr.MaxMnthsArrears)))
	
	

	and (exists (select ra.ActionTaken from CollectionCommnRuleActions ra where cr.ID = ra.ParentID and ra.ActionTaken='All' and
	 ca.previousaction in  (select code from code c --If rule states ALL actions, then check this action exists in FUA category
																	where c.category = 'FUA')) or 
		( ca.previousaction in (select ra.ActionTaken from CollectionCommnRuleActions ra where cr.ID = ra.ParentID)  ))
	
	
	and ca.EmpeeType = cr.EmpeeType
	and cr.CommissionType = 'T'
	and ca.RunDate = @CurrentRunDate
	and cr.PcentOfWorklist = 0 --IP - 15/07/10 - Do not include the rule which looks at percentage of worklist completed as this is handled below.

group by cr.ID,  cr.RuleName,cr.CommissionType, ca.EmpeeType, ca.Worklist
order by cr.ID, cr.RuleName, ca.EmpeeType, ca.Worklist



--Return the total number of calls made within a worklist by employee type.
;with NumberOfCallsByWorklist
as
(
	select ca2.Worklist, ca2.EmpeeType, count(*) as NoOfCallsByWorkList
	from CollectionCommnAccts ca2
	where ca2.RunDate = @CurrentRunDate
	group by ca2.Worklist, ca2.EmpeeType

),

--Return the employees that have worked on a worklist in the run.
EmployeesInTeam
as
(
	select distinct ca3.EmpeeNo, ca3.EmpeeType, ca3.Worklist
	from CollectionCommnAccts ca3
	where ca3.RunDate = @CurrentRunDate
),

NoOfEmployeesInTeam
as
(
	select ca.worklist, count(distinct ca.empeeno) as NoInTeam
	from CollectionCommnAccts ca
	where ca.RunDate = @CurrentRunDate --16/07/10  IP  UAT(1104) UAT5.2
	group by ca.Worklist
)
 
insert into CollectionCommn(RunDate,
							RuleID,
							RuleName,
							CommissionType,
							EmpeeNo,
							EmpeeType,
							Worklist,
							ArrearsTot,
							ArrearsPaidTot,
							AmtCollectedTot,
							FeeTot,
							NoOfCalls,
							CommnOnArrears,
							CommnOnAmtPaid,
							CommnOnFee,
							CommnSetVal)
select distinct @CurrentRunDate as RunDate,			--IP - 09/07/10 - UAT(1094)UAT5.2
	   c.RuleID,
	   c.RuleName,
	   c.CommissionType,
	   e.EmpeeNo,
	   c.EmpeeType,
	   c.Worklist,
	   c.ArrearsTot,
	   c.ArrearsPaidTot,
	   c.AmountCollectedTot,
	   c.FeeTot,
	   c.NoOfCalls,
	   case when nt.NoInTeam = 0 then 0 else round((c.ArrearsPaidTot * (cr.PcentCommOnArrears/100)/nt.NoInTeam),2) end, --Commission divided amongst team (Commission on arrears)
	   case	when nt.NoInTeam = 0 then 0 else round((c.AmountCollectedTot * (cr.PcentCommOnAmtPaid/100)/nt.NoInTeam),2) end , --Commission divided amongst team (Commission on amount paid)
	   case when nt.NoInTeam = 0 then 0 else round((c.FeeTot * (cr.PcentCommOnFee/100)/nt.NoInTeam),2) end, --Commission divided amongst team (Commission on fee)
	   case	when nt.NoInTeam = 0 then 0 else (cr.CommSetVal/nt.NoInTeam) end as [CommissionSetVal] --Commission divided amongst team
from @CollectionCommn c
	inner join CollectionCommnRules cr on cr.ID = c.RuleID
	and cr.EmpeeType = c.EmpeeType
	and cr.CommissionType = 'T'
	inner join CollectionCommnRuleActions cra on cr.ID = cra.ParentID
	inner join NumberOfCallsByWorklist nw on c.Worklist = nw.Worklist
	and c.EmpeeType = nw.EmpeeType
	inner join EmployeesInTeam e on e.Worklist = c.Worklist and e.EmpeeType = c.EmpeeType --Only give commission to employees that are in the team.
	inner join NoOfEmployeesInTeam nt on c.Worklist = nt.Worklist --Divide commission amongst number of employees in the team

where((cr.PcentArrearsColl = 0 or (cr.PcentArrearsColl > 0 and case when c.ArrearsTot = 0 then 0 else ((c.ArrearsPaidTot/c.ArrearsTot)*100)end >=cr.PcentArrearsColl))  --Has more than the defined % of arrears collected been met.
			and (cr.NoOfCalls = 0 or (cr.NoOfCalls > 0 and case when @daysSinceLastRun=0 then 0 else (floor((Cast(c.NoOfCalls as float)/ CAST(@daysSinceLastRun as float)) * cr.TimeFrameDays) )end >=cr.NoOfCalls)) --Has more than the NoOfCalls for the defined Time Frame been met.
			and (cr.PcentOfCalls = 0 or (cr.PcentOfCalls > 0 and case when nw.NoOfCallsByWorkList=0 then 0 else (round((cast(c.NoOfCalls as float)/cast(nw.NoOfCallsByWorkList as float))*100,2))end >= cr.PcentOfCalls)) )

 
--15/07/10
--Seperate insert into CollectionCommns for Percentage of worklist completed

--Return the employees that have worked on a worklist in the run.
;with EmployeesInTeam2
as
(
	select distinct ca3.EmpeeNo, ca3.EmpeeType, ca3.Worklist
	from CollectionCommnAccts ca3
	where ca3.RunDate = @CurrentRunDate
),

NoOfEmployeesInTeam2
as
(
	select ca.worklist, count(distinct ca.empeeno) as NoInTeam
	from CollectionCommnAccts ca
	where ca.RunDate = @CurrentRunDate --16/07/10  IP  UAT(1104) UAT5.2
	group by ca.Worklist
)

insert into CollectionCommn(RunDate,
							RuleID,
							RuleName,
							CommissionType,
							EmpeeNo,
							EmpeeType,
							Worklist,
							ArrearsTot,
							ArrearsPaidTot,
							AmtCollectedTot,
							FeeTot,
							NoOfCalls,
							CommnOnArrears,
							CommnOnAmtPaid,
							CommnOnFee,
							CommnSetVal)
							
select @CurrentRunDate as RunDate,
	   cr.ID,
	   cr.RuleName,
	   cr.CommissionType,
	   e.Empeeno,
	   cw.EmpeeType,
	   cw.Worklist,
	   0,
	   0,
	   0,
	   0,
	   cw.TotNoAcctsActioned,
	   0,
	   0,
	   0,
	  case	when  nt.NoInTeam = 0 then 0 else (cr.CommSetVal/nt.NoInTeam) end as [CommissionSetVal] --Commission divided amongst team
from @CallsByWorklistTeam cw
	inner join CollectionCommnRules cr 
	on cr.EmpeeType = cw.EmpeeType
	inner join EmployeesInTeam2 e on e.Worklist = cw.Worklist and e.EmpeeType = cw.EmpeeType --Only give commission to employees that are in the team.
	inner join NoOfEmployeesInTeam2 nt on cw.Worklist = nt.Worklist --Divide commission amongst number of employees in the team
	where (cr.PcentOfWorklist > 0 and case when cw.TotAcctsInWorkList=0 then 0 else ((cast(cw.TotNoAcctsActioned as float)/cast(cw.TotAcctsInWorkList as float)) * 100)end >= cr.PcentOfWorklist)
	and cr.CommissionType = 'T'
	
/*****************************************RULES NOT PASSED****************************************************************************************************/
 
--IP/JC - 22/07/10 - Reports require details of the rules that the employees did not qualify on. i.e. no commission calculated.

/*****************************************Individual Commissions****************************************************************************************************/

--Select Number Of Calls made by an employee since the last EOD run as a total
;with NumberOfCalls
as
(

    select ca1.empeeno, rp.RoleId, count(*) as [NoOfCalls]
	from CollectionCommnAccts ca1 
    inner join Admin.[User] u on ca1.EmpeeNo = u.Id
    inner join Admin.UserRole rp on u.Id = rp.UserId
	where ca1.RunDate = @CurrentRunDate
	group by ca1.empeeno, rp.RoleId
	
)


insert into CollectionCommn(RunDate,
							RuleID,
							RuleName,
							CommissionType,
							EmpeeNo,
							EmpeeType,
							Worklist,
							ArrearsTot,
							ArrearsPaidTot,
							AmtCollectedTot,
							FeeTot,
							NoOfCalls,
							CommnOnArrears,
							CommnOnAmtPaid,
							CommnOnFee,
							CommnSetVal)
							
select @CurrentRunDate as RunDate,
	   c.RuleID,
	   c.RuleName,
	   c.CommissionType,
	   c.Empeeno,
	   c.EmpeeType,
	   '' as Worklist,
	   c.ArrearsTot,
	   c.ArrearsPaidTot,
	   c.AmountCollectedTot,
	   c.FeeTot,
	   c.NoOfCalls,
	   0, --Commission on the arrears paid
	   0,  --Commission on amount paid
	   0, --Commission on fee
	   0 --Set value commission
from @CollectionCommn c
	inner join CollectionCommnRules cr on cr.ID = c.RuleID
	and cr.EmpeeType = c.EmpeeType
	and cr.CommissionType = 'I'
	--inner join CollectionCommnRuleActions cra on cr.ID = cra.ParentID		--UAT1096 jec stop row for every rule action 
	inner join NumberOfCalls n on c.EmpeeNo = n.EmpeeNo

where((cr.PcentArrearsColl > 0 and not(case when c.ArrearsTot = 0 then 0 else ((c.ArrearsPaidTot/c.ArrearsTot)*100)end>=cr.PcentArrearsColl)) --Has more than the defined % of arrears collected been met.
			or ((cr.NoOfCalls > 0 and not case when @daysSinceLastRun=0 then 0 else (floor((Cast(c.NoOfCalls as float)/ CAST(@daysSinceLastRun as float)) * cr.TimeFrameDays)) end >=cr.NoOfCalls)) --Has more than the NoOfCalls for the defined Time Frame been met.
			or ((cr.PcentOfCalls > 0 and not case when n.NoOfCalls=0 then 0 else (round((cast(c.NoOfCalls as float)/cast(n.NoOfCalls as float))*100,2)) end>= cr.PcentOfCalls))) --

order by c.Empeeno

 

--15/07/10
--Seperate insert into CollectionCommns for Percentage of worklist completed
insert into CollectionCommn(RunDate,
							RuleID,
							RuleName,
							CommissionType,
							EmpeeNo,
							EmpeeType,
							Worklist,
							ArrearsTot,
							ArrearsPaidTot,
							AmtCollectedTot,
							FeeTot,
							NoOfCalls,
							CommnOnArrears,
							CommnOnAmtPaid,
							CommnOnFee,
							CommnSetVal)
							
select @CurrentRunDate as RunDate,
	   cr.ID,
	   cr.RuleName,
	   cr.CommissionType,
	   c.Empeeno,
	   c.EmpeeType,
	   c.Worklist,
	   0,
	   0,
	   0,
	   0,
	   c.NoAcctsActioned,
	   0,
	   0,
	   0,
	   0 --Set value commission
from @CallsByWorklist c
	inner join CollectionCommnRules cr 
	on cr.EmpeeType = c.EmpeeType
	where cr.PcentOfWorklist > 0 and not case when c.TotAcctsInWorkList=0 then 0 else ((cast(c.NoAcctsActioned as float)/cast(c.TotAcctsInWorkList as float)) * 100) end >= cr.PcentOfWorklist
	and cr.CommissionType = 'I'


/*************************************Team Commissions*************************************************************************************************************************/
--Return the total number of calls made within a worklist by employee type.
;with NumberOfCallsByWorklist
as
(
	select ca2.Worklist, ca2.EmpeeType, count(*) as NoOfCallsByWorkList
	from CollectionCommnAccts ca2
	where ca2.RunDate = @CurrentRunDate
	group by ca2.Worklist, ca2.EmpeeType

),

--Return the employees that have worked on a worklist in the run.
EmployeesInTeam
as
(
	select distinct ca3.EmpeeNo, ca3.EmpeeType, ca3.Worklist
	from CollectionCommnAccts ca3
	where ca3.RunDate = @CurrentRunDate
),

NoOfEmployeesInTeam
as
(
	select ca.worklist, count(distinct ca.empeeno) as NoInTeam
	from CollectionCommnAccts ca
	where ca.RunDate = @CurrentRunDate --16/07/10  IP  UAT(1104) UAT5.2
	group by ca.Worklist
)
 
insert into CollectionCommn(RunDate,
							RuleID,
							RuleName,
							CommissionType,
							EmpeeNo,
							EmpeeType,
							Worklist,
							ArrearsTot,
							ArrearsPaidTot,
							AmtCollectedTot,
							FeeTot,
							NoOfCalls,
							CommnOnArrears,
							CommnOnAmtPaid,
							CommnOnFee,
							CommnSetVal)
select distinct @CurrentRunDate as RunDate,			--IP - 09/07/10 - UAT(1094)UAT5.2
	   c.RuleID,
	   c.RuleName,
	   c.CommissionType,
	   e.EmpeeNo,
	   c.EmpeeType,
	   c.Worklist,
	   c.ArrearsTot,
	   c.ArrearsPaidTot,
	   c.AmountCollectedTot,
	   c.FeeTot,
	   c.NoOfCalls,
	   0, --Commission divided amongst team (Commission on arrears)
	   0 , --Commission divided amongst team (Commission on amount paid)
	   0, --Commission divided amongst team (Commission on fee)
	   0--Commission divided amongst team
from @CollectionCommn c
	inner join CollectionCommnRules cr on cr.ID = c.RuleID
	and cr.EmpeeType = c.EmpeeType
	and cr.CommissionType = 'T'
	inner join CollectionCommnRuleActions cra on cr.ID = cra.ParentID
	inner join NumberOfCallsByWorklist nw on c.Worklist = nw.Worklist
	and c.EmpeeType = nw.EmpeeType
	inner join EmployeesInTeam e on e.Worklist = c.Worklist and e.EmpeeType = c.EmpeeType --Only give commission to employees that are in the team.
	inner join NoOfEmployeesInTeam nt on c.Worklist = nt.Worklist --Divide commission amongst number of employees in the team

where ((cr.PcentArrearsColl > 0 and not (case when c.ArrearsTot = 0 then 0 else ((c.ArrearsPaidTot/c.ArrearsTot)*100)end >=cr.PcentArrearsColl))  --Has more than the defined % of arrears collected been met.
			or ((cr.NoOfCalls > 0 and not case when @daysSinceLastRun=0 then 0 else (floor((Cast(c.NoOfCalls as float)/ CAST(@daysSinceLastRun as float)) * cr.TimeFrameDays)) end >=cr.NoOfCalls)) --Has more than the NoOfCalls for the defined Time Frame been met.
			or ((cr.PcentOfCalls > 0 and not case when nw.NoOfCallsByWorkList=0 then 0 else (round((cast(c.NoOfCalls as float)/cast(nw.NoOfCallsByWorkList as float))*100,2)) end >= cr.PcentOfCalls)) 
)

--15/07/10
--Seperate insert into CollectionCommns for Percentage of worklist completed

--Return the employees that have worked on a worklist in the run.
;with EmployeesInTeam2
as
(
	select distinct ca3.EmpeeNo, ca3.EmpeeType, ca3.Worklist
	from CollectionCommnAccts ca3
	where ca3.RunDate = @CurrentRunDate
),

NoOfEmployeesInTeam2
as
(
	select ca.worklist, count(distinct ca.empeeno) as NoInTeam
	from CollectionCommnAccts ca
	where ca.RunDate = @CurrentRunDate --16/07/10  IP  UAT(1104) UAT5.2
	group by ca.Worklist
)
 
insert into CollectionCommn(RunDate,
							RuleID,
							RuleName,
							CommissionType,
							EmpeeNo,
							EmpeeType,
							Worklist,
							ArrearsTot,
							ArrearsPaidTot,
							AmtCollectedTot,
							FeeTot,
							NoOfCalls,
							CommnOnArrears,
							CommnOnAmtPaid,
							CommnOnFee,
							CommnSetVal)
							
select @CurrentRunDate as RunDate,
	   cr.ID,
	   cr.RuleName,
	   cr.CommissionType,
	   e.Empeeno,
	   cw.EmpeeType,
	   cw.Worklist,
	   0,
	   0,
	   0,
	   0,
	   cw.TotNoAcctsActioned,
	   0,
	   0,
	   0,
	   0 --Commission divided amongst team
from @CallsByWorklistTeam cw
	inner join CollectionCommnRules cr 
	on cr.EmpeeType = cw.EmpeeType
	inner join EmployeesInTeam2 e on e.Worklist = cw.Worklist and e.EmpeeType = cw.EmpeeType --Only give commission to employees that are in the team.
	inner join NoOfEmployeesInTeam2 nt on cw.Worklist = nt.Worklist --Divide commission amongst number of employees in the team
	where cr.PcentOfWorklist > 0 and not case when cw.TotAcctsInWorkList=0 then 0 else ((cast(cw.TotNoAcctsActioned as float)/cast(cw.TotAcctsInWorkList as float)) * 100) end >= cr.PcentOfWorklist
	and cr.CommissionType = 'T'

/*****************************************RULES NOT PASSED****************************************************************************************************/
			
 
IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END

go

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End
