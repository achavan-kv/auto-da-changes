-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'NonStockPromotion'
               AND TABLE_SCHEMA = 'NonStocks')
BEGIN
    CREATE TABLE [NonStocks].[NonStockPromotion]
    (
	    [Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	    [StartDate] [date] NOT NULL,
	    [EndDate] [date] NOT NULL,
	    [RetailPrice] decimal (19,3) NULL,
        [PercentageDiscount] decimal (4,2) NULL,
        [Fascia] VARCHAR(20) NULL,
        [BranchNumber] [smallint] NULL,
        [NonStockId] [int] NOT NULL 
        CONSTRAINT [FK_NonStocks_NonStockPromotion_BranchNumber] FOREIGN KEY ([BranchNumber]) REFERENCES [dbo].[branch] ([branchno]),
        CONSTRAINT [FK_NonStocks_NonStockPromotion_NonStockId] FOREIGN KEY ([NonStockId]) REFERENCES [NonStocks].[NonStock] ([Id])
    )
END
GO