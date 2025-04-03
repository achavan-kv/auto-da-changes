DECLARE @taskid INT 
SELECT @taskid = MAX(TaskID) + 1 FROM [task]

IF NOT EXISTS (SELECT * FROM [Control] WHERE [Control]='menuInstallationReview' and screen = 'MainForm')
BEGIN
    insert into control 
    (TaskID, Screen, Control, Visible, Enabled, ParentMenu)  
    values
    (@taskid,'MainForm','menuInstallationReview',1,1,'menuAccount')

    insert into task
    (taskid,taskname)
    values
    (@taskid,'menuInstallation Review')	
END

GO 
