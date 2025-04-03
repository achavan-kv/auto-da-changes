-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LineItemBookingSchedule]') AND type in (N'U'))
BEGIN
	CREATE TABLE LineItemBookingSchedule
	(
		ID INT IDENTITY(1,1),
		LineItemID INT NOT NULL DEFAULT 0,
		DelOrColl CHAR(1),
		RetItemID INT,
		RetVal MONEY,
		RetStockLocn INT,
		BookingId INT
	)	
	
	ALTER TABLE [dbo].[LineItemBookingSchedule] ADD  CONSTRAINT [pk_LineItemBookingSchedule] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)
	
	
END
