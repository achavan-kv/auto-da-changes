-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from control where TaskID = 399)
BEGIN

    INSERT INTO Control(TaskID, Screen, Control, Visible, Enabled, ParentMenu)
    SELECT 399, 'WorkLists', 'AllowAddUserToSUPWorkList', 1, 1, ''
END

