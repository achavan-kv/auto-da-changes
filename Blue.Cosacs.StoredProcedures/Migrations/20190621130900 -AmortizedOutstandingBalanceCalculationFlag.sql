-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/*
	CR : CLA Outstanding Balance 
	Author : Rahul D
	Date : 21/06/2019
	Details : Flag to enable or disable functionality in CLA Outstanding Balance CR
*/
BEGIN TRAN
INSERT INTO [dbo].[CountryMaintenance]
SELECT TOP 1 countryCode,30,'Enable Amortized Outstanding Balance Calculation','False','checkbox',
'0','','','- If this is set to true then ''Outstanding Balance'' amount for cash loan is required to calculate as per new amortization rule.

Note: This flag is applicable only if ''Enable Amortized Cash Loan'' flag is TRUE. This functionality will work for only for new Cash loan accounts..',
'CL_NewOutstandingCalculation'
 FROM [dbo].[CountryMaintenance]
 WHERE countryCode = (SELECT countryCode FROM country)
COMMIT

