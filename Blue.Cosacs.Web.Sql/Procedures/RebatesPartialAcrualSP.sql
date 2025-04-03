IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[RebatesPartialAcrualSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[RebatesPartialAcrualSP]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[RebatesPartialAcrualSP]

-- =============================================
-- Author:		John Croft
-- Create date: 5th September 2008
-- Description:	Rebates Partial Acrual
-- Version:		002
--	Show rebates on accounts as at Last Period End and Next Period End so that the exact rebates accrual upto
--  Current date can be calculated.
-- 
-- Change Control
-----------------
-- 06/03/09 jec Add Branch for grouping
-- 24/04/09 jec UAT101 Change exists command
-- 30/04/09 jec UAT101 Accrual as at run date
-- 01/05/09 jec Revised to use RebateForecast_byaccount_current table for Last Period rebate
--				(no need to recalc rebate) & makes report quicker
-- 07/12/09 jec UAT170 Add primary kety to rebates_RPA
-- 06/04/20 Revised to use RebateForecast_byaccount_current table for Last Period rebate, column name like P2
-- =============================================
	-- Add the parameters for the stored procedure here
		--@year char(4)
		@periodenddate datetime,
        @branchType CHAR(1),
        @cashLoanCustomerGroup VARCHAR(33)
			
AS
BEGIN
	set nocount on

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

    -- Heading dates
    declare @dt datetime, @chardt char(12)	
    declare @head table (lastPEdate char(12),nextPEdate char(12),currdate char(12))
    insert into @head (lastPEdate,nextPEdate,currdate) values ('','','')

    -- last PE date
    set @dt=(select Top 1 enddate 
	    from RebateForecast_PeriodEndDates
	    where rundate!=Convert(datetime,CAST('1900-01-01' as CHAR(12)),103)
	    order by Convert(datetime,CAST(enddate as CHAR(12)),103) desc)
    set @chardt=Convert(char(12),CAST(@dt as CHAR(12)),103)

    update @head
    set lastPEdate=@chardt

    -- next PE date
    set @dt=(select Top 1 enddate  
	    from RebateForecast_PeriodEndDates
	    where rundate=Convert(datetime,CAST('1900-01-01' as CHAR(12)),103)
	    and enddate!=Convert(datetime,CAST('1900-01-01' as CHAR(12)),103) order by enddate)
    set @chardt=Convert(char(12),CAST(@dt as CHAR(12)),103)

    update @head
    set nextPEdate=@chardt

    --Current date
    update @head
    set currdate=(select  Convert(char(12),CAST(getdate() as CHAR(12)),103))
    --Ensure "Current date" relative to Next Period End date (if run old old db)
    update @head
    set currdate=dateadd(mm,datepart(month,nextPEdate)-datepart(month,currdate),
				    dateadd(yy,datepart(year,nextPEdate)-datepart(year,currdate),currdate))


    ---- Save current rebates - not strictly necessary
	    if not exists (select * from dbo.sysobjects 
		    where id = object_id(N'[dbo].[rebates_RPA]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	    Begin
			    select top 1 * into rebates_RPA from rebates
			    select top 1 * into rebates_Total_RPA from Rebates_total						
	    End
	    -- Add primary key
	    IF NOT EXISTS (SELECT * FROM sysobjects  WHERE NAME = 'pk_rebates_RPA' AND TYPE = 'K')
		    ALTER TABLE rebates_RPA ADD CONSTRAINT PK_rebates_RPA PRIMARY KEY (acctno)	 

    -- clear tables
    truncate table rebates_RPA
    truncate table rebates_Total_RPA

    -- DN_RebateSP parameters
    Declare @poRebate money,
		    @poRebateWithin12Mths money,
		    @poRebateAfter12Mths money,
		    @return int,
		    @RebateDate_LastPeriodEnd datetime,
		    @RebateDate_NextPeriodEnd datetime

    --set @RebateDate_LastPeriodEnd=@periodenddate
    --		
    set @poRebate=0
    set @poRebateWithin12Mths=0
    set @poRebateAfter12Mths=0
    set @return=0
    ----set @RebateDate_April=dateadd(m,1,@RebateDate_YearEnd)
    set @RebateDate_NextPeriodEnd=(select Top 1 enddate from RebateForecast_PeriodEndDates
		    where rundate=Convert(datetime,CAST('1900-01-01' as CHAR(12)),103)
		    and enddate!=Convert(datetime,CAST('1900-01-01' as CHAR(12)),103)
		    order by enddate asc)

    -- Calculate rebates as at Next Period End
	    exec DN_RebateSP
	    @AcctNo = 'RPA', -- char
	    @poRebate = @poRebate OUTPUT,
	    @poRebateWithin12Mths = @poRebateWithin12Mths OUTPUT,
	    @poRebateAfter12Mths= @poRebateAfter12Mths OUTPUT,
	    @return =@return OUTPUT,
	    @FromDate = '01-jan-1900', 
	    @FromThresDate = '01-jan-1900', 
	    @UntilThresDate = '01-jan-1900', 
	    @RuleChangeDate = '01-apr-2002', 
	    @RebateDate = @RebateDate_NextPeriodEnd 

	    -- Acrual calculated as (31-dd)/31 * (LastPeriod rebate - NextPeriod rebate) 
    -- where dd is the due day and nn is the getdate() day and (31-dd+nn)/31 max=1


	select m.acctno as AccountNo,
           m.servicechg as 'Full Service Chg',
           m.instalno as Term,
		   DATEADD(mm,DATEDIFF(m,m.datefirst,lastPEdate)+1,m.datefirst) as NextDueDate, 
		   m.porebate_p1 as 'RebateAtLastPE', 
           ISNULL( m.porebate_p2, 0) as 'RebateAtNextPE', 
           ISNULL(a.monthsarrears, 0) as 'Month in Arrears',
		   m.datedel as DeliveryDate, 
		   case when cast((((31-datepart(dd,m.datefirst)))/31.00) as money)>1
			      then cast((m.porebate_p1 - ISNULL(a.rebate, 0)) as money)		-- full accrual
			      else cast((((31-datepart(dd,m.datefirst)))/31.00)* (m.porebate_p1 - ISNULL(a.rebate, 0)) as money)
		   end as Acrual,
		   Cast(b.branchno as char(3)) +' '+ branchname as branch,
		   h.lastPEdate,
           h.nextPEdate,
           h.currdate
	FROM @head h, 
         RebateForecast_byaccount_current m
         LEFT JOIN rebates_RPA a
         ON a.acctno = m.acctno 
	     INNER JOIN branch b 
         ON left(m.acctno,3) = b.branchno
         INNER JOIN acct ac
         ON ac.acctno = m.acctno
	WHERE m.porebate_p1>0
         AND (@branchType IS NULL 
              OR 
              b.CashLoanBranch = @branchType)
         AND ac.termstype IN (SELECT TermsType
                              FROM TermsType
                              WHERE (@cashLoanCustomerGroup LIKE '%All%'
									   OR LoanNewCustomer = @newCustomer
                                       OR LoanRecentCustomer = @recentCustomer 
                                       OR LoanExistingCustomer = @existingCustomer 
                                       OR LoanStaff = @staffCustomer)
                             )                                    
	ORDER BY m.acctno


END
GO