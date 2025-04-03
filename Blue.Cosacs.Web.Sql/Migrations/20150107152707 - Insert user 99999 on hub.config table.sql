-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS(SELECT 1 FROM Hub.Config)
	INSERT INTO Hub.Config
		(UserName)
	Values
		(99999)
	