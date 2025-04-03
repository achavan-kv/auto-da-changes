-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- put your SQL code here
DECLARE @taskid INT 
SELECT @taskid =MAX(TaskID) +1 FROM [task]
IF NOT EXISTS (SELECT * FROM [Control] WHERE [Control]='FinancialChanges' and screen = 'StoreCardAccount')
BEGIN
    insert into control 
    (TaskID      ,Screen, Control, Visible   ,  Enabled    , ParentMenu)  
    values
    (@taskid,'StoreCardAccount','FinancialChanges',0,1,'')

    insert into task
    (taskid,taskname)
    values
    (@taskid,'StoreCard - Higher Level Rights')	
END

