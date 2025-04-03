
UPDATE Sales.ItemType SET Id = 1
WHERE Id = 0 AND Name = 'Product'
GO

IF NOT EXISTS( SELECT Id FROM Sales.ItemType WHERE Id = 1 AND Name = 'Product' ) BEGIN
	INSERT INTO Sales.ItemType(Id, Name) VALUES (1, 'Product')
END	
GO

IF NOT EXISTS( SELECT Id FROM Sales.ItemType WHERE Id = 2 AND Name = 'Warranty' ) BEGIN
	INSERT INTO Sales.ItemType(Id, Name) VALUES (2, 'Warranty')
END	
GO

IF NOT EXISTS( SELECT Id FROM Sales.ItemType WHERE Id = 3 AND Name = 'Installation' ) BEGIN
	INSERT INTO Sales.ItemType(Id, Name) VALUES (3, 'Installation')
END	
GO

IF NOT EXISTS( SELECT Id FROM Sales.ItemType WHERE Id = 4 AND Name = 'Non-Stock' ) BEGIN
	INSERT INTO Sales.ItemType(Id, Name) VALUES (4, 'Non-Stock')
END	
GO