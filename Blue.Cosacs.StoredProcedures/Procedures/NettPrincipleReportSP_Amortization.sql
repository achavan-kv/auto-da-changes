 IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[NettPrincipleReportSP_Amortization]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[NettPrincipleReportSP_Amortization]

GO

--exec NettPrincipleReportSP_Amortization '121,126,226,783','ALL','ALL'
Create PROCEDURE [dbo].[NettPrincipleReportSP_Amortization]  --' All'    
-- =============================================    
-- Author:  John Croft    
--        
-- Create date: 15th September 2008    
-- Description: Nett Principle Report    
--    
-- Retrieves data for the Nett Principle Report from     
--  Summary1 and Rebates tables    
--    
-- Change Control    
-----------------    
-- 21/08/09 jec UAT123 Nett Principle not matching AOB New ageing
-- 27/10/11 jec CR1232 Cash Loans 
-- 11/10/12 jec #10138 Allow for Cash Loan accounttype group
-- 05/12/12 jec #11764 Allow for Singer Account accounttype group   
-- =============================================    
 -- Add the parameters for the stored procedure here    
    @branchlist VARCHAR(500),
    @AccountType VARCHAR(3),
    @cashLoanCustomerGroup VARCHAR(33)
    
AS    
    
declare @placefrom SMALLINT,    
  @BranchNo SMALLINT,      
  --@branchlist VARCHAR(500),  --testing only    
  @placeto SMALLINT    

--Get Cash Loan groups details
DECLARE @newCustomer TINYINT = 255,
        @recentCustomer TINYINT = 255,
        @existingCustomer TINYINT = 255,
        @staffCustomer TINYINT = 255

--Determine which Cash Loan Custoemrs/Accounts to show
    IF @cashLoanCustomerGroup LIKE '%New%'
        SET @newCustomer = 1
    IF @cashLoanCustomerGroup LIKE '%Recent%'
        SET @recentCustomer = 1
    IF @cashLoanCustomerGroup LIKE '%Existing%'
        SET @existingCustomer = 1
    IF @cashLoanCustomerGroup LIKE '%Staff%'
        SET @staffCustomer = 1
    
CREATE TABLE #branchlist (branchno INT)  -- Table to hold branches to be processed    
    
if  @branchlist!= ' All'     
Begin    

-- new function for splitting branches 
			insert into #branchlist
			select * from dbo.split(@branchlist,',')     
End    
else    
BEGIN    
 insert into #branchlist    
 select branchno  from branch     
