-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

DECLARE @taskid INT 
SELECT @taskid = taskid FROM control WHERE CONTROL = 'AllowStorecard' AND screen = 'BasicCustomerDetails'
UPDATE dbo.[Control] SET TaskID = @taskid 
WHERE CONTROL='AllowStorecard' AND screen = 'AccountSearch'