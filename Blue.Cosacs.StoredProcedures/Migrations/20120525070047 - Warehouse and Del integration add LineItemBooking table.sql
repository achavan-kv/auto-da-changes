-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(SELECT * FROM syscolumns
			WHERE name = 'ID'
			AND object_name(id) = 'LineItem')
BEGIN
	ALTER TABLE LineItem ADD ID INT IDENTITY
	
	CREATE NONCLUSTERED INDEX [ix_ID] ON [dbo].[lineitem] 
	(
		[ID] 
	)

END


IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LineItemBooking]') AND type in (N'U'))
BEGIN
	CREATE TABLE LineItemBooking
	(
		ID INT IDENTITY(1,1) PRIMARY KEY,
		LineItemID INT NOT NULL DEFAULT 0	
	)
	
END
