IF OBJECT_ID('FK_Merchandising_VendorReturn_GoodsReceipt', 'C') IS NOT NULL
alter table merchandising.vendorreturn
drop constraint FK_Merchandising_VendorReturn_GoodsReceipt

alter table merchandising.vendorreturn
add ReceiptType varchar(100) null
