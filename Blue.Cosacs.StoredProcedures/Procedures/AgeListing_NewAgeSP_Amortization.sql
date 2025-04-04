IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[AgeListing_NewAgeSP_Amortization]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[AgeListing_NewAgeSP_Amortization]

GO 
----exec [AgeListing_NewAgeSP_Amortization] '0','ALL','ALL','0' 
 Create PROCEDURE [dbo].[AgeListing_NewAgeSP_Amortization]
 --exec AgeListing_NewAgeSP_Amortization @branchno=N'0',@statuscodeband=N'All',@accounttype=N'C',@arrearsgroup=0
-- =============================================
-- Author:		John Croft
--				(loosely based on 69066_jam_report11_breakdown.sql)
-- Create date: 17th September 2008
-- Description:	Age Listing Report (New Ageing)
--
--	Retrieves accounts that have been in arrears as analysed in the Analysis of Balances - New Ageing report
-- 
-- Change Control
-----------------
-- 07/06/10 jec CR1105 Modifications
-- 15/07/10 jec UAT213 Date first instalment calc
-- 26/10/11 jec #3910 CR1232 Cash Loan
-- 01/03/12 jec #9613 Discrepancy with Analysis of Balances NewAge report
-- 01/03/13 jec #12505 CR12418 - Account Age Listing Parameter Changes
-- 07/03/13 jec #12563 Balances for statuscodeband='St' don't match AOB report
-- =============================================
	-- Add the parameters for the stored procedure here
				@branchno varchar(4),
                @accounttype VARCHAR(5),		-- #12505
				@statuscodeband varchar(3),
                @arrearsgroup INT=0
                --@cashloan bit
               
AS
-- create table for account life bands
select distinct acctlifeband, case 
		when acctlifeband='>00-01' then -999			-- #9613 
		when acctlifeband='>01-06' then 32
		when acctlifeband='>06-12' then 184
		when acctlifeband='>12-24' then 366
		when acctlifeband='>24' then 731
		end as minlife,
		case
		when acctlifeband='>00-01' then 31
		when acctlifeband='>01-06' then 183
		when acctlifeband='>06-12' then 365
		when acctlifeband='>12-24' then 730
		when acctlifeband='>24' then 9999
		end as maxlife	
		
into #acctlife
from summary4_New_non 
where acctlifeband!=''
order by acctlifeband

-- create table for Arrears grouping
create table #arrearsGroup
(
seq int,
ArrearsGroup varchar(20),
mindays int,
maxdays int
)

insert into #arrearsGroup (seq,ArrearsGroup,mindays,maxdays)
select 1 as seq,'Current/In Advance' as ArrearsGroup,-99999 as mindays,0 as maxdays
union 
select 2 as seq,'1-30 days' as ArrearsGroup,1 as mindays,30 as maxdays
union 
select 3 as seq,'31-60 days' as ArrearsGroup,31 as mindays,60 as maxdays 
union 
select 4 as seq,'61-90 days' as ArrearsGroup,61 as mindays,90 as maxdays
union 
select 5 as seq,'91-120 days' as ArrearsGroup,91 as mindays,120 as maxdays
union 
select 6 as seq,'121-150 days' as ArrearsGroup,121 as mindays,150 as maxdays
union 
select 7 as seq,'151-180 days' as ArrearsGroup,151 as mindays,180 as maxdays
union 
select 8 as seq,'181-360 days' as ArrearsGroup,181 as mindays,360 as maxdays
union 
select 9 as seq,'360+ days' as ArrearsGroup,361 as mindays,99999 as maxdays


-- Select data for arrears group groupings
if @statuscodeband not in('6','7','8','9','Ad','Cr','Nd','Sc','Sp','St')		
Begin
	SELECT	s.acctno as AccountNumber, s.branchno as Branch, accttype as AccountType,accttypegroup as AccountTypeGroup, 
			statuscodeband as StatusCodeBand,acctlifeband as AccountLifeBand,ArrearsGroup,
			s.monthsarrears,daysarrears, 
			Case When outstbal<=0 then 0.00 else 
			(select Top 1 Isnull(OpeningBal,0) from  CLAmortizationPaymentHistory  c  where c.AcctNo=s.AcctNo )- 
			isnull((select SUM(PrevPrincipal) as PrevPrincipal  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo   group by AcctNo ),0.00 ) End as balance,
			---outstbalcorr as balance,
			-- CR1105 New Columns
			s.Termstype,s.Deposit,s.Instalamount,DATEADD(m,-(s.instalno-1),s.datelast) as DateFirstInstalment,s.datelast as DateLastInstalment,
			s.Instalno as NumberOfInstallments,s.Arrears as DelinquencyAmount,outstbal as OutstandingBalance,
			outstbal-outstbalcorr as AdditionalCharges,IsNull(Rebate,0.00) as Rebate ,Payamount as BalancePaid,
			case When outstbal<=0 then 0.00 
			When (select top 1 AdminFee from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and IsPaid ='1' order by Instalment desc ) <>'0.00' then
			isnull((select sum(Principal) as baldue12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo<=12 group by AcctNo ),0.00)
			+ Isnull( (select top 1 AdminFee from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and IsPaid ='1' ),0)
			Else isnull((select sum(Principal) as baldue12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo<=12 group by AcctNo ),0.00)
			End AS baldue12mths,
			Case When outstbal<=0 then 0.00
			Else isnull((select sum(Principal) as baldueafter12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo>12 group by AcctNo ),0.00) End as baldueafter12mths,
			IsNull(Rebatewithin12mths,0.00) as Rebatewithin12mths,IsNull(Rebateafter12mths,0.00) as Rebateafter12mths,
			case When   outstbal<=0 then 0.00 else 
			isnull((select sum(ServiceChg) as FFIWithin12Months  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo<=12 group by AcctNo ),0.00) End as  FFIWithin12Months,
			Case When outstbal<=0 then 0.00 Else 
			isnull((select sum(ServiceChg) as FFIWithin12Months  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo>12 group by AcctNo ),0.00) End as FFIAfter12Months
	FROM	summary1 s LEFT Outer JOIN rebates r on s.acctno=r.acctno ,#acctlife,#arrearsGroup
	where acctlife between minlife and maxlife
			and daysarrears between mindays and maxdays-- and S.acctno='225000300201'
			and (statuscodeband=@statuscodeband OR  (@statuscodeband= 'ALL' and statuscodeband is not null 
													-- only include non arrears group  groupings if Arrears group is ALL(0)
													and ((statuscodeband not in('Ad','Cr','Nd','Sc','Sp','St') and @arrearsgroup!=0)
														-- exclude balance between for 'St' statuscode band 
														or (@arrearsgroup=0	and ((outstbal not between -0.00999999 and 0.009999999 and statuscodeband='St') or statuscodeband!='St') )	) )	)	-- #12627 #12505			
			and (seq=@arrearsgroup or @arrearsgroup=0)				-- #12505
			and (s.branchno=@branchno	 or @branchno='0')
			and ((@accounttype='CRD' and accttypegroup in('HP','RF'))			-- #12505
					or (@accounttype='OPT' and accttypegroup in('PT','C'))
					or @accounttype = accttypegroup
					or @accounttype='ALL'
					or @accounttype='C')  And s.AcctNo in (select AcctNo from Acct where isAmortized=1 and IsAmortizedOutStandingBal=1) 
					--and ((@cashloan=0 and not exists(select * from CashLoan cl where s.acctno=cl.acctno))		-- Non Cash Loan #3910
			--		or 	(@cashloan=1 and exists(select * from CashLoan cl where s.acctno=cl.acctno)) )	-- Cash Loan
	order BY statuscodeband,acctlifeband,accttypegroup,ArrearsGroup,daysarrears,s.acctno
