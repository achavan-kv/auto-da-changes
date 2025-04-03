-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
SET NOCOUNT ON;

UPDATE FinancialWeeks SET [Quarter] = 1 WHERE [YEAR] = 2014 AND [Week] = 13
UPDATE FinancialWeeks SET [Quarter] = 2 WHERE [YEAR] = 2014 AND [Week] = 26
UPDATE FinancialWeeks SET [Quarter] = 3 WHERE [YEAR] = 2014 AND [Week] = 39
GO