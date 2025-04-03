-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS( SELECT Id FROM Sales.ItemType WHERE Id = 8 AND Name = 'Kit' ) BEGIN
	INSERT INTO Sales.ItemType(Id, Name) VALUES (8, 'Kit')
END	
GO