-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF EXISTS(select * from CountryMaintenance where CodeName = 'CL_ExistAccountLength')
BEGIN

    UPDATE 
        CountryMaintenance
    SET
        Name = 'Existing and Settled Account length',
        [Description] = 'On an existing or settled account, the minimum time period since delivery for the customer to qualify for Cash Loan.'
    WHERE
        CodeName = 'CL_ExistAccountLength'
END

--With updated definitions for Cash Loan groups, the below parameters are no longer used.

--Name of parameter: Most recent settled Credit account months.
--Description: If a customer has no current accounts, this is the maximum number of months since the most recent Credit account has been settled for the customer to qualify for Instant Credit or Cash Loan.

IF EXISTS(select * from CountryMaintenance where CodeName = 'CL_SettledCredMonths')
BEGIN

    UPDATE 
        CountryMaintenance
    SET
        [Description] = 'PARAMETER NO LONGER USED'
    WHERE
        CodeName = 'CL_SettledCredMonths'
        
END

--Name of parameter: Settled Account Length - CashLoans
--Description: If a customer is qualifying under a recent settled account the account must have been open for at least this many months
IF EXISTS(select * from CountryMaintenance where CodeName = 'CL_settledmonths')
BEGIN
      UPDATE 
        CountryMaintenance
    SET
        [Description] = 'PARAMETER NO LONGER USED'
    WHERE
        CodeName = 'CL_settledmonths'
END