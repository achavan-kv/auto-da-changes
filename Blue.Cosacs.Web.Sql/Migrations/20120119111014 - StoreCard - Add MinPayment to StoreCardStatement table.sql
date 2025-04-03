-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from syscolumns where name = 'StmtMinPayment' and object_name(id) = 'StoreCardStatement')
BEGIN
	ALTER TABLE StoreCardStatement ADD StmtMinPayment MONEY not null default 0
END 
GO

IF NOT EXISTS(select * from syscolumns where name = 'PrevStmtMinPayment' and object_name(id) = 'StoreCardStatement')
BEGIN
	ALTER TABLE StoreCardStatement ADD PrevStmtMinPayment MONEY not null default 0
END 
GO