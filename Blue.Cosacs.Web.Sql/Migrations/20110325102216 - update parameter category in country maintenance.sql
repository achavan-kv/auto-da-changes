-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DECLARE @pcat INT 
SELECT @pcat = ParameterCategory
 FROM CountryMaintenance WHERE  CodeName = 'CL_EmployChgManApprove'

UPDATE CountryMaintenance SET ParameterCategory = @pcat 
WHERE CodeName = 'CL_CashLoanLetter'