IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[summary18sp]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[summary18sp]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[summary18sp]
--ATL report	
	AS
	

	truncate table summary18
	
	declare	@FromDate  DATETIME, 
			@ToDate  DATETIME,
			@SettToDate  DATETIME
		
set @FromDate  = Convert(datetime,cast(dateadd(m, -12, dateadd(d, -(datepart(d, getdate())-1), getdate()))as char(12)),103)
set @ToDate  = dateadd(mi, -1, dateadd(m,12,@FromDate))		
set @SettToDate=GETDATE()
	-- accounts delivered in period
	select CAST(left(a.acctno,3) as INT) as branchno,c.userid, a.acctno,convert(money,0) as SettledInMths,ag.datedel,i.[datefirst],i.datelast,
			a.currstatus,a.termstype,i.instalno,ag.servicechg,ag.agrmttotal,CAST(0 as INT) as SettInstNo,
			convert(money,0) as SettledAgrmtTotal,CAST(0 as INT) as PeriodSettledNum,CAST(0 as INT) as PeriodSettInstNo,
			CAST(0 as MONEY) as PeriodSettledAvgTerm,convert(money,0) as PeriodSettledAgrmtTotal,	-- 26/05/10
			ag.agrmttotal-ag.servicechg as CashPrice,ISNULL(p.ScoringBand,' ') as ScoringBand,r.intrate	-- 14/06/10
	into #delivered
	from acct a INNER JOIN agreement ag on a.acctno=ag.acctno
				INNER JOIN instalplan i on i.acctno=ag.acctno and ag.agrmtno = i.agrmtno
				INNER JOIN Branch b on b.branchno=CAST(left(a.acctno,3) as INT)
				INNER JOIN CourtsPerson c on c.UserID= ag.empeenosale
				--INNER JOIN splitFN(@Branch,',') br on CAST(LEFT(a.acctno,3) as INT) = br.items				
				-- jec 14/06/10
				INNER JOIN Proposal p on a.acctno=p.acctno
				INNER JOIN intratehistory r on a.termstype=r.termstype 
						and p.dateprop >= r.datefrom and (p.dateprop <=r.dateto	or r.dateto='1900-01-01')
						and p.ScoringBand=r.Band				

	where ISNULL(ag.datedel,'1900-01-01') between @FromDate and @ToDate
		and a.accttype not in ('C','S') and a.agrmttotal!=0
		--and (b.StoreType=@StoreType or @StoreType='B')

	-- Term Settled	
	UPDATE #delivered		-- 26/05/10
		set SettInstNo=DATEDIFF(mm,[datefirst],DATEADD(d,1,s.datestatchge)),SettledAgrmtTotal=agrmttotal	
	from #delivered d INNER JOIN status s on d.acctno = s.acctno and d.currstatus=s.statuscode
	 and d.currstatus='S' and s.CurrentStatus='Y'
	 
	-- Term Settled within Period	-- 26/05/10	
	UPDATE #delivered
		set PeriodSettInstNo=DATEDIFF(mm,[datefirst],DATEADD(d,1,s.datestatchge)),PeriodSettledAgrmtTotal=agrmttotal,
			PeriodSettledNum=1
	from #delivered d INNER JOIN status s on d.acctno = s.acctno and d.currstatus=s.statuscode
	 and d.currstatus='S' and s.CurrentStatus='Y'
	 and s.datestatchge <= @SettToDate
		
	-- Summarise deliveries
	-- By Employee 	
	select 0 as branchno,d.userid,d.termstype,ScoringBand,COUNT(*) as NumAccts,CAST(0 as float) as PctAccounts,
			SUM(servicechg) as ServiceChg,SUM(agrmttotal) as AgrmtTotal,
			SUM(InstalNo) as TotalTerm,CAST(SUM(InstalNo)/(COUNT(*) * 1.00) as money) as AVGTerm,
			CAST(0 as INT) as NumSettled,CAST(0 as int) as TermSettled,CAST(0 as MONEY) as AvgTermSettled,
			CAST(0 as float) as PctAgrTot,CAST(0 as MONEY) as WghtAvgTerm,CAST(0 as MONEY) as WghtAvgTermSettled,
			CAST(0 as MONEY) as RebatesPaid,CAST (0 as MONEY) as UnearnedIncome,CAST (0 as MONEY) as EarnedIncome,
			CAST(0 as MONEY) as WgtTermAgrTot,CAST(0 as MONEY) as WgtSettAgrTot,
			Sum(SettledAgrmtTotal) as AgrmtTotalSettled,Sum(PeriodSettledNum) as PeriodNumSettled,
			Sum(PeriodSettledAgrmtTotal) as PeriodSettledAgrmtTotal,CAST(0 as MONEY) as PeriodAvgTermSettled,
			CAST(0 as int) as PeriodTermSettled,	-- 14/06/10		
			SUM(CashPrice) as TotCashPrice,MAX(IntRate) as IntRate	-- 14/06/10				
			
	into #Delsum
	from #delivered d 
	group by d.userid,d.termstype,ScoringBand	-- 14/06/10	
	order by d.userid, d.termstype,ScoringBand
	
	-- By Branch
	insert  into #Delsum 	
	select d.branchno, 0,d.termstype,ScoringBand,COUNT(*) as NumAccts,CAST(0 as float) as PctAccounts,
			SUM(servicechg) as ServiceChg,SUM(agrmttotal) as AgrmtTotal,
			SUM(InstalNo) as TotalTerm,CAST(SUM(InstalNo)/(COUNT(*) * 1.00) as money) as AVGTerm,
			CAST(0 as INT) as NumSettled,CAST(0 as int) as TermSettled,CAST(0 as MONEY) as AvgTermSettled,
			CAST(0 as float) as PctAgrTot,CAST(0 as MONEY) as WghtAvgTerm,CAST(0 as MONEY) as WghtAvgTermSettled,
			CAST(0 as MONEY) as RebatesPaid,CAST (0 as MONEY) as UnearnedIncome,CAST (0 as MONEY) as EarnedIncome,
			CAST(0 as MONEY) as WgtTermAgrTot,CAST(0 as MONEY) as WgtSettAgrTot,
			Sum(SettledAgrmtTotal) as AgrmtTotalSettled,Sum(PeriodSettledNum) as PeriodNumSettled,
			Sum(PeriodSettledAgrmtTotal) as PeriodSettledAgrmtTotal,CAST(0 as MONEY) as PeriodAvgTermSettled,
			CAST(0 as int) as PeriodTermSettled,	-- 14/06/10		
			SUM(CashPrice) as TotCashPrice,MAX(IntRate) as IntRate	-- 14/06/10				
			
	
	from #delivered d 
	group by d.BranchNo,d.termstype,ScoringBand	-- 14/06/10	
	Order by d.BranchNo, d.termstype,ScoringBand
	
	
	-- By Country 
	insert into #Delsum
	select 0,0,d.termstype,ScoringBand,COUNT(*) as NumAccts,CAST(0 as float) as PctAccounts,
			SUM(servicechg) as ServiceChg,SUM(agrmttotal) as AgrmtTotal,
			SUM(InstalNo) as TotalTerm,CAST(SUM(InstalNo)/(COUNT(*) * 1.00) as money) as AVGTerm,
			CAST(0 as INT) as NumSettled,CAST(0 as int) as TermSettled,CAST(0 as MONEY) as AvgTermSettled,
			CAST(0 as float) as PctAgrTot,CAST(0 as MONEY) as WghtAvgTerm,CAST(0 as MONEY) as WghtAvgTermSettled,
			CAST(0 as MONEY) as RebatesPaid,CAST (0 as MONEY) as UnearnedIncome,CAST (0 as MONEY) as EarnedIncome,
			CAST(0 as MONEY) as WgtTermAgrTot,CAST(0 as MONEY) as WgtSettAgrTot,
			Sum(SettledAgrmtTotal) as AgrmtTotalSettled,Sum(PeriodSettledNum) as PeriodNumSettled,
			Sum(PeriodSettledAgrmtTotal) as PeriodSettledAgrmtTotal,CAST(0 as MONEY) as PeriodAvgTermSettled,
			CAST(0 as int) as PeriodTermSettled,		-- 26/05/10
			SUM(CashPrice) as TotCashPrice,MAX(IntRate) as IntRate	-- 14/06/10				
	
	from #delivered d 
	group by d.termstype,ScoringBand	-- 14/06/10	
	Order by d.termstype,ScoringBand	-- 14/06/10	
	
	
	
	-- settled
	-- by Employee  	
	select BranchNo,d.userid,d.termstype,ScoringBand,		-- 14/06/10
	COUNT(*) as NumSett,
		SUM(DATEDIFF(mm,[datefirst],DATEADD(d,1,s.datestatchge))) as TotSetTerm, 
		CAST(SUM(DATEDIFF(mm,[datefirst],DATEADD(d,1,s.datestatchge)))/(COUNT(*) * 1.00) as MONEY) as AvgTermSett,
		SUM(AgrmtTotal) as AgrmtTotalSettled
	into #Sett
	from #delivered d INNER JOIN status s on d.acctno = s.acctno and d.currstatus=s.statuscode
	 and d.currstatus='S' and s.CurrentStatus='Y'	 
	group by BranchNo,d.userid,d.termstype,ScoringBand		-- 14/06/10
	order by BranchNo,d.userid,d.termstype,ScoringBand		-- 14/06/10
	
	-- by Branch  	
	insert into #sett
	select branchno, 0,d.termstype,ScoringBand,		-- 14/06/10
	COUNT(*) as NumSett,
		SUM(DATEDIFF(mm,[datefirst],DATEADD(d,1,s.datestatchge))) as TotSetTerm, 
		CAST(SUM(DATEDIFF(mm,[datefirst],DATEADD(d,1,s.datestatchge)))/(COUNT(*) * 1.00) as MONEY) as AvgTermSett,
		SUM(AgrmtTotal) as AgrmtTotalSettled
	
	from #delivered d INNER JOIN status s on d.acctno = s.acctno and d.currstatus=s.statuscode
	 and d.currstatus='S' and s.CurrentStatus='Y'	 
	group by BranchNo,d.termstype,ScoringBand		-- 14/06/10
	order by BranchNo,d.termstype,ScoringBand		-- 14/06/10
	
	-- by Country
	insert into #Sett 	
	select 0,0,d.termstype,ScoringBand,COUNT(*) as NumSett,	-- 14/06/10
		SUM(DATEDIFF(mm,[datefirst],DATEADD(d,1,s.datestatchge))) as TotSetTerm, 
		CAST(SUM(DATEDIFF(mm,[datefirst],DATEADD(d,1,s.datestatchge)))/(COUNT(*) * 1.00) as MONEY) as AvgTermSett,
		SUM(AgrmtTotal) as AgrmtTotalSettled
	
	from #delivered d INNER JOIN status s on d.acctno = s.acctno and d.currstatus=s.statuscode
	 and d.currstatus='S' and s.CurrentStatus='Y'
	group by d.termstype,ScoringBand		-- 14/06/10
	order by d.termstype,ScoringBand		-- 14/06/10
	
	-- settled  in Period
	-- by Employee  	
	select BranchNo,d.userid,d.termstype,ScoringBand,	-- 14/06/10
	COUNT(*) as NumSett,
		SUM(DATEDIFF(mm,[datefirst],DATEADD(d,1,s.datestatchge))) as PeriodTotSetTerm, 
		CAST(SUM(DATEDIFF(mm,[datefirst],DATEADD(d,1,s.datestatchge)))/(COUNT(*) * 1.00) as MONEY) as PeriodAvgTermSett,
		SUM(AgrmtTotal) as PeriodAgrmtTotalSettled
	into #PeriodSett
	from #delivered d INNER JOIN status s on d.acctno = s.acctno and d.currstatus=s.statuscode
	 and d.currstatus='S' and s.CurrentStatus='Y'
	 and s.datestatchge <= @SettToDate	 
	group by BranchNo,d.userid,d.termstype,ScoringBand		-- 14/06/10
	order by BranchNo,d.userid,d.termstype,ScoringBand		-- 14/06/10
	
	-- by Branch  	
	insert into #PeriodSett
	select branchno, 0,d.termstype,ScoringBand,	-- 14/06/10
	COUNT(*) as NumSett,
		SUM(DATEDIFF(mm,[datefirst],DATEADD(d,1,s.datestatchge))) as PeriodTotSetTerm, 
		CAST(SUM(DATEDIFF(mm,[datefirst],DATEADD(d,1,s.datestatchge)))/(COUNT(*) * 1.00) as MONEY) as PeriodAvgTermSett,
		SUM(AgrmtTotal) as PeriodAgrmtTotalSettled
	
	from #delivered d INNER JOIN status s on d.acctno = s.acctno and d.currstatus=s.statuscode
	 and d.currstatus='S' and s.CurrentStatus='Y'
	 and s.datestatchge <= @SettToDate	 
	group by BranchNo,d.termstype,ScoringBand		-- 14/06/10
	order by BranchNo,d.termstype,ScoringBand		-- 14/06/10
	
	-- by Country
	insert into #PeriodSett 	
	select 0,0,d.termstype,ScoringBand,COUNT(*) as NumSett,	-- 14/06/10
		SUM(DATEDIFF(mm,[datefirst],DATEADD(d,1,s.datestatchge))) as PeriodTotSetTerm, 
		CAST(SUM(DATEDIFF(mm,[datefirst],DATEADD(d,1,s.datestatchge)))/(COUNT(*) * 1.00) as MONEY) as PeriodAvgTermSett,
		SUM(AgrmtTotal) as PeriodAgrmtTotalSettled
	
	from #delivered d INNER JOIN status s on d.acctno = s.acctno and d.currstatus=s.statuscode
	 and d.currstatus='S' and s.CurrentStatus='Y'
	 and s.datestatchge <= @SettToDate	 
	group by d.termstype,ScoringBand		-- 14/06/10
	order by d.termstype,ScoringBand		-- 14/06/10
	
	-- All settled	
	UPDATE #Delsum 
		set NumSettled=NumSett,TermSettled=TotSetTerm,AvgTermSettled=AvgTermSett
	from #sett s INNER JOIN #Delsum d on s.BranchNo = d.BranchNo and d.userid = s.userid and s.termstype = d.termstype and s.ScoringBand = d.ScoringBand	-- 14/06/10
	
	-- settled in period
	UPDATE #Delsum 
		set PeriodNumSettled=NumSett,PeriodTermSettled=PeriodTotSetTerm,PeriodAvgTermSettled=PeriodAvgTermSett
	from #PeriodSett s INNER JOIN #Delsum d on s.BranchNo = d.BranchNo and d.userid = s.userid and s.termstype = d.termstype and s.ScoringBand = d.ScoringBand	-- 14/06/10
	  
	-- % Agreement Total 
	
	Declare @TotAgreement TABLE (BranchNo INT,userid varchar(150), TotAgreementTotal MONEY,TotAccounts int)
	Insert into @TotAgreement	
	select BranchNo,userid,SUM(AgrmtTotal),SUM(NumAccts) from #Delsum group by BranchNo, userid
	
	Declare @TotAgreementTermsType TABLE (BranchNo INT,userid varchar(150),TermsType VARCHAR(3),TotAgreementTotal MONEY,TotAccounts int) -- 26/05/10
	Insert into @TotAgreementTermsType	
	select BranchNo,userid,TermsType,SUM(AgrmtTotal),SUM(NumAccts) from #Delsum group by BranchNo,TermsType,userid
	
	-- Settled 
	Declare @SettAgreementTermsType TABLE (BranchNo INT,userid varchar(150),TermsType VARCHAR(3),SettAgreementTotal MONEY,SettAccounts int) -- 26/05/10
	Insert into @SettAgreementTermsType	
	select BranchNo,userid,TermsType,SUM(AgrmtTotalSettled),SUM(NumAccts) from #Delsum group by BranchNo,TermsType,userid
	
	UPDATE #Delsum
		set PctAgrTot=AgrmtTotal/(TotAgreementTotal * 1.00),PctAccounts=NumAccts/(TotAccounts * 1.00)
	from #Delsum d INNER JOIN @TotAgreement t on d.branchno=t.branchno and d.userid = t.userid
	
	-- Weighted Terms
	
	Declare @WghtTotAgreement TABLE (BranchNo INT,userid varchar(150),termsType VARCHAR(3),ScoringBand CHAR(1),TotAgreementTotal MONEY,TotAgreementTotalSett MONEY) -- 26/05/10 --21/06/10
	Insert into @WghtTotAgreement	
	select BranchNo,userid,termsType,ScoringBand,TotAgreement=SUM(AgrmtTotal),SUM(AgrmtTotalSettled) from #Delsum 
	group by BranchNo,userid,termsType,ScoringBand
	
	-- By Employee
	Declare @WeightAvg TABLE (BranchNo INT,userid varchar(150),Termstype varCHAR(3),ScoringBand CHAR(1),TermAgrTot money,SettAgrTot money)	--21/06/10
	Insert into @WeightAvg	
	select 0,userid,Termstype,ScoringBand,SUM(instalno*Agrmttotal),SUM(SettInstNo*SettledAgrmtTotal)	-- 26/05/10 --21/06/10
	from #delivered d
	group by userid,termstype,ScoringBand		--21/06/10
	order by userid,termstype,ScoringBand
	
	-- By Branch
	
	Insert into @WeightAvg	
	select branchno, 0,Termstype,ScoringBand,SUM(instalno*Agrmttotal),SUM(SettInstNo*SettledAgrmtTotal)	-- 26/05/10 --21/06/10
	from #delivered d
	group by d.BranchNo,termstype,ScoringBand		--21/06/10
	order by d.BranchNo,termstype,ScoringBand
	
	
	-- By Country	
	Insert into @WeightAvg	
	select 0,0,Termstype,ScoringBand,SUM(instalno*Agrmttotal),SUM(SettInstNo*SettledAgrmtTotal)	-- 26/05/10
	from #delivered d
	group by termstype,ScoringBand
	
	order by termstype
		
	UPDATE #Delsum
		set WghtAvgTerm=Case when TotAgreementTotal=0 then 0 else TermAgrTot/TotAgreementTotal end,
			WghtAvgTermSettled=Case when TotAgreementTotalSett=0 then 0 else SettAgrTot/TotAgreementTotalSett end,	-- 26/05/10
			WgtTermAgrTot=w.TermAgrTot,WgtSettAgrTot=w.SettAgrTot		
	from #Delsum d INNER JOIN @WghtTotAgreement t on d.BranchNo = t.BranchNo and d.userid =t.userid and d.termstype = t.termstype  and d.ScoringBand = t.ScoringBand	--21/06/10
				   INNER JOIN @WeightAvg w on d.BranchNo = w.BranchNo and d.userid =w.userid and d.termstype = w.termstype  and d.ScoringBand = w.ScoringBand	--21/06/10
	
	-- Unearned Income
	-- By employee	
	select 0 as branchno,d.userid,d.termstype,ScoringBand,	-- 14/06/10
			SUM(d.ServiceChg) as ServiceChg,SUM(ISNULL(r.Rebate,0)) as Rebate,
			SUM(ISNULL(f.transvalue,0)) as RebPaid
	into #Rebates
	from #delivered d LEFT OUTER JOIN rebates r on d.acctno = r.acctno
					LEFT Outer JOIN fintrans f on d.acctno=f.acctno and f.transtypecode='REB'
	group by d.userid,d.termstype,ScoringBand		-- 14/06/10
	order by d.userid,d.termstype,ScoringBand		-- 14/06/10
	
	-- By Branch	
	insert into #Rebates
	select d.BranchNo,0,d.termstype,ScoringBand,	-- 14/06/10
			SUM(d.ServiceChg) as ServiceChg,SUM(ISNULL(r.Rebate,0)) as Rebate,
			SUM(ISNULL(f.transvalue,0)) as RebPaid
	from #delivered d LEFT OUTER JOIN rebates r on d.acctno = r.acctno
					LEFT Outer JOIN fintrans f on d.acctno=f.acctno and f.transtypecode='REB'
	group by d.branchno,d.userid,d.termstype,ScoringBand		-- 14/06/10
	order by d.branchno,d.userid,d.termstype,ScoringBand		-- 14/06/10
	
	
	-- By Country
	insert into #Rebates	
	select 0,0,d.termstype,ScoringBand,		-- 14/06/10
			SUM(d.ServiceChg) as ServiceChg,SUM(ISNULL(r.Rebate,0)) as Rebate,
			SUM(ISNULL(f.transvalue,0)) as RebPaid
	from #delivered d LEFT OUTER JOIN rebates r on d.acctno = r.acctno
					LEFT Outer JOIN fintrans f on d.acctno=f.acctno and f.transtypecode='REB'
	group by d.termstype,ScoringBand	-- 14/06/10
	order by d.termstype,ScoringBand	-- 14/06/10
	
	UPDATE #Delsum
		set RebatesPaid=RebPaid,UnearnedIncome=Rebate,EarnedIncome=d.ServiceChg+RebPaid-Rebate
	from #Delsum d INNER JOIN #Rebates r on d.BranchNo = r.BranchNo and d.userid = r.userid and d.termstype = r.termstype and d.ScoringBand = r.ScoringBand		-- 21/06/10
	
	-- Return data
