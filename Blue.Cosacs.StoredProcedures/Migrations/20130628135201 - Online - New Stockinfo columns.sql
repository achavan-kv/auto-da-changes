-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'OnlineAvailable'
               AND OBJECT_NAME(id) = 'Stockinfo')
BEGIN

	ALTER TABLE Stockinfo ADD OnlineAvailable bit
	ALTER TABLE Stockinfo ADD OnlineDateAdded datetime 
	ALTER TABLE Stockinfo ADD OnlineDateRemoved datetime 
	

END
