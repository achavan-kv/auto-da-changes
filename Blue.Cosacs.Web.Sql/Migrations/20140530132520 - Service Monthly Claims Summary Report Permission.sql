-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS(SELECT * FROM Admin.Permission WHERE Id = 2019 AND CategoryId = 20) BEGIN
	INSERT INTO Admin.Permission (Id, Name, CategoryId, [Description])
	VALUES (2019, N'Report - Service Monthly Claims Summary', 20, 
		N'View Service Monthly Claims Summary Report')
END
GO