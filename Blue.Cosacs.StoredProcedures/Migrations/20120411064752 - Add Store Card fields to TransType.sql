-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Add new debtors account fields for Store Card
IF NOT EXISTS(select * from syscolumns where name = 'SCInterfaceAccount' and object_name(id) = 'TransType')
BEGIN
	ALTER TABLE TransType ADD SCInterfaceAccount VARCHAR(10) NOT NULL DEFAULT ''
END 

GO

IF NOT EXISTS(select * from syscolumns where name = 'SCInterfaceBalancing' and object_name(id) = 'TransType')
BEGIN
	ALTER TABLE TransType ADD SCInterfaceBalancing VARCHAR(10) NOT NULL DEFAULT ''
END 

GO

--Default the fields to be the same as the credit debtors accounts.
UPDATE TransType
SET SCInterfaceAccount = InterfaceAccount,
	SCInterfaceBalancing = InterfaceBalancing
FROM TransType
