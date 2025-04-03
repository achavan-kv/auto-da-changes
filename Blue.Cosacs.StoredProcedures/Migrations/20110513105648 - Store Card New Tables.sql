-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Store Card Feature #3004

IF NOT EXISTS(select * from information_schema.tables where table_name = 'StoreCardStatus')
BEGIN
	CREATE TABLE StoreCardStatus 
	(
		Status varchar(5),
		Description varchar(22)
	
	CONSTRAINT [pk_StoreCardStatus] PRIMARY KEY CLUSTERED
	(
		[Status] ASC
	))
END
GO
--EXEC sp_help storecardstatus 
--Insert Store Card Status's into the table.

IF NOT EXISTS(select * from StoreCardStatus where [status] = 'I')
BEGIN
	insert into StoreCardStatus
	select 'I', 'Card Issued'
END
GO

IF NOT EXISTS(select * from StoreCardStatus where [status] = 'LS')
BEGIN
	insert into StoreCardStatus
	select 'LS', 'Lost/Stolen'
END
GO

IF NOT EXISTS(select * from StoreCardStatus where [status] = 'NA')
BEGIN
	insert into StoreCardStatus
	select 'NA', 'No active cards issued'
END
GO
IF NOT EXISTS(select * from StoreCardStatus where [status] = 'TBI')
BEGIN
	insert into StoreCardStatus
	select 'TBI', 'Card To Be Issued'
END
GO

IF NOT EXISTS(select * from StoreCardStatus where [status] = 'TBR')
BEGIN
	insert into StoreCardStatus	
	select 'TBR', 'To Be Re-issued'
END
GO

--Add new column 'Status' to table StoreCardPaymentDetails

IF NOT EXISTS(select * from information_schema.columns where table_name = 'StoreCardPaymentDetails' and column_name = 'Status')
BEGIN
	alter table StoreCardPaymentDetails add [Status] varchar(5) 	
END
GO


IF NOT EXISTS(select * from sys.foreign_keys where name = 'FK_StoreCardStatus')
BEGIN
	ALTER TABLE [StoreCardPaymentDetails] ADD CONSTRAINT
	[FK_StoreCardStatus] FOREIGN KEY
	(
		[Status]
	)REFERENCES StoreCardStatus 
	(
		[Status]
	)
END
GO
