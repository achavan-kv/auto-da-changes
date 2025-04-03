-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here



IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_TYPE='FOREIGN KEY' AND TABLE_SCHEMA='NonStocks'
        AND TABLE_NAME = 'NonStock' AND CONSTRAINT_NAME = 'FK_NonStocks_NonStockType')
BEGIN
    ALTER TABLE [NonStocks].[NonStock] DROP CONSTRAINT [FK_NonStocks_NonStockType]
END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA='NonStocks'
    AND TABLE_NAME = 'NonStock' AND COLUMN_NAME = 'Type')
BEGIN
    ALTER TABLE NonStocks.NonStock ALTER COLUMN [Type] Varchar(7) not null
END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA='NonStocks'
    AND TABLE_NAME = 'NonStockType' AND COLUMN_NAME = 'Code')
BEGIN
    ALTER TABLE NonStocks.NonStockType ALTER COLUMN [Code] Varchar(7) not null
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_TYPE='FOREIGN KEY' AND TABLE_SCHEMA='NonStocks'
        AND TABLE_NAME = 'NonStock' AND CONSTRAINT_NAME = 'FK_NonStocks_NonStockType')
BEGIN
    ALTER TABLE [NonStocks].[NonStock]  WITH CHECK ADD  CONSTRAINT [FK_NonStocks_NonStockType] FOREIGN KEY([Type])
    REFERENCES [NonStocks].[NonStockType] ([Code])  
END
GO
