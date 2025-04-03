-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RunNo'
               AND OBJECT_NAME(id) = 'LineitemAudit')
BEGIN
  ALTER TABLE LineitemAudit ADD RunNo Int
END

-- Committed Stock
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RICommittedStock') AND type in (N'U'))
BEGIN
    CREATE TABLE RICommittedStock(
    Record				VARCHAR(100)
    )
End

-- Delivery Transfer
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RIDeliveryTransfers') AND type in (N'U'))
BEGIN
    CREATE TABLE RIDeliveryTransfers(
    Record				VARCHAR(100)
    )
End

-- Delivery Transfer
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RIRepossessions') AND type in (N'U'))
BEGIN
    CREATE TABLE RIRepossessions(
    Record				VARCHAR(100)
    )
End

-- Delivery & Returns 
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RIDeliveriesReturns') AND type in (N'U'))
BEGIN
    CREATE TABLE RIDeliveriesReturns(
    Record				VARCHAR(200)
    )
End

-- ISO Country Code
IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ISOCountryCode'
               AND OBJECT_NAME(id) = 'Country')
BEGIN
  ALTER TABLE Country ADD ISOCountryCode CHAR(2) not null default ''
END
go

UPDATE Country set ISOCountryCode = case 
				when CountryCode='A' then 'GY'	--Guyana
				when CountryCode='B' then 'BB'	--Barbados
				when CountryCode='D' then 'DM'	--Dominica
				when CountryCode='G' then 'GD'	--Grenada
				when CountryCode='J' then 'JM'	--Jamaica
				when CountryCode='K' then 'KN'	--St Kitts
				when CountryCode='L' then 'LC'	--St Lucia
				when CountryCode='N' then 'AG'	--Antigua
				when CountryCode='T' then 'TT'	--Trinidad
				when CountryCode='V' then 'VC'	--St Vincent
				when CountryCode='Z' then 'BZ'	--Belize
				end

-- Raw Kit data
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RItemp_RawKitload') AND type in (N'U'))
BEGIN
	CREATE TABLE RItemp_RawKitload
	(
		KitItemIUPC	 	VARCHAR(18) default '' not null,
		ComponentIUPC 	VARCHAR(18) default '' 	not null,
		ComponentQty 	VARCHAR(5)	default '' 	not null,
		ComponentPrice	VARCHAR(11) default '' 	not null,
		DeletedFlag		VARCHAR(1)	default '' 	not null
	)
		
END

-- Temp Kit data
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RItemp_Kitload') AND type in (N'U'))
BEGIN
	CREATE TABLE RItemp_Kitload
	(
		KitItemIUPC	 	VARCHAR(18) ,
		ComponentIUPC 	VARCHAR(18) ,
		ComponentQty 	DECIMAL(5,2),
		ComponentPrice	money ,
		DeletedFlag		VARCHAR(1),
		ItExists		BIT,
		CpExists		BIT,
		RowProcessed	Bit
	)
		
END
-- Kit Product Price
IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ComponentPrice'
               AND OBJECT_NAME(id) = 'Kitproduct')
BEGIN
  ALTER TABLE kitproduct ADD ComponentPrice MONEY default 0 not null
END


-- Raw Purchase Order data
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RItemp_RawPOload') AND type in (N'U'))
BEGIN
	CREATE TABLE RItemp_RawPOload
	(
		ItemIUPC	 	VARCHAR(18) default '' not null,
		POReceiptDate 	VARCHAR(10) default '' 	not null,
		POOrderQty 		VARCHAR(8)	default '' 	not null,
		VendorNumber	VARCHAR(18) default '' 	not null,
		StockLocn		VARCHAR(3)	default '' 	not null
	)
		
END

-- Raw Product data
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RItemp_RawProductload') AND type in (N'U'))
BEGIN
	CREATE TABLE RItemp_RawProductload
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
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RItemp_RawStkQtyload') AND type in (N'U'))
BEGIN	
	CREATE TABLE RItemp_RawStkQtyload
	(
	    ItemIUPC	 	VARCHAR(18) default '' not null,
	    StockLocn 		VARCHAR(3) default '' not null,
	    ActualStock		VARCHAR(10) default '' not null,
	    AvailableStock	VARCHAR(10) default '' not null,
	    CostPrice		VARCHAR(10) default '' not null
	)
			
END