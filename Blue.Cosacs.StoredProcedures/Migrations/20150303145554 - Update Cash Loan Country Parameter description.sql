-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF EXISTS(select * from countrymaintenance where codename = 'CL_MaxLoanAmount')
BEGIN
    UPDATE
        CountryMaintenance
    SET
        Name = 'Existing Customer Maximum Loan Amount'
    WHERE
        CodeName = 'CL_MaxLoanAmount'
END
GO

IF EXISTS(select * from countrymaintenance where codename = 'CL_HighSettStatRecentCust')
BEGIN
    UPDATE
        CountryMaintenance
    SET
        Name = 'Highest status of any Recent Customers',
        [Description] = 'The maximum status of any current account or settled account for Recent Customers settled within the "time frame". A status higher than this will prevent cash loan qualification.'
    WHERE
        CodeName = 'CL_HighSettStatRecentCust'
END
GO

IF EXISTS(select * from countrymaintenance where codename = 'CL_RecentCustMaxArrearsLevel')
BEGIN
    UPDATE
        CountryMaintenance
    SET
        Name = 'Maximum arrears level for Recent Customers',
        [Description] = 'Maximum number of instalments in arrears for Recent Customer accounts. Any account more than this in arrears will prevent cash loan qualification'
    WHERE
        CodeName = 'CL_RecentCustMaxArrearsLevel'
END
GO



