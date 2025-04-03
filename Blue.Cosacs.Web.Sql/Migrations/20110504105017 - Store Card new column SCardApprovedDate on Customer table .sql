-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Date will store the preapproval date for a Store Card. This is updated by the Pre-approval End Of Day routine
IF NOT EXISTS(select * from information_schema.columns where table_name = 'Customer' and Column_Name = 'SCardApprovedDate')
BEGIN
	alter table customer add SCardApprovedDate datetime null
END