IF EXISTS (SELECT * 
  FROM sys.foreign_keys 
   WHERE object_id = OBJECT_ID(N'merchandising.FK_Merchandising_VendorReturn_GoodsReceipt')
   AND parent_object_id = OBJECT_ID(N'merchandising.vendorreturn')
)
alter table merchandising.vendorreturn
drop constraint [FK_Merchandising_VendorReturn_GoodsReceipt]

IF EXISTS (SELECT * 
  FROM sys.foreign_keys 
   WHERE object_id = OBJECT_ID(N'merchandising.FK_Merchandising_VendorReturnProduct_GoodsReceiptProduct')
   AND parent_object_id = OBJECT_ID(N'merchandising.vendorreturnproduct')
)
alter table merchandising.vendorreturnproduct
drop constraint [FK_Merchandising_VendorReturnProduct_GoodsReceiptProduct]