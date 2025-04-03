DECLARE @taskid INT 
SELECT @taskid = MAX(TaskID) + 1 FROM [task]

IF NOT EXISTS (SELECT * FROM [Control] WHERE [Control]='menuInstBookingPrint' and screen = 'MainForm')
BEGIN
    insert into control 
    (TaskID, Screen, Control, Visible, Enabled, ParentMenu)  
    values
    (@taskid,'MainForm','menuInstBookingPrint',1,1,'menuService')

    insert into task
    (taskid,taskname)
    values
    (@taskid,'Installation Booking Print')	
END

GO 