END       
    -- get data   
	select    a.sequence,a.ArrearsBand,sum(a.baldue12mths) as baldue12mths,sum(a.baldueafter12mths) as baldueafter12mths,sum(rebatewithin12mths) as rebatewithin12mths,sum(rebateafter12mths) as rebateafter12mths,sum(FIwithin12Month) as FIwithin12Month,sum(FIAfter24Month) as FIAfter24Month into #nettPrinciple  from  
  ( 
  select s.acctno,     
  case     
   when daysarrears <= 0 then 10    
   when daysarrears <= 30 then 20    
   when daysarrears <= 60 then 30    
   when daysarrears <= 90 then 40    
   when daysarrears <= 120 then 50    
   when daysarrears <= 150 then 60    
   when daysarrears <= 180 then 70    
   when daysarrears <= 360 then 80    
   when daysarrears > 360 then 90    
   else 100    
   end as sequence,    
  case     
   when daysarrears <= 0 then 'Current (in advance or no arrears)'    
   when daysarrears <= 30 then '1-30 days (up to one month in arrears)'    
   when daysarrears <= 60 then '31-60 days'    
   when daysarrears <= 90 then '61-90 days'    
   when daysarrears <= 120 then '91-120 days'    
   when daysarrears <= 150 then '121-150 days'    
   when daysarrears <= 180 then '151-180 days'    
   when daysarrears <= 360 then '181-360 days'    
   when daysarrears > 360 then '361+ days'    
   else 'Other'    
   end as ArrearsBand,
  Case When s.outstbal<=0 then 0.00 Else  
  isnull((select sum(Principal) as baldue12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo<=12 group by AcctNo ),0.00)	End  AS baldue12mths,
  Case When s.outstbal<=0 then 0.00 Else    
 isnull((select sum(Principal) as baldueafter12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo>12 group by AcctNo ),0.00)  End as baldueafter12mths,   
 sum(isnull(r.rebatewithin12mths,0)) as rebatewithin12mths,sum(isnull(r.rebateafter12mths,0)) as rebateafter12mths, 
    Case When s.outstbal<=0 then 0.00 Else    
 (select  sum(ServiceChg)   from CLAmortizationPaymentHistory t where t.AcctNo=s.acctno and InstallmentNo<=12  )  End as FIwithin12Month,
    Case When outstbal<=0 then 0.00 Else    
 (select  sum(ServiceChg)   from CLAmortizationPaymentHistory t where t.AcctNo=s.acctno and InstallmentNo>12  )  End as FIAfter24Month
  --into #nettPrinciple    
 from summary1 s 
 LEFT OUTER JOIN rebates r 
 ON s.acctno = r.acctno    
 INNER JOIN #branchlist b 
 ON s.branchno=b.branchno   
 WHERE statuscodeband in ('1-2','3','4','5','6','7','8','9', 'StC') 
    AND accttypegroup in ('HP','RF', 'SC','CLN','SGR')   And s.AcctNo in (select AcctNo from Acct where isAmortized=1 and IsAmortizedOutStandingBal=1) 
    AND outstbal not between -0.00999999 and 0.009999999    
    AND currstatus!='S'
    AND (@AccountType='All'	-- All accounts CR1232
		 OR (@AccountType='CR' and not exists(select * from CashLoan cl where s.acctno=cl.acctno) and s.accttypegroup in('HP','RF'))		-- Non Cash Loan CR1232
		 OR (@AccountType='CL' and exists(select * from CashLoan cl where s.acctno=cl.acctno)) -- Cash Loan
		 OR (@AccountType='SGR' and exists(select * from SingerAcct sa where s.acctno=sa.acctno)) -- #11764 Singer Account    
		 OR (@AccountType='SC' and s.acctno like '___9%'))	  --store card   
    AND s.termstype IN (SELECT tt.TermsType
                        FROM TermsType tt
                        WHERE (@cashLoanCustomerGroup LIKE '%All%'
                               OR tt.LoanNewCustomer = @newCustomer
                               OR tt.LoanRecentCustomer = @recentCustomer
                               OR tt.LoanExistingCustomer = @existingCustomer
                               OR tt.LoanStaff = @staffCustomer)
                       ) 
 group by case     
   when daysarrears <= 0 then 10    
   when daysarrears <= 30 then 20    
   when daysarrears <= 60 then 30    
   when daysarrears <= 90 then 40    
   when daysarrears <= 120 then 50    
   when daysarrears <= 150 then 60    
   when daysarrears <= 180 then 70    
   when daysarrears <= 360 then 80    
   when daysarrears > 360 then 90    
   else 100    
   end, case     
   when daysarrears <= 0 then 'Current (in advance or no arrears)'    
   when daysarrears <= 30 then '1-30 days (up to one month in arrears)'    
   when daysarrears <= 60 then '31-60 days'    
   when daysarrears <= 90 then '61-90 days'    
   when daysarrears <= 120 then '91-120 days'    
   when daysarrears <= 150 then '121-150 days'    
   when daysarrears <= 180 then '151-180 days'    
   when daysarrears <= 360 then '181-360 days'    
   when daysarrears > 360 then '361+ days'    
   else 'Other'    
   end, s.acctno,outstbal  
   ) a   group by a.sequence,a.ArrearsBand 
 
  --select * from #nettPrinciple    
 ---- Ad - (special case)    
 insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths,FIwithin12Month,FIAfter24Month)    
 select a.sequence,a.arrearsband,sum(a.baldue12mths) as baldue12mths,sum(a.baldueafter12mths) as baldueafter12mths,sum(a.rebatewithin12mths) as rebatewithin12mths,sum(a.rebateafter12mths) as rebateafter12mths ,sum(a.FIwithin12Month) as FIwithin12Month,sum(a.FIAfter24Month) as FIAfter24Month  
 from (
 select 150 as sequence,'Ad' as arrearsband,s.acctno, --StatusCodeBand,    
  0 as baldue12mths,0 as baldueafter12mths,0 as rebatewithin12mths,0 as rebateafter12mths,  
   --Case When s.outstbal<=0 then 0.00 Else   
  --(select  sum(ServiceChg)   from CLAmortizationPaymentHistory t where t.AcctNo=s.acctno and InstallmentNo<=12  )     as FIwithin12Month,
  0 as FIwithin12Month,
   --Case When s.outstbal<=0 then 0.00 Else   
  --(select  sum(ServiceChg)   from CLAmortizationPaymentHistory t where t.AcctNo=s.acctno and InstallmentNo>12  )    as FIAfter24Month 
  0 as FIAfter24Month 
 from summary1 s 
 LEFT OUTER JOIN rebates r 
 ON s.acctno = r.acctno    
 INNER JOIN #branchlist b 
 ON s.branchno=b.branchno    
 WHERE (currstatus <> 'S' or(currstatus = 'S' and outstbal <> 0)) 
    AND outstbal NOT BETWEEN -0.0099999 AND 0.0099999      And s.AcctNo in (select AcctNo from Acct where isAmortized=1 and IsAmortizedOutStandingBal=1) 
    AND accttypegroup IN('HP','RF','CLN','SGR')		-- #11764  	 
    AND (@AccountType='All'	-- All accounts CR1232
		 OR (@AccountType='CR' and not exists(select * from CashLoan cl where s.acctno=cl.acctno) and s.accttypegroup in('HP','RF'))		-- Non Cash Loan CR1232
		 OR (@AccountType='SGR' and exists(select * from SingerAcct sa where s.acctno=sa.acctno)) -- #11764 Singer Account    
		 OR (@AccountType='CL' and exists(select * from CashLoan cl where s.acctno=cl.acctno)) )	-- Cash Loan  
    AND s.termstype IN (SELECT tt.TermsType
                        FROM TermsType tt
                        WHERE (@cashLoanCustomerGroup LIKE '%All%'
                               OR tt.LoanNewCustomer = @newCustomer
                               OR tt.LoanRecentCustomer = @recentCustomer
                               OR tt.LoanExistingCustomer = @existingCustomer
                               OR tt.LoanStaff = @staffCustomer)
                       ) group by s.acctno 
					   ) a
					   Group by sequence,arrearsband 

 ---- insert row if none exists    
 if @@rowcount=0    
 BEGIN    
  insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths,FIwithin12Month,FIAfter24Month)    
  select 150,'Ad',0,0,0,0,0,0    
 END     
    
 ---- Cr    
 insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths,FIwithin12Month,FIAfter24Month)    
 select sequence,arrearsband,sum(a.baldue12mths) as baldue12mths,sum(a.baldueafter12mths) as baldueafter12mths,sum(a.rebatewithin12mths) as rebatewithin12mths,sum(a.rebateafter12mths) as rebateafter12mths ,sum(a.FIwithin12Month) as FIwithin12Month,sum(a.FIAfter24Month) as FIAfter24Month 
 from (
 select 160 as sequence,StatusCodeBand as arrearsband ,s.acctno,    
   isnull((select sum(Principal) as baldue12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo<=12 group by AcctNo ),0.00)	AS baldue12mths,
   isnull((select sum(Principal) as baldueafter12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo>12 group by AcctNo ),0.00) as baldueafter12mths, 
  sum(isnull(r.rebatewithin12mths,0)) as rebatewithin12mths,sum(isnull(r.rebateafter12mths,0)) as rebateafter12mths,
    
  (select  sum(ServiceChg)   from CLAmortizationPaymentHistory t where t.AcctNo=s.acctno and InstallmentNo<=12  )   as FIwithin12Month,
 
  (select  sum(ServiceChg)   from CLAmortizationPaymentHistory t where t.AcctNo=s.acctno and InstallmentNo>12  )   as FIAfter24Month   
 FROM summary1 s 
 LEFT OUTER JOIN rebates r 
 ON s.acctno = r.acctno    
 INNER JOIN #branchlist b 
 ON s.branchno=b.branchno    
 WHERE StatusCodeBand = 'Cr' 
    AND accttypegroup IN ('HP','RF','CLN','SGR')		-- #11764 	   
    AND currstatus!='S'      And s.AcctNo in (select AcctNo from Acct where isAmortized=1 and IsAmortizedOutStandingBal=1) 
    AND outstbal NOT BETWEEN -0.00999999 AND 0.009999999 
    AND (@AccountType='All'	-- All accounts CR1232
		 OR (@AccountType='CR' and not exists(select * from CashLoan cl where s.acctno=cl.acctno) and s.accttypegroup in('HP','RF'))		-- Non Cash Loan CR1232
		 OR	(@AccountType='SGR' and exists(select * from SingerAcct sa where s.acctno=sa.acctno)) -- #11764 Singer Account    
		 OR	(@AccountType='CL' and exists(select * from CashLoan cl where s.acctno=cl.acctno)) )	-- Cash Loan  
    AND s.termstype IN (SELECT tt.TermsType
                        FROM TermsType tt
                        WHERE (@cashLoanCustomerGroup LIKE '%All%'
                               OR tt.LoanNewCustomer = @newCustomer
                               OR tt.LoanRecentCustomer = @recentCustomer
                               OR tt.LoanExistingCustomer = @existingCustomer
                               OR tt.LoanStaff = @staffCustomer)
                       )
 group by StatusCodeBand,s.acctno
 ) a group by sequence,arrearsband 
     
 ------ insert row if none exists    
 if @@rowcount=0    
 BEGIN    
  insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths,FIwithin12Month,FIAfter24Month)    
  select 160,'Cr',0,0,0,0,0,0    
 END    
     
 ---- Nd    
 insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths,FIwithin12Month,FIAfter24Month)    
 select sequence,arrearsband,sum(a.baldue12mths) as baldue12mths,sum(a.baldueafter12mths) as baldueafter12mths,sum(a.rebatewithin12mths) as rebatewithin12mths,sum(a.rebateafter12mths) as rebateafter12mths ,sum(a.FIwithin12Month) as FIwithin12Month,sum(a.FIAfter24Month) as FIAfter24Month 
 from (
 select 170 as sequence,StatusCodeBand as arrearsband ,s.acctno,
       
  isnull((select sum(Principal) as baldue12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo<=12 group by AcctNo ),0.00)  	AS baldue12mths,
    
  isnull((select sum(Principal) as baldueafter12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo>12 group by AcctNo ),0.00)   as baldueafter12mths,  
  sum(isnull(r.rebatewithin12mths,0)) as rebatewithin12mths,sum(isnull(r.rebateafter12mths,0)) as rebateafter12mths,
 
  (select  sum(ServiceChg)   from CLAmortizationPaymentHistory t where t.AcctNo=s.acctno and InstallmentNo<=12  )    as FIwithin12Month,
 
  (select  sum(ServiceChg)   from CLAmortizationPaymentHistory t where t.AcctNo=s.acctno and InstallmentNo>12  )   as FIAfter24Month       
 FROM summary1 s 
 LEFT OUTER JOIN rebates r 
 ON s.acctno = r.acctno    
 INNER JOIN #branchlist b 
 ON s.branchno=b.branchno    
 WHERE StatusCodeBand='Nd' 
    AND accttypegroup IN ('HP','RF','CLN','SGR')		-- #11764 	  
    AND outstbal NOT BETWEEN -0.00999999 AND 0.009999999    
    AND currstatus!='S'  And s.AcctNo in (select AcctNo from Acct where isAmortized=1 and IsAmortizedOutStandingBal=1) 
    AND (@AccountType='All'	-- All accounts CR1232
		 OR (@AccountType='CR' and not exists(select * from CashLoan cl where s.acctno=cl.acctno) and s.accttypegroup in('HP','RF'))		-- Non Cash Loan CR1232
		 OR (@AccountType='SGR' and exists(select * from SingerAcct sa where s.acctno=sa.acctno)) -- #11764 Singer Account    
		 OR (@AccountType='CL' and exists(select * from CashLoan cl where s.acctno=cl.acctno)) )	-- Cash Loan    
    AND s.termstype IN (SELECT tt.TermsType
                        FROM TermsType tt
                        WHERE (@cashLoanCustomerGroup LIKE '%All%'
                               OR tt.LoanNewCustomer = @newCustomer
                               OR tt.LoanRecentCustomer = @recentCustomer
                               OR tt.LoanExistingCustomer = @existingCustomer
                               OR tt.LoanStaff = @staffCustomer)
                       )
 group by StatusCodeBand , S.acctno 
 ) a group by  sequence,arrearsband  

 ---- insert row if none exists    
 if @@rowcount=0    
 BEGIN    
  insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths,FIwithin12Month,FIAfter24Month)    
  select 170,'Nd',0,0,0,0,0,0    
 END    
      
 ---- Sc    
 insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths,FIwithin12Month,FIAfter24Month)    
 select  sequence,arrearsband,sum(a.baldue12mths) as baldue12mths,sum(a.baldueafter12mths) as baldueafter12mths,sum(a.rebatewithin12mths) as rebatewithin12mths,sum(a.rebateafter12mths) as rebateafter12mths ,sum(a.FIwithin12Month) as FIwithin12Month,sum(a.FIAfter24Month) as FIAfter24Month 
  from ( 
 select 180 as sequence,StatusCodeBand as arrearsband,s.acctno ,  
   
  isnull((select sum(Principal) as baldue12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo<=12 group by AcctNo ),0.00)   AS baldue12mths,
   
  isnull((select sum(Principal) as baldueafter12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo>12 group by AcctNo ),0.00)   as baldueafter12mths,  
  sum(isnull(r.rebatewithin12mths,0)) as rebatewithin12mths,sum(isnull(r.rebateafter12mths,0)) as rebateafter12mths,
   
 (select  sum(ServiceChg)   from CLAmortizationPaymentHistory t where t.AcctNo=s.acctno and InstallmentNo<=12  )   as FIwithin12Month,
   
 (select  sum(ServiceChg)   from CLAmortizationPaymentHistory t where t.AcctNo=s.acctno and InstallmentNo>12  )   as FIAfter24Month          
 FROM summary1 s 
 LEFT OUTER JOIN rebates r 
 ON s.acctno = r.acctno    
 INNER JOIN #branchlist b 
 ON s.branchno=b.branchno    
 WHERE StatusCodeBand='Sc' 
    AND accttypegroup IN ('HP','RF','CLN','SGR')		-- #11764 	  
    AND outstbal NOT BETWEEN -0.00999999 AND 0.009999999   
    AND currstatus!='S'   And s.AcctNo in (select AcctNo from Acct where isAmortized=1 and IsAmortizedOutStandingBal=1) 
    AND (@AccountType='All'	-- All accounts CR1232
		 OR (@AccountType='CR' and not exists(select * from CashLoan cl where s.acctno=cl.acctno) and s.accttypegroup in('HP','RF'))		-- Non Cash Loan CR1232
		 OR (@AccountType='SGR' and exists(select * from SingerAcct sa where s.acctno=sa.acctno)) -- #11764 Singer Account    
		 OR (@AccountType='CL' and exists(select * from CashLoan cl where s.acctno=cl.acctno)) )	-- Cash Loan   
    AND s.termstype IN (SELECT tt.TermsType
                        FROM TermsType tt
                        WHERE (@cashLoanCustomerGroup LIKE '%All%'
                               OR tt.LoanNewCustomer = @newCustomer
                               OR tt.LoanRecentCustomer = @recentCustomer
                               OR tt.LoanExistingCustomer = @existingCustomer
                               OR tt.LoanStaff = @staffCustomer)
                       )
 group by StatusCodeBand,s.AcctNo
 ) a group  by sequence,arrearsband 
     
 ---- insert row if none exists    
 if @@rowcount=0    
 BEGIN    
  insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths,FIwithin12Month,FIAfter24Month)    
  select 180,'Sc',0,0,0,0,0,0    
 END    
     
 ---- Sp    
 insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths,FIwithin12Month,FIAfter24Month)    
 select sequence,arrearsband,sum(a.baldue12mths) as baldue12mths,sum(a.baldueafter12mths) as baldueafter12mths,sum(a.rebatewithin12mths) as rebatewithin12mths,sum(a.rebateafter12mths) as rebateafter12mths ,sum(a.FIwithin12Month) as FIwithin12Month,sum(a.FIAfter24Month) as FIAfter24Month  
  from ( 
 select 190 as sequence,StatusCodeBand as arrearsband,s.acctno, 
       
  isnull((select sum(Principal) as baldue12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo<=12 group by AcctNo ),0.00)  	AS baldue12mths,
 
  isnull((select sum(Principal) as baldueafter12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo>12 group by AcctNo ),0.00)    as baldueafter12mths,     
  sum(isnull(r.rebatewithin12mths,0)) as rebatewithin12mths,sum(isnull(r.rebateafter12mths,0)) as rebateafter12mths,
   
  (select  sum(ServiceChg)   from CLAmortizationPaymentHistory t where t.AcctNo=s.acctno and InstallmentNo<=12  )    as FIwithin12Month,
   
  (select  sum(ServiceChg)   from CLAmortizationPaymentHistory t where t.AcctNo=s.acctno and InstallmentNo>12  )   as FIAfter24Month         
 FROM summary1 s 
 LEFT OUTER JOIN rebates r 
 ON s.acctno = r.acctno    
 INNER JOIN #branchlist b 
 ON s.branchno=b.branchno    
 WHERE StatusCodeBand='Sp' 
    AND accttypegroup IN ('HP','RF','CLN','SGR')		-- #11764  	  -- special accounts are 'PT'    
    AND outstbal NOT BETWEEN -0.00999999 AND 0.009999999  And s.AcctNo in (select AcctNo from Acct where isAmortized=1 and IsAmortizedOutStandingBal=1) 
    AND (@AccountType='All'	-- All accounts CR1232
		 OR (@AccountType='CR' and not exists(select * from CashLoan cl where s.acctno=cl.acctno) and s.accttypegroup in('HP','RF'))		-- Non Cash Loan CR1232
		 OR (@AccountType='SGR' and exists(select * from SingerAcct sa where s.acctno=sa.acctno)) -- #11764 Singer Account    
		 OR (@AccountType='CL' and exists(select * from CashLoan cl where s.acctno=cl.acctno)) )	-- Cash Loan  
    AND s.termstype IN (SELECT tt.TermsType
                        FROM TermsType tt
                        WHERE (@cashLoanCustomerGroup LIKE '%All%'
                               OR tt.LoanNewCustomer = @newCustomer
                               OR tt.LoanRecentCustomer = @recentCustomer
                               OR tt.LoanExistingCustomer = @existingCustomer
                               OR tt.LoanStaff = @staffCustomer)
                       )
 group by StatusCodeBand,s.acctno
 ) a group by sequence,arrearsband 

 ---- insert row if none exists         
 if @@rowcount=0    
 BEGIN    
  insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths,FIwithin12Month,FIAfter24Month)    
  select 190,'Sp',0,0,0,0,0,0    
 END    
     
 ---- St    
 insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths,FIwithin12Month,FIAfter24Month)    
 select sequence,arrearsband,sum(a.baldue12mths) as baldue12mths,sum(a.baldueafter12mths) as baldueafter12mths,sum(a.rebatewithin12mths) as rebatewithin12mths,sum(a.rebateafter12mths) as rebateafter12mths ,sum(a.FIwithin12Month) as FIwithin12Month,sum(a.FIAfter24Month) as FIAfter24Month
 from (
 select 200 as sequence,StatusCodeBand as arrearsband,s.acctno,  
    
 isnull((select sum(Principal) as baldue12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo<=12 group by AcctNo ),0.00)	  AS baldue12mths,
    
 isnull((select sum(Principal) as baldueafter12mths  from CLAmortizationPaymentHistory c  where c.AcctNo=s.AcctNo and InstallmentNo>12 group by AcctNo ),0.00)   as baldueafter12mths,  
  sum(isnull(r.rebatewithin12mths,0)) as rebatewithin12mths,sum(isnull(r.rebateafter12mths,0)) as rebateafter12mths,
 
 (select  sum(ServiceChg)   from CLAmortizationPaymentHistory t where t.AcctNo=s.acctno and InstallmentNo<=12  )   as FIwithin12Month,
  
(select  sum(ServiceChg)   from CLAmortizationPaymentHistory t where t.AcctNo=s.acctno and InstallmentNo>12  )   as FIAfter24Month      
 FROM summary1 s 
 LEFT OUTER JOIN rebates r 
 ON s.acctno = r.acctno    
 INNER JOIN #branchlist b 
 ON s.branchno=b.branchno    
 WHERE StatusCodeBand='St' 
    AND accttypegroup IN ('HP','RF','CLN','SGR')		-- #11764  	   
    AND outstbal NOT BETWEEN -0.00999999 AND 0.009999999 And s.AcctNo in (select AcctNo from Acct where isAmortized=1 and IsAmortizedOutStandingBal=1) 
    AND (@AccountType='All'	-- All accounts CR1232
		 OR (@AccountType='CR' and not exists(select * from CashLoan cl where s.acctno=cl.acctno) and s.accttypegroup in('HP','RF'))		-- Non Cash Loan CR1232
		 OR (@AccountType='SGR' and exists(select * from SingerAcct sa where s.acctno=sa.acctno)) -- #11764 Singer Account    
		 OR (@AccountType='CL' and exists(select * from CashLoan cl where s.acctno=cl.acctno)) )	-- Cash Loan    
    AND s.termstype IN (SELECT tt.TermsType
                        FROM TermsType tt
                        WHERE (@cashLoanCustomerGroup LIKE '%All%'
                               OR tt.LoanNewCustomer = @newCustomer
                               OR tt.LoanRecentCustomer = @recentCustomer
                               OR tt.LoanExistingCustomer = @existingCustomer
                               OR tt.LoanStaff = @staffCustomer)
                       )
 group by StatusCodeBand,S.acctno
 ) a group by  sequence,arrearsband  

 -- -- insert row if none exists    
 if @@rowcount=0    
 BEGIN    
  insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths,FIwithin12Month,FIAfter24Month)    
  select 200,'St',0,0,0,0,0,0    
 END    
    
 ---- return data    
 select ArrearsBand,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths,isnull(FIwithin12Month,'0.00') as FIwithin12Month,isnull(FIAfter24Month,'0.00') as FIAfter24Month     
 from #nettPrinciple    
 order by sequence    

