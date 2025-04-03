-- Trigger for Audit of Stockinfo table

IF EXISTS (SELECT * FROM sysobjects WHERE NAME= 'Trig_StockInfo_InsertUpdate')
DROP TRIGGER Trig_StockInfo_InsertUpdate
GO 

CREATE Trigger [dbo].[Trig_StockInfo_InsertUpdate] ON [dbo].[StockInfo]
For UPDATE, INSERT

AS

INSERT INTO StockInfoAudit(ItemNo, Itemdescr1,Itemdescr2, Category, Supplier, ProdStatus, SupplierCode,
			Warrantable,Itemtype,WarrantyRenewalFlag, Leadtime, AssemblyRequired, Taxrate ,DateChange,
			ID, SKU, IUPC, Class, SubClass, ColourName, ColourCode, QtyBoxes, WarrantyLength, 
			VendorWarranty, Brand, VendorStyle, VendorLongStyle, VendorEAN, RepossessedItem,ItemPOSDescr,SparePart)
	Select ItemNo, Itemdescr1,Itemdescr2, Category, Supplier, ProdStatus, SupplierCode,
			Warrantable,Itemtype,WarrantyRenewalFlag, Leadtime, AssemblyRequired, Taxrate , GetDate(),
			ID, SKU, IUPC, Class, SubClass, ColourName, ColourCode, QtyBoxes, WarrantyLength, 
			VendorWarranty, Brand, VendorStyle, VendorLongStyle, VendorEAN, RepossessedItem,ItemPOSDescr,SparePart
	From INSERTED I

-- End End End End End End End End End End End End End End End End End End End End End End End End