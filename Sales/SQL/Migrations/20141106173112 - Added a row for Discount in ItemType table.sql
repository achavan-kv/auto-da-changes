-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
IF NOT EXISTS( SELECT Id FROM Sales.ItemType WHERE Id = 5 AND Name = 'Discount' ) BEGIN
	INSERT INTO Sales.ItemType(Id, Name) VALUES (5, 'Discount')
END	
GO
