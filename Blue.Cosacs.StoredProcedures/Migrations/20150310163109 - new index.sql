-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE NONCLUSTERED INDEX ID_TermsTypeAudit_Account_DateFrom
ON [dbo].[termstypeaudit] ([acctno],[datefrom])