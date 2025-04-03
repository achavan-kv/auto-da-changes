alter table [Financial].[VendorReturnMessage]
add SalesLocationId varchar(100) null

alter table [Financial].[GoodsReceiptMessage]
add SalesLocationId varchar(100) null

alter table [Financial].[StockAdjustmentMessage]
add SalesLocationId varchar(100) null