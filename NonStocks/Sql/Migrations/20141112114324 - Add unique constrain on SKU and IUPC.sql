-- transaction: true

ALTER TABLE NonStocks.NonStock ADD CONSTRAINT UQ_NonStocks_NonStock_SKU UNIQUE (SKU)
GO

ALTER TABLE NonStocks.NonStock ADD CONSTRAINT UQ_NonStocks_NonStock_IUPC UNIQUE (IUPC)
GO
