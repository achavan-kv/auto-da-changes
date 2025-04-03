-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'InstantCreditApprovalChecks' AND  Column_Name = 'Staff')
BEGIN
	ALTER TABLE InstantCreditApprovalChecks Add Staff char(1) null
END