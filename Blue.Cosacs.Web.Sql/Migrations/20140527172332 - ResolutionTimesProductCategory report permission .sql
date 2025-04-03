-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
INSERT INTO Admin.Permission
	(Id, Name, CategoryId, Description)
VALUES
    (2017, 'Report - Resolution Times Per Product Category', 20, 'Resolution Times Per Product Category')