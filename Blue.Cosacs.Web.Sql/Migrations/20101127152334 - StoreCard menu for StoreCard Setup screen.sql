-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- put your SQL code here
DECLARE @taskid INT 
SELECT @taskid =MAX(TaskID) +1 FROM [task]
IF NOT EXISTS (SELECT * FROM [Control] WHERE [Control]='menuStoreCardRateSetup' and screen = 'MainForm')
BEGIN
    insert into control 
    (TaskID      ,Screen, Control, Visible   ,  Enabled    , ParentMenu)  
    values
    (@taskid,'MainForm','menuStoreCardRateSetup',1,1,'menuSysMaint')

    insert into task
    (taskid,taskname)
    values
    (@taskid,'Setup Storecard interest Rates')	
END

GO 
