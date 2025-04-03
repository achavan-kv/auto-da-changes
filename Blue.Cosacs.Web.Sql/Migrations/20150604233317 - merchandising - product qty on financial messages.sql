alter table [Financial].[GoodsReceiptProductMessage]
add Units int null

alter table [Financial].[StockAdjustmentProductMessage]
add Units int null

alter table [Financial].[TransferProductMessage]
add Units int null

alter table [Financial].[VendorReturnProductMessage]
add Units int null

go 

update  [Financial].[GoodsReceiptProductMessage]
set Units = 0

update [Financial].[StockAdjustmentProductMessage]
set Units = 0

update [Financial].[TransferProductMessage]
set Units = 0

update [Financial].[VendorReturnProductMessage]
set Units = 0

alter table [Financial].[GoodsReceiptProductMessage]
alter column Units int not null

alter table [Financial].[StockAdjustmentProductMessage]
alter column Units int not null

alter table [Financial].[TransferProductMessage]
alter column Units int not null

alter table [Financial].[VendorReturnProductMessage]
alter column Units int not null