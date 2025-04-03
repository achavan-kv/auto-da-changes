SET IDENTITY_INSERT Warranty.Link ON 
INSERT INTO Warranty.Link
	(id, Name, EffectiveDate)
VALUES
	(-100, 'IntegrationTest', '20440301'),
	(-101, 'IntegrationTest2', '20440401')
SET IDENTITY_INSERT Warranty.Link OFF

SET IDENTITY_INSERT Warranty.LinkWarranty ON 
INSERT INTO Warranty.LinkWarranty
	(Id, LinkId, WarrantyId, ProductMin, ProductMax)
VALUES
	(-100, -100, 1, 10.00, 149.00), 
	(-101, -100, 5, 150.00, 300.00), 
	(-102, -100, 7, 301.00, 1000.00), 
	(-103, -100, 121, 0.00, 99999999999.00), 
	(-104, -101, 37, 150.00, 380.00), 
	(-105, -101, 39, 381.00, 800.00), 
	(-106, -101, 7, 801.00, 3250.00)
SET IDENTITY_INSERT Warranty.LinkWarranty OFF

SET IDENTITY_INSERT Warranty.LinkProduct ON
INSERT INTO Warranty.LinkProduct
	(Id, LinkId, StockBranch, ItemNumber, Level_1, Level_2, Level_3, StoreType, RefCode) 
VALUES
	(-100, -100, 'NULL', 'NULL', 'NULL', 'NULL', 'NULL', 'NULL', 'NULL'), 
	(-101, -100, 'NULL', 'NULL', 'PCE', '1', 'NULL', 'NULL', 'NULL'), 
	(-102, -100, 'NULL', 'NULL', 'NULL', 'NULL', 'NULL', 'NULL', '00'), 
	(-103, -101, 'NULL', 'NULL', 'PCE', '1', '102', 'C', 'NULL'), 
	(-104, -101, '900', 'NULL', 'NULL', 'NULL', 'NULL', 'NULL', '40')
SET IDENTITY_INSERT Warranty.LinkProduct OFF



