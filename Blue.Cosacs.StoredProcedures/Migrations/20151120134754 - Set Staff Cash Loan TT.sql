-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF (SELECT countrycode FROM country) = 'A'
BEGIN 
    UPDATE TermsTypeTable 
    SET LoanExistingCustomer = 0,
        LoanStaff = 1
    WHERE isLoan = 1 
        AND [description] LIKE '%staff%'
        AND LoanStaff = 0
END      