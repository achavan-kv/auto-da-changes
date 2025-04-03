-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.[Control] WHERE screen='BasicCustomerDetails' AND [control] = 'allowStoreCard')
BEGIN
	DECLARE @taskid INT 
	SELECT @taskid = MAX(taskid) +1 FROM CONTROL

INSERT INTO dbo.[Control] (
	TaskID,
	Screen,
	[Control],
	Visible,
	Enabled,
	ParentMenu
) VALUES (@taskid, 'BasicCustomerDetails','allowStoreCard',0,1,'')

	
END