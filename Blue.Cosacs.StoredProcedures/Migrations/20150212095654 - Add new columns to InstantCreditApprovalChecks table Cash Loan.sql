-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'InstantCreditApprovalChecks' AND  Column_Name = 'HNewCustomer')
BEGIN
	ALTER TABLE InstantCreditApprovalChecks Add HNewCustomer bit not null default 0
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'InstantCreditApprovalChecks' AND  Column_Name = 'HRecentCustomer')
BEGIN
	ALTER TABLE InstantCreditApprovalChecks Add HRecentCustomer bit not null default 0
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'InstantCreditApprovalChecks' AND  Column_Name = 'HExistingCustomer')
BEGIN
	ALTER TABLE InstantCreditApprovalChecks Add HExistingCustomer bit not null default 0
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'InstantCreditApprovalChecks' AND  Column_Name = 'JNewCustomer')
BEGIN
	ALTER TABLE InstantCreditApprovalChecks Add JNewCustomer bit not null default 0
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'InstantCreditApprovalChecks' AND  Column_Name = 'JRecentCustomer')
BEGIN
	ALTER TABLE InstantCreditApprovalChecks Add JRecentCustomer bit not null default 0
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'InstantCreditApprovalChecks' AND  Column_Name = 'JExistingCustomer')
BEGIN
	ALTER TABLE InstantCreditApprovalChecks Add JExistingCustomer bit not null default 0
END
GO