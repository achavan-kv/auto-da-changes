-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'NonStockPrice'
               AND TABLE_SCHEMA = 'NonStocks')
BEGIN
    CREATE TABLE [NonStocks].[NonStockPrice]
    (
	    [Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	    [NonStockId] [int] NOT NULL,
	    [Fascia] [varchar](20) NULL,
	    [BranchNumber] [smallint] NULL,
	    [CostPrice] decimal (19,3) NULL,
	    [RetailPrice] decimal (19,3) NULL,
        [TaxInclusivePrice] decimal (19,3) NULL,
	    [EffectiveDate] [date] NOT NULL,
        CONSTRAINT [FK_NonStocks_NonStock] FOREIGN KEY ([NonStockId]) REFERENCES [NonStocks].[NonStock] ([Id])
    )
END
GO

