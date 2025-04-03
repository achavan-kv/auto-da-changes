-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from CountryMaintenance where CodeName = 'CL_NewCustMaxLoanAmount')
BEGIN
    insert into 
        CountryMaintenance
    select 
        c.countrycode, 30, 'New Customer Maximum Loan Amount', 0, 'numeric', 0, '', '', 'Maximum Loan amount available for New Customers - no loan is available with default 0.', 'CL_NewCustMaxLoanAmount'
    from 
        country c
END
GO

IF NOT EXISTS(select * from CountryMaintenance where CodeName = 'CL_RecentCustMaxLoanAmount')
BEGIN
insert into 
        CountryMaintenance
    select 
        c.countrycode, 30, 'Recent Customer Maximum Loan Amount', 0, 'numeric', 0, '', '', 'Maximum Loan amount available for Recent Customers - no loan is available with default 0.', 'CL_RecentCustMaxLoanAmount'
    from 
        country c
END
GO

IF NOT EXISTS(select * from CountryMaintenance where CodeName = 'CL_StaffCustMaxLoanAmount')
BEGIN
insert into 
        CountryMaintenance
    select 
        c.countrycode, 30, 'Staff Customer Maximum Loan Amount', 0, 'numeric', 0, '', '', 'Maximum Loan amount available for Staff Customers - no loan is available with default 0.', 'CL_StaffCustMaxLoanAmount'
    from 
        country c
END
GO

IF NOT EXISTS(select * from CountryMaintenance where CodeName = 'CL_RecentCustAccountLength')
BEGIN
insert into 
        CountryMaintenance
    select 
        c.countrycode, 30, 'Recent Account length', 2, 'numeric', 0, '', '', 'On a recent account, the minimum time period since delivery for Cash Loan.', 'CL_RecentCustAccountLength'
    from 
        country c
END
GO

IF NOT EXISTS(select * from CountryMaintenance where CodeName = 'CL_HighSettStatRecentCust')
BEGIN
insert into 
        CountryMaintenance
    select 
        c.countrycode, 30, 'Highest status of any recent customers', 2, 'numeric', 0, '', '', 'The maximum status of any current account or settled account for recent customers settled within the "time frame". A status higher than this will prevent cash loan qualification.', 'CL_HighSettStatRecentCust'
    from 
        country c
END
GO

IF NOT EXISTS(select * from CountryMaintenance where CodeName = 'CL_RecentCustMaxArrearsLevel')
BEGIN
insert into 
        CountryMaintenance
    select 
        c.countrycode, 30, 'Maximum arrears level for recent customers', 2, 'numeric', 0, '', '', 'Maximum number of instalments in arrears for recent customer accounts. Any account more than this in arrears will prevent cash loan qualification', 'CL_RecentCustMaxArrearsLevel'
    from 
        country c
END
GO