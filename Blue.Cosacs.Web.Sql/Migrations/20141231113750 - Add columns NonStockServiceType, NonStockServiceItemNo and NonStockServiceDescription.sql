-- transaction: true

ALTER TABLE [Warehouse].[Booking]
ADD [NonStockServiceType] VARCHAR(8) NULL -- Just like field NonStocks.NonStock.Type which is size varchar(8)

ALTER TABLE [Warehouse].[Booking]
ADD [NonStockServiceItemNo] VARCHAR(18) NULL -- Just like field dbo.StockInfo.itemno which is of size varchar(18)

ALTER TABLE [Warehouse].[Booking] -- '32 chars': for dbo.StockInfo.itemdescr1 and 40 chars: for dbo.StockInfo.itemdescr2 (or ShortDescription and LongDescription). 32+40=72 => decided for 75
ADD [NonStockServiceDescription] VARCHAR(75) NULL

