-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
DECLARE @v sql_variant 
SET @v = N'1 -> Call was added through the user interface | 2 -> Call was added through the CustomerInstalmentEnding job | 3 -> Call was added through the InactiveCustomers job | 3 -> Call was added through the FollowUp job'
EXECUTE sp_updateextendedproperty N'MS_Description', @v, N'SCHEMA', N'SalesManagement', N'TABLE', N'Call', N'COLUMN', N'Source'