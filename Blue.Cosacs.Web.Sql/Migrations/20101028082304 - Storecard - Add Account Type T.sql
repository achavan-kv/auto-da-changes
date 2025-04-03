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
    (@taskid,'Customer - Generate Store Card Account')	
END


IF NOT EXISTS (SELECT * FROM dbo.acctnoctrl WHERE acctcat = 'T')
INSERT INTO dbo.acctnoctrl (
	origbr,	branchno,	acctcat,
	acctcatdesc,	hiallocated,	hiallowed
) 
SELECT 	origbr,	branchno,
	'T',	'Store Card',
	1,	hiallowed 
	FROM acctnoctrl
WHERE acctcat ='R'


