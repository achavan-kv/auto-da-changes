-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'NonStock' AND   Column_Name = 'TaxRate'
           AND TABLE_SCHEMA = 'NonStocks')
BEGIN
	ALTER TABLE NonStocks.NonStock ALTER COLUMN TaxRate [BluePercentage] NULL
END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'NonStockPrice' AND   Column_Name = 'CostPrice'
           AND TABLE_SCHEMA = 'NonStocks')
BEGIN
	ALTER TABLE NonStocks.NonStockPrice ALTER COLUMN CostPrice [BlueAmount] NULL
END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'NonStockPrice' AND   Column_Name = 'RetailPrice'
           AND TABLE_SCHEMA = 'NonStocks')
BEGIN
	ALTER TABLE NonStocks.NonStockPrice ALTER COLUMN RetailPrice [BlueAmount] NULL
END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'NonStockPrice' AND   Column_Name = 'TaxInclusivePrice'
           AND TABLE_SCHEMA = 'NonStocks')
BEGIN
	ALTER TABLE NonStocks.NonStockPrice ALTER COLUMN TaxInclusivePrice [BlueAmount] NULL
END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'NonStockPromotion' AND   Column_Name = 'RetailPrice'
           AND TABLE_SCHEMA = 'NonStocks')
BEGIN
	ALTER TABLE NonStocks.NonStockPromotion ALTER COLUMN RetailPrice [BlueAmount] NULL
END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'NonStockPromotion' AND   Column_Name = 'PercentageDiscount'
           AND TABLE_SCHEMA = 'NonStocks')
BEGIN
	ALTER TABLE NonStocks.NonStockPromotion ALTER COLUMN PercentageDiscount [BluePercentage] NULL
END
GO
