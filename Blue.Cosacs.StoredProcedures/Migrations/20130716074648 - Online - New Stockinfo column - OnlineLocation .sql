-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'OnlineDConly'
               AND OBJECT_NAME(id) = 'Stockinfo')
BEGIN

	ALTER TABLE Stockinfo ADD OnlineDConly bit

END