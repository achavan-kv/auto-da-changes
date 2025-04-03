-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'Express'
               AND OBJECT_NAME(id) = 'LineItem')
BEGIN
  ALTER TABLE LineItem ADD Express CHAR(1) default ' '
END

IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LineItemBookingFailures]') AND type in (N'U'))
BEGIN
	CREATE TABLE LineItemBookingFailures
	(
		ID INT IDENTITY ,
		BookingID INT,
		OriginalBookingID INT,
		Quantity FLOAT,
		CancelReason VARCHAR(50)	
	)
	
	ALTER TABLE [dbo].[LineItemBookingFailures] ADD  CONSTRAINT [pk_LineItemBookingFailures] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)
	
	
END

GO

IF NOT EXISTS (select * from task where TaskName = 'Delivery & Collection Failures')
BEGIN

	Declare @TaskID int
	SET @TaskID = (select max(TaskID) from task) + 1
	
	INSERT INTO Task (TaskID, TaskName)
	SELECT @TaskID, 'Delivery & Collection Failures'
	
	INSERT INTO [Control] (TaskID, Screen, Control, Visible, Enabled, ParentMenu)
	SELECT @TaskID, 'MainForm', 'menuFailedDeliveriesCollections', 1, 1, 'menuWarehouse'
	
END