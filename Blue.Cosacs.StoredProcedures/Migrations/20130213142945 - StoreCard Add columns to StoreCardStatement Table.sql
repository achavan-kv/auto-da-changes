-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12208

IF NOT EXISTS(select * from syscolumns where name = 'RunNo' and object_name(id) = 'StoreCardStatement')
BEGIN
	ALTER TABLE StoreCardStatement ADD RunNo INT 
END	
GO

IF NOT EXISTS(select * from syscolumns where name = 'BatchNo' and object_name(id) = 'StoreCardStatement')
BEGIN
	ALTER TABLE StoreCardStatement ADD BatchNo INT 
END	
GO

IF NOT EXISTS(select * from syscolumns where name = 'ManualDatePrinted' and object_name(id) = 'StoreCardStatement')
BEGIN
	ALTER TABLE StoreCardStatement ADD ManualDatePrinted DATETIME 
END	
GO