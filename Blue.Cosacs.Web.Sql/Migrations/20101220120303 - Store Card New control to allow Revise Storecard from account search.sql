 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.[Control] WHERE screen='AccountSearch' 
AND [control] = 'allowStoreCard')
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
) VALUES (@taskid, 'AccountSearch','allowStoreCard',0,1,'')

	
END

