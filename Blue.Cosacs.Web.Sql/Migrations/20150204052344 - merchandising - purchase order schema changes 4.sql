alter table Merchandising.PurchaseOrderProduct
add
	 [PreLandedUnitCost]	 money			not null	constraint DF_Merchandising_PurchaseOrder_PreLandedUnitCost default 0
	,[PreLandedExtendedCost] money			not null	constraint DF_Merchandising_PurchaseOrder_PreLandedExtendedUnitCost default 0
