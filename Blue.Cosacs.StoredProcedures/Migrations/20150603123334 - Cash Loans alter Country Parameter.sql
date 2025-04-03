-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from CountryMaintenance where CodeName = 'CL_SettledCredMonths')
BEGIN

    UPDATE 
        CountryMaintenance
    SET
        Name = 'Settled Credit account months',
        [Description] = 'For an "Existing" customer that has no current accounts, they will still qualify for Cash Loan if they have a settled delivered account that was settled less than this number of months ago.'
    WHERE
        CodeName = 'CL_SettledCredMonths'
        
END
GO