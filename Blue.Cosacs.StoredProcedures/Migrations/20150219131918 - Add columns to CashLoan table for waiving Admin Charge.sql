-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'CashLoan' AND Column_Name = 'AdminChargeWaived')
BEGIN
	ALTER TABLE CashLoan Add AdminChargeWaived bit not null default 0
END
GO


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'CashLoan' AND Column_Name = 'AdminCharge')
BEGIN
	ALTER TABLE CashLoan Add AdminCharge money null
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'CashLoan' AND Column_Name = 'EmpeenoAdminChargeWaived')
BEGIN
	ALTER TABLE CashLoan Add EmpeenoAdminChargeWaived int null
END
GO