End

else
-- select non arrears group  groupings
Begin
	SELECT	s.acctno as AccountNumber, s.branchno as Branch, accttype as AccountType,accttypegroup as AccountTypeGroup, 
			statuscodeband as StatusCodeBand, ' ' as AccountLifeBand,' ' as ArrearsGroup,
			s.monthsarrears,daysarrears, --outstbalcorr as balance,
			 Case When outstbal<=0 then 0.00 else 
			(select Top 1 Isnull(OpeningBal,0) from  CLAmortizationPaymentHistory  c  where c.AcctNo=s.AcctNo )- 
			isnull((select SUM(PrevPrincipal) as PrevPrincipal  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo   group by AcctNo ),0.00 ) End as balance,
			-- CR1105 New Columns
			s.Termstype,s.Deposit,s.Instalamount,DATEADD(m,-(s.instalno-1),s.datelast) as DateFirstInstalment,s.datelast as DateLastInstalment,
			s.Instalno as NumberOfInstallments,s.Arrears as DelinquencyAmount,outstbal as OutstandingBalance,
			outstbal-outstbalcorr as AdditionalCharges,IsNull(Rebate,0.00) as Rebate,Payamount as BalancePaid, 
			case When outstbal<=0 then 0.00
			When (select top 1 AdminFee from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and IsPaid ='1'  order by Instalment desc ) <>'0.00' then
			isnull((select sum(Principal) as baldue12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo<=12 group by AcctNo ),0.00)
			+ Isnull( (select top 1 AdminFee from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and IsPaid ='1' ),0)
			Else isnull((select sum(Principal) as baldue12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo<=12 group by AcctNo ),0.00)
			End AS baldue12mths,
			Case When outstbal<=0 then 0.00 Else 
			isnull((select sum(Principal) as baldueafter12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo>12 group by AcctNo ),0.00) End as baldueafter12mths,
					IsNull(Rebatewithin12mths,0.00) as Rebatewithin12mths,IsNull(Rebateafter12mths,0.00) as Rebateafter12mths,
			Case When outstbal<=0 then 0.00 Else 
			isnull((select sum(ServiceChg) as FFIWithin12Months  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo<=12 group by AcctNo ),0.00)End as  FFIWithin12Months,
			Case When outstbal<=0 then 0.00 Else 
			isnull((select sum(ServiceChg) as FFIWithin12Months  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo>12 group by AcctNo ),0.00) End  as FFIAfter12Months
	FROM	summary1 s LEFT Outer JOIN rebates r on s.acctno=r.acctno 
		where (statuscodeband=@statuscodeband OR  (@statuscodeband= 'ALL' and statuscodeband is not null) )		-- #9613 
			--and ROUND(outstbalcorr,2)!=0				-- #12563 #9613
			and outstbal not between -0.00999999 and 0.009999999		-- #12563  
			and (s.branchno=@branchno	 or @branchno='0')
			and ((@accounttype='CRD' and accttypegroup in('HP','RF'))			-- #12505
					or (@accounttype='OPT' and accttypegroup in('PT','C'))
					or @accounttype = accttypegroup
					or @accounttype='ALL'
					or @accounttype='C')  And s.AcctNo in (select AcctNo from Acct where isAmortized=1 and IsAmortizedOutStandingBal=1) 
						-- and S.acctno='225000300201'
			--and ((@cashloan=0 and not exists(select * from CashLoan cl where s.acctno=cl.acctno))		-- Non Cash Loan #3910
		--		or 	(@cashloan=1 and exists(select * from CashLoan cl where s.acctno=cl.acctno)) )	-- Cash Loan
	order BY statuscodeband,accttypegroup,ArrearsGroup,daysarrears,s.acctno,FFIWithin12Months,FFIAfter12Months
End


