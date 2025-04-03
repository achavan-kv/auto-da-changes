-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO dbo.CountryMaintenance
SELECT c.countrycode, 
       14, 
       'Cash Loan Rebate Calculation Rule', 
       cm.Value, 
       'numeric', 
       0, 
       '', 
       '', 
       'This is the rebate calculation rule that will be used when a cash loan customer receives their rebate. This does not affect the Rebate calculation frame totals or the Rebate Forecast Report totals. The rules are as follows: 78-0, 78-1, 78-2, 78-3, etc. Eg: To set rule 78-2 set the value to -2. To set the rule to 78-0 set the value to 0. To set the rule to 78+1 set the value to 1.',
       'LoanRebateRule'
FROM country c,
     CountryMaintenance cm
WHERE cm.CodeName = 'RebateCalculationRule'

IF(SELECT countrycode FROM country) = 'L'
BEGIN
    UPDATE CountryMaintenance
    SET Value = 'Term'
    WHERE CodeName = 'LoanRebateRule'
END