DECLARE @taskid INT 
SELECT @taskid = MAX(TaskID) + 1 FROM [task]

IF NOT EXISTS (SELECT * FROM [Control] WHERE [Control]='menuInstManagement' and screen = 'MainForm')
BEGIN
    insert into control 
    (TaskID, Screen, Control, Visible, Enabled, ParentMenu)  
    values
    (@taskid,'MainForm','menuInstManagement',1,1,'menuService')

    insert into task
    (taskid,taskname)
    values
    (@taskid,'Installation Management')	
END

GO 