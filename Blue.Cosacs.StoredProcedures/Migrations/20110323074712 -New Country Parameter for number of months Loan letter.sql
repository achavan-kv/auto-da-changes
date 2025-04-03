-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.CountryMaintenance WHERE CodeName = 'CL_CashLoanLetter')
INSERT INTO CountryMaintenance
(CountryCode,ParameterCategory,
NAME,Value,[Type],PRECISION,
OptionCategory,OptionListName,Description,
CodeName)
SELECT CountryCode,'30',
'Number of Months Cash Loan letter applicable','2','numeric','0',
'','','Number months a Cash loan letter would be applicable - after sending. After this period if customers come in for the Loan they may no longer qualify',
'CL_CashLoanLetter' FROM dbo.Country

