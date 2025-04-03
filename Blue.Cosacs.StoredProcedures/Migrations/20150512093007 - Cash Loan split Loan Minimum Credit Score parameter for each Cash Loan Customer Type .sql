-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @ParameterCategory varchar(4), @DefaultCreditScore varchar(1500)

set @ParameterCategory = (select ParameterCategory from CountryMaintenance where CodeName = 'CL_LoanMinCredScore')
set @DefaultCreditScore = (select value from CountryMaintenance where CodeName = 'CL_LoanMinCredScore')


IF EXISTS(select * from CountryMaintenance where CodeName = 'CL_LoanMinCredScore')
BEGIN
    UPDATE
        CountryMaintenance
    SET
        Name = 'Existing Customer Loan Minimum Credit Score',
        [Description] = 'This is the minimum score that must be achieved for an existing customer to qualify for a Cash Loan'
    WHERE
        CodeName = 'CL_LoanMinCredScore'
END

IF NOT EXISTS(select * from CountryMaintenance where CodeName = 'CL_LoanMinCredScoreRecent')
BEGIN

    INSERT INTO 
        CountryMaintenance (CountryCode, 
                            ParameterCategory, 
                            Name, 
                            Value, 
                            [Type], 
                            [Precision], 
                            OptionCategory, 
                            OptionListName, 
                            [Description], 
                            CodeName)
    SELECT 
        c.countrycode, 
        @ParameterCategory, 
        'Recent Customer Loan Minimum Credit Score', 
        @defaultCreditScore, 
        'numeric', 
        0,
        '',
        '',
        'This is the minimum score that must be achieved for a recent customer to qualify for a Cash Loan',
        'CL_LoanMinCredScoreRecent'
    FROM
        Country c     
END

IF NOT EXISTS(select * from CountryMaintenance where CodeName = 'CL_LoanMinCredScoreNew')
BEGIN

    INSERT INTO 
        CountryMaintenance (CountryCode, 
                            ParameterCategory, 
                            Name, 
                            Value, 
                            [Type], 
                            [Precision], 
                            OptionCategory, 
                            OptionListName, 
                            [Description], 
                            CodeName)
    SELECT 
        c.countrycode, 
        @ParameterCategory, 
        'New Customer Loan Minimum Credit Score', 
        @defaultCreditScore, 
        'numeric', 
        0,
        '',
        '',
        'This is the minimum score that must be achieved for a new customer to qualify for a Cash Loan',
        'CL_LoanMinCredScoreNew'
    FROM
        Country c     
END
