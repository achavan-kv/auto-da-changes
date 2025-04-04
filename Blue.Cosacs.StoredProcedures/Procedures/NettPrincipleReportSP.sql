IF  EXISTS (SELECT 1 
	FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NettPrincipleReportSP]') 
	AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[NettPrincipleReportSP]

GO 
/****** Object:  StoredProcedure [dbo].[NettPrincipleReportSP]    Script Date: 7/15/2020 1:43:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[NettPrincipleReportSP]  --' All'    
-- =============================================    
-- Author:  John Croft    
-- ===============================================================================================
-- Version:		<002> 
-- =============================================================================================== 
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
    @branchlist VARCHAR(1000),
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
    select     
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
 sum(baldue12mths) as baldue12mths,sum(baldueafter12mths) as baldueafter12mths,    
 sum(isnull(r.rebatewithin12mths,0)) as rebatewithin12mths,sum(isnull(r.rebateafter12mths,0)) as rebateafter12mths    
 
 into #nettPrinciple    
 from summary1 s 
 LEFT OUTER JOIN rebates r 
 ON s.acctno = r.acctno    
 INNER JOIN #branchlist b 
 ON s.branchno=b.branchno    
 WHERE statuscodeband in ('1-2','3','4','5','6','7','8','9', 'StC') 
    AND accttypegroup in ('HP','RF', 'SC','CLN','SGR')		
    AND outstbal not between -0.00999999 and 0.009999999    
    AND currstatus!='S'
    AND (@AccountType='All'	-- All accounts CR1232
		 OR (@AccountType='CR' and not exists(select 1 from CashLoan cl where s.acctno=cl.acctno) and s.accttypegroup in('HP','RF'))		-- Non Cash Loan CR1232
		 OR (@AccountType='CL' and exists(select 1 from CashLoan cl where s.acctno=cl.acctno)) -- Cash Loan
		 OR (@AccountType='SGR' and exists(select 1 from SingerAcct sa where s.acctno=sa.acctno)) -- #11764 Singer Account    
		 OR (@AccountType='SC' and s.acctno like '___9%'))	  --store card   
    AND s.termstype IN (SELECT tt.TermsType
                        FROM TermsType tt
                        WHERE (@cashLoanCustomerGroup LIKE '%All%'
                               OR tt.LoanNewCustomer = @newCustomer
                               OR tt.LoanRecentCustomer = @recentCustomer
                               OR tt.LoanExistingCustomer = @existingCustomer
                               OR tt.LoanStaff = @staffCustomer)
                       )
 group by  case     
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
   end    
 order by   case     
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
   end    
      
 -- Ad - (special case)    
 insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths)    
 select 150,'Ad', --StatusCodeBand,    
  0,SUM(outstbal - outstbalcorr) as baldueafter12mths,0,0    
 from summary1 s 
 LEFT OUTER JOIN rebates r 
 ON s.acctno = r.acctno    
 INNER JOIN #branchlist b 
 ON s.branchno=b.branchno    
 WHERE (currstatus <> 'S' or(currstatus = 'S' and outstbal <> 0))
    AND outstbal NOT BETWEEN -0.0099999 AND 0.0099999    
    AND accttypegroup IN('HP','RF','CLN','SGR')		-- #11764  	 
    AND (@AccountType='All'	-- All accounts CR1232
		 OR (@AccountType='CR' and not exists(select 1 from CashLoan cl where s.acctno=cl.acctno) and s.accttypegroup in('HP','RF'))		-- Non Cash Loan CR1232
		 OR (@AccountType='SGR' and exists(select 1 from SingerAcct sa where s.acctno=sa.acctno)) -- #11764 Singer Account    
		 OR (@AccountType='CL' and exists(select 1 from CashLoan cl where s.acctno=cl.acctno)) )	-- Cash Loan  
    AND s.termstype IN (SELECT tt.TermsType
                        FROM TermsType tt
                        WHERE (@cashLoanCustomerGroup LIKE '%All%'
                               OR tt.LoanNewCustomer = @newCustomer
                               OR tt.LoanRecentCustomer = @recentCustomer
                               OR tt.LoanExistingCustomer = @existingCustomer
                               OR tt.LoanStaff = @staffCustomer)
                       )

 -- insert row if none exists    
 if @@rowcount=0    
 BEGIN    
  insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths)    
  select 150,'Ad',0,0,0,0    
 END     
    
 -- Cr    
 insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths)    
 select 160,StatusCodeBand,    
  sum(baldue12mths) as baldue12mths,sum(baldueafter12mths) as baldueafter12mths,    
  sum(isnull(r.rebatewithin12mths,0)) as rebatewithin12mths,sum(isnull(r.rebateafter12mths,0)) as rebateafter12mths    
 FROM summary1 s 
 LEFT OUTER JOIN rebates r 
 ON s.acctno = r.acctno    
 INNER JOIN #branchlist b 
 ON s.branchno=b.branchno    
 WHERE StatusCodeBand = 'Cr' 
    AND accttypegroup IN ('HP','RF','CLN','SGR')		-- #11764 	   
    AND currstatus!='S'    
    AND outstbal NOT BETWEEN -0.00999999 AND 0.009999999 
    AND (@AccountType='All'	-- All accounts CR1232
		 OR (@AccountType='CR' and not exists(select 1 from CashLoan cl where s.acctno=cl.acctno) and s.accttypegroup in('HP','RF'))		-- Non Cash Loan CR1232
		 OR	(@AccountType='SGR' and exists(select 1 from SingerAcct sa where s.acctno=sa.acctno)) -- #11764 Singer Account    
		 OR	(@AccountType='CL' and exists(select 1 from CashLoan cl where s.acctno=cl.acctno)) )	-- Cash Loan  
    AND s.termstype IN (SELECT tt.TermsType
                        FROM TermsType tt
                        WHERE (@cashLoanCustomerGroup LIKE '%All%'
                               OR tt.LoanNewCustomer = @newCustomer
                               OR tt.LoanRecentCustomer = @recentCustomer
                               OR tt.LoanExistingCustomer = @existingCustomer
                               OR tt.LoanStaff = @staffCustomer)
                       )
 group by StatusCodeBand
     
 -- insert row if none exists    
 if @@rowcount=0    
 BEGIN    
  insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths)    
  select 160,'Cr',0,0,0,0    
 END    
     
 -- Nd    
 insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths)    
 select 170,StatusCodeBand,    
  sum(baldue12mths) as baldue12mths,sum(baldueafter12mths) as baldueafter12mths,    
  sum(isnull(r.rebatewithin12mths,0)) as rebatewithin12mths,sum(isnull(r.rebateafter12mths,0)) as rebateafter12mths    
 FROM summary1 s 
 LEFT OUTER JOIN rebates r 
 ON s.acctno = r.acctno    
 INNER JOIN #branchlist b 
 ON s.branchno=b.branchno    
 WHERE StatusCodeBand='Nd' 
    AND accttypegroup IN ('HP','RF','CLN','SGR')		-- #11764 	  
    AND outstbal NOT BETWEEN -0.00999999 AND 0.009999999    
    AND currstatus!='S'
    AND (@AccountType='All'	-- All accounts CR1232
		 OR (@AccountType='CR' and not exists(select 1 from CashLoan cl where s.acctno=cl.acctno) and s.accttypegroup in('HP','RF'))		-- Non Cash Loan CR1232
		 OR (@AccountType='SGR' and exists(select 1 from SingerAcct sa where s.acctno=sa.acctno)) -- #11764 Singer Account    
		 OR (@AccountType='CL' and exists(select 1 from CashLoan cl where s.acctno=cl.acctno)) )	-- Cash Loan    
    AND s.termstype IN (SELECT tt.TermsType
                        FROM TermsType tt
                        WHERE (@cashLoanCustomerGroup LIKE '%All%'
                               OR tt.LoanNewCustomer = @newCustomer
                               OR tt.LoanRecentCustomer = @recentCustomer
                               OR tt.LoanExistingCustomer = @existingCustomer
                               OR tt.LoanStaff = @staffCustomer)
                       )
 group by StatusCodeBand    

 -- insert row if none exists    
 if @@rowcount=0    
 BEGIN    
  insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths)    
  select 170,'Nd',0,0,0,0    
 END    
      
 -- Sc    
 insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths)    
 select 180,StatusCodeBand,    
  sum(baldue12mths) as baldue12mths,sum(baldueafter12mths) as baldueafter12mths,    
  sum(isnull(r.rebatewithin12mths,0)) as rebatewithin12mths,sum(isnull(r.rebateafter12mths,0)) as rebateafter12mths    
 FROM summary1 s 
 LEFT OUTER JOIN rebates r 
 ON s.acctno = r.acctno    
 INNER JOIN #branchlist b 
 ON s.branchno=b.branchno    
 WHERE StatusCodeBand='Sc' 
    AND accttypegroup IN ('HP','RF','CLN','SGR')		-- #11764 	  
    AND outstbal NOT BETWEEN -0.00999999 AND 0.009999999   
    AND currstatus!='S' 
    AND (@AccountType='All'	-- All accounts CR1232
		 OR (@AccountType='CR' and not exists(select 1 from CashLoan cl where s.acctno=cl.acctno) and s.accttypegroup in('HP','RF'))		-- Non Cash Loan CR1232
		 OR (@AccountType='SGR' and exists(select 1 from SingerAcct sa where s.acctno=sa.acctno)) -- #11764 Singer Account    
		 OR (@AccountType='CL' and exists(select 1 from CashLoan cl where s.acctno=cl.acctno)) )	-- Cash Loan   
    AND s.termstype IN (SELECT tt.TermsType
                        FROM TermsType tt
                        WHERE (@cashLoanCustomerGroup LIKE '%All%'
                               OR tt.LoanNewCustomer = @newCustomer
                               OR tt.LoanRecentCustomer = @recentCustomer
                               OR tt.LoanExistingCustomer = @existingCustomer
                               OR tt.LoanStaff = @staffCustomer)
                       )
 group by StatusCodeBand
     
 -- insert row if none exists    
 if @@rowcount=0    
 BEGIN    
  insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths)    
  select 180,'Sc',0,0,0,0    
 END    
     
 -- Sp    
 insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths)    
 select 190,StatusCodeBand,    
  sum(baldue12mths) as baldue12mths,sum(baldueafter12mths) as baldueafter12mths,    
  sum(isnull(r.rebatewithin12mths,0)) as rebatewithin12mths,sum(isnull(r.rebateafter12mths,0)) as rebateafter12mths    
 FROM summary1 s 
 LEFT OUTER JOIN rebates r 
 ON s.acctno = r.acctno    
 INNER JOIN #branchlist b 
 ON s.branchno=b.branchno    
 WHERE StatusCodeBand='Sp' 
    AND accttypegroup IN ('HP','RF','CLN','SGR')		-- #11764  	  -- special accounts are 'PT'    
    AND outstbal NOT BETWEEN -0.00999999 AND 0.009999999 
    AND (@AccountType='All'	-- All accounts CR1232
		 OR (@AccountType='CR' and not exists(select 1 from CashLoan cl where s.acctno=cl.acctno) and s.accttypegroup in('HP','RF'))		-- Non Cash Loan CR1232
		 OR (@AccountType='SGR' and exists(select 1 from SingerAcct sa where s.acctno=sa.acctno)) -- #11764 Singer Account    
		 OR (@AccountType='CL' and exists(select 1 from CashLoan cl where s.acctno=cl.acctno)) )	-- Cash Loan  
    AND s.termstype IN (SELECT tt.TermsType
                        FROM TermsType tt
                        WHERE (@cashLoanCustomerGroup LIKE '%All%'
                               OR tt.LoanNewCustomer = @newCustomer
                               OR tt.LoanRecentCustomer = @recentCustomer
                               OR tt.LoanExistingCustomer = @existingCustomer
                               OR tt.LoanStaff = @staffCustomer)
                       )
 group by StatusCodeBand

 -- insert row if none exists         
 if @@rowcount=0    
 BEGIN    
  insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths)    
  select 190,'Sp',0,0,0,0    
 END    
     
 -- St    
 insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths)    
 select 200,StatusCodeBand,    
  sum(baldue12mths) as baldue12mths,sum(baldueafter12mths) as baldueafter12mths,    
  sum(isnull(r.rebatewithin12mths,0)) as rebatewithin12mths,sum(isnull(r.rebateafter12mths,0)) as rebateafter12mths    
 FROM summary1 s 
 LEFT OUTER JOIN rebates r 
 ON s.acctno = r.acctno    
 INNER JOIN #branchlist b 
 ON s.branchno=b.branchno    
 WHERE StatusCodeBand='St' 
    AND accttypegroup IN ('HP','RF','CLN','SGR')		-- #11764  	   
    AND outstbal NOT BETWEEN -0.00999999 AND 0.009999999
    AND (@AccountType='All'	-- All accounts CR1232
		 OR (@AccountType='CR' and not exists(select 1 from CashLoan cl where s.acctno=cl.acctno) and s.accttypegroup in('HP','RF'))		-- Non Cash Loan CR1232
		 OR (@AccountType='SGR' and exists(select 1 from SingerAcct sa where s.acctno=sa.acctno)) -- #11764 Singer Account    
		 OR (@AccountType='CL' and exists(select 1 from CashLoan cl where s.acctno=cl.acctno)) )	-- Cash Loan    
    AND s.termstype IN (SELECT tt.TermsType
                        FROM TermsType tt
                        WHERE (@cashLoanCustomerGroup LIKE '%All%'
                               OR tt.LoanNewCustomer = @newCustomer
                               OR tt.LoanRecentCustomer = @recentCustomer
                               OR tt.LoanExistingCustomer = @existingCustomer
                               OR tt.LoanStaff = @staffCustomer)
                       )
 group by StatusCodeBand    

  -- insert row if none exists    
 if @@rowcount=0    
 BEGIN    
  insert into #nettPrinciple(sequence,arrearsband,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths)    
  select 200,'St',0,0,0,0    
 END    
    
 -- return data    
 select ArrearsBand,baldue12mths,baldueafter12mths,rebatewithin12mths,rebateafter12mths    
 from #nettPrinciple    
 order by sequence    

