-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'NonStockType'
               AND TABLE_SCHEMA = 'NonStocks')
BEGIN
    CREATE TABLE NonStocks.NonStockType
    (
        Code varchar(5) NOT NULL PRIMARY KEY
    )
END
GO

INSERT INTO NonStocks.NonStockType
SELECT 'inst'
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'NonStock'
               AND TABLE_SCHEMA = 'NonStocks')
BEGIN
    CREATE TABLE NonStocks.NonStock
    (
        Id int IDENTITY(1,1) PRIMARY KEY,
        [Type] varchar(5) NOT NULL,
        SKU varchar(18) NOT NULL,
        IUPC varchar(18) NOT NULL,
        ShortDescription varchar(32) NOT NULL,
        LongDescription varchar(40) NOT NULL,
        Active bit NOT NULL DEFAULT 1,
        TaxRate float NULL,
        CONSTRAINT [FK_NonStocks_NonStockType] FOREIGN KEY ([Type]) REFERENCES [NonStocks].[NonStockType] ([Code])
    )
END
GO
