-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'ReadyAssistDetails' AND  Column_Name = 'ItemId')
BEGIN
	ALTER TABLE ReadyAssistDetails Add ItemId int not null default 0
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'ReadyAssistDetails' AND  Column_Name = 'ContractNo')
BEGIN
	ALTER TABLE ReadyAssistDetails Add ContractNo varchar(10) not null default ''
END