insert into summary18 (Branchno,userid, Employee,[Option],storetype,NumberOfAccounts,PctAccounts,AverageTerm,
AverageTermSettled,AccountsSettledInPeriod,AverageTermSettledInPeriod,TotalServiceCharge,
AgreementTotal,PctAgreementTotal,WeightedAverageTerm,WeightedAverageTermSettled,TotalRebatesPaid,
UnearnedIncome,EarnedIncome,BranchName,TotalTerm,NumSettled,TermSettled,WgtTermAgrTot,WgtSettAgrTot,
AgrmtTotalSettled,PeriodTermSettled,PeriodNumSettled,ScoringBand,TotCashPrice,IntRate,PeriodSettledAgrmtTotal)

	select d.branchno,d.userid,isnull(c.empeename, ''),ISNULL(T.termstype,'') +': '+ISNULL(T.Description,'Unknown') as [Option], isnull(b.StoreType, 'B'), NumAccts as NumberOfAccounts,PctAccounts,AvgTerm as AverageTerm,
			AvgTermSettled as AverageTermSettled,PeriodNumSettled as AccountsSettledInPeriod,
			PeriodAvgTermSettled as AverageTermSettledInPeriod,		-- 26/05/10 
			ServiceChg as TotalServiceCharge,AgrmtTotal as AgreementTotal,
			PctAgrTot as PctAgreementTotal,WghtAvgTerm as WeightedAverageTerm,
			WghtAvgTermSettled as WeightedAverageTermSettled,ABS(RebatesPaid) as TotalRebatesPaid,
			UnearnedIncome,EarnedIncome,ISNULL(Branchname,'Company') as BranchName,TotalTerm,NumSettled,TermSettled,
	-- #10599 Note! =replace(iif(sum(Fields!AgrmtTotalSettled.Value) = 0," ",Format(Sum(Fields!WgtSettAgrTot.Value)/iif(sum(Fields!AgrmtTotalSettled.Value)= 0,1,sum(Fields!AgrmtTotalSettled.Value)),"N2")),"NaN"," ")
	-- expression used in report to avoid #error/Nan displaying in "Weighted Average Term Settled" column due to RS2008 error
			WgtTermAgrTot,WgtSettAgrTot,AgrmtTotalSettled,PeriodTermSettled,PeriodNumSettled,		
			ScoringBand,TotCashPrice,IntRate,PeriodSettledAgrmtTotal	-- 14/06/10
			
	From #Delsum d LEFT OUTER JOIN Termstype t on d.termstype=t.TermsType
					LEFT OUTER JOIN Branch b on d.branchno=b.branchno
					left outer join courtsperson c on c.userid = d.userid
	order by d.termstype




GO

