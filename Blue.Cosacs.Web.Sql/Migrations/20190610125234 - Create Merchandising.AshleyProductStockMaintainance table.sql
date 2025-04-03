IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'AshleyProductStockMaintainance'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN
CREATE TABLE [Merchandising].[AshleyProductStockMaintainance]
(
	ID INT PRIMARY KEY IDENTITY(1,1),
	ProductId INT,
	VendorId INT,
	StockAvailable INT,
	LocationId INT	
)

END
GO