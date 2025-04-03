-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- Delivery Transfer
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RIDeliveryTransfersRepo') AND type in (N'U'))
BEGIN
    CREATE TABLE RIDeliveryTransfersRepo(
    Record				VARCHAR(100)
    )
End

-- Delivery & Returns 
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RIDeliveriesReturnsRepo') AND type in (N'U'))
BEGIN
    CREATE TABLE RIDeliveriesReturnsRepo(
    Record				VARCHAR(500)
    )
End

-- Raw Product data
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RItemp_RawProductloadRepo') AND type in (N'U'))
BEGIN
	CREATE TABLE RItemp_RawProductloadRepo
	(
		ItemIUPC	 	VARCHAR(18) default '' not null,
		ItemDescr1	 	VARCHAR(25) default '' not null,
		ItemDescr2	 	VARCHAR(30) default '' not null,
		ItemPOSDescr 	VARCHAR(18) default '' not null,
		Category 		VARCHAR(3) default '' not null,
		SupplierName 	VARCHAR(40) default '' not null,
		ProdStatus	 	VARCHAR(1) default '' not null,
		SupplierCode 	VARCHAR(18) default '' not null,
		Warrantable 	VARCHAR(1) default '' not null,
		ItemType	 	VARCHAR(1) default '' not null,
		AssemblyReq	 	VARCHAR(1) default '' not null,
		RefCode	 		VARCHAR(3) default '' not null,
		Deleted	 		VARCHAR(1) default '' not null,
		SKU		 		VARCHAR(8) default '' not null,
		ItemNo	 		VARCHAR(8) default '' not null,
		VendorUPC 		VARCHAR(18) default '' not null,
		BranchNo 		VARCHAR(3) default '' not null,
		CreditPrice		VARCHAR(10) default '' not null,
		CashPrice 		VARCHAR(10) default '' not null,
		DutyFreePrice	VARCHAR(10) default '' not null,
		CostPrice		VARCHAR(10) default '' not null,
		FYWperiodFlag	VARCHAR(2) default '' not null,			-- is this correct
		FYWperiod		VARCHAR(2) default '' not null,
		brandname		VARCHAR(25) default '' not null,		-- is this correct
		ColourName		VARCHAR(12) default '' not null,
		ColourCode		VARCHAR(3) default '' not null,
		VendorStyle		VARCHAR(12) default '' not null,		-- is this correct
		VendorStylelong	VARCHAR(12) default '' not null,		-- is this correct
		Boxes			VARCHAR(2) default '' not null,
		Class			VARCHAR(3) default '' not null,
		SubClass		VARCHAR(5) default '' not null		
		
	)
		
END

-- Raw Stock Quantity data
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RItemp_RawStkQtyloadRepo') AND type in (N'U'))
BEGIN	
	CREATE TABLE RItemp_RawStkQtyloadRepo
	(
	    ItemIUPC	 	VARCHAR(18) default '' not null,
	    StockLocn 		VARCHAR(3) default '' not null,
	    ActualStock		VARCHAR(10) default '' not null,
	    AvailableStock	VARCHAR(10) default '' not null,
	    CostPrice		VARCHAR(10) default '' not null
	)
			
END
