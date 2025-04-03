-- put your SQL code here
DECLARE @taskid INT 
SELECT @taskid =MAX(TaskID) +1 FROM [task]
IF NOT EXISTS (SELECT * FROM [Control] WHERE [Control]='allowStoreCard' and screen = 'BasicCustomerDetails')
BEGIN
    insert into control 
    (TaskID      ,Screen, Control, Visible   ,  Enabled    , ParentMenu)  
    values
    (@taskid,'BasicCustomerDetails','allowStoreCard',0,1,'')

    insert into task
    (taskid,taskname)
    values
    (@taskid,'Customer - Store Card Generate and Revise')	
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns
WHERE column_name = 'CreateStore' AND table_name ='Branch')
	ALTER TABLE dbo.branch ADD CreateStore BIT DEFAULT 0
GO 	
	UPDATE branch SET createstore = 0