-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (select * from task where TaskName = 'Search Cash and Go - Reprint Receipt')
BEGIN

	Declare @TaskID int
	SET @TaskID = (select max(TaskID) from task) + 1
	
	INSERT INTO Task (TaskID, TaskName)
	SELECT @TaskID, 'Search Cash and Go - Reprint Receipt'
	
	INSERT INTO [Control] (TaskID, Screen, Control, Visible, Enabled, ParentMenu)
	SELECT @TaskID, 'SearchCashAndGo', 'ReprintReceipt', 1, 1, ''
	
END