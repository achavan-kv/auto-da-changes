-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RepossessedItem'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD RepossessedItem BIT not null default(0)
END


IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemPOSDescr'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD ItemPOSDescr VARCHAR(25)
END
go

select distinct itemno,MAX(itemtype) as ItemType	--,COUNT(*) 
into #missingStock
from lineitem l
where not exists(select * from stockinfo s where l.itemno=s.itemno)
group by l.itemno

IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trig_StockInfo_InsertUpdate]'))
Disable TRIGGER [dbo].[Trig_StockInfo_InsertUpdate] on dbo.StockInfo


UPDATE #missingStock set ItemType='S' where itemtype=''
insert into dbo.StockInfo (
	itemno,
	itemdescr1,
	itemdescr2,
	category,
	Supplier,
	prodstatus,
	suppliercode,
	warrantable,
	itemtype,
	warrantyrenewalflag,
	leadtime,
	assemblyrequired,
	taxrate,
	SKU,
	IUPC,
	Class,
	SubClass,
	ColourName,
	ColourCode,
	QtyBoxes,
	WarrantyLength,
	VendorWarranty,
	Brand,
	VendorStyle,
	VendorLongStyle,
	VendorEAN,
	RepossessedItem,
	ItemPOSDescr
) 
select   
	/* itemno - varchar(18) */ m.itemno,
	/* itemdescr1 - varchar(32) */ 'Deleted Item',
	/* itemdescr2 - varchar(40) */ m.itemno + ' Deleted Item',
	/* category - smallint */ 0,
	/* Supplier - varchar(40) */ '',
	/* prodstatus - varchar(1) */ 'D',
	/* suppliercode - varchar(18) */ '',
	/* warrantable - smallint */ 0,
	/* itemtype - varchar(1) */ m.ItemType,
	/* warrantyrenewalflag - char(1) */ '',
	/* leadtime - smallint */ 0,
	/* assemblyrequired - char(1) */ '',
	/* taxrate - float */ 0,
	/* SKU - varchar(8) */ '000000',
	/* IUPC - varchar(18) */ m.itemno,
	/* Class - char(3) */ '',
	/* SubClass - char(5) */ '',
	/* ColourName - varchar(12) */ '',
	/* ColourCode - varchar(3) */ '',
	/* QtyBoxes - smallint */ 0,
	/* WarrantyLength - smallint */ 0,
	/* VendorWarranty - smallint */ 0,
	/* Brand - varchar(25) */ '',
	/* VendorStyle - varchar(12) */ '',
	/* VendorLongStyle - varchar(25) */ '',
	/* VendorEAN - varchar(18) */ '',
	/* RepossessedItem - bit */ 0,
	/* ItemPOSDescr - varchar(25) */ '' 
	
	from #missingStock m
	
	IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trig_StockInfo_InsertUpdate]'))
	Enable TRIGGER [dbo].[Trig_StockInfo_InsertUpdate] on dbo.StockInfo
	
	IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trig_StockQuantity_InsertUpdate]'))
	Disable TRIGGER [dbo].[Trig_StockQuantity_InsertUpdate] on dbo.StockQuantity

	insert into StockQuantity (
		itemno,
		stocklocn,
		qtyAvailable,
		stock,
		stockonorder,
		stockdamage,
		leadtime,
		dateupdated,
		deleted,
		LastOperationSource,
		ID
	) 
	Select 
		/* itemno - varchar(18) */ m.itemno,
		/* stocklocn - smallint */ b.branchno,
		/* qtyAvailable - float */ 0,
		/* stock - float */ 0,
		/* stockonorder - float */ 0,
		/* stockdamage - float */ 0,
		/* leadtime - smallint */ 0,
		/* dateupdated - datetime */ '2011-7-12 11:29:36.538',
		/* deleted - varchar(1) */ 'Y',
		/* LastOperationSource - varchar(10) */ '',
		/* ID - int */ ISNULL(s.ID,0) 
		From #missingStock m LEFT outer JOIN StockInfo s on m.itemno=s.itemno,
				branch b
		
	IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trig_StockQuantity_InsertUpdate]'))
	Enable TRIGGER [dbo].[Trig_StockQuantity_InsertUpdate] on dbo.StockQuantity
	
	IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trig_StockPrice_InsertUpdate]'))
	Disable TRIGGER [dbo].[Trig_StockPrice_InsertUpdate] on dbo.StockPrice
			
	insert into dbo.StockPrice (
		itemno,
		branchno,
		CreditPrice,
		CashPrice,
		DutyFreePrice,
		CostPrice,
		Refcode,
		DateActivated,
		ID
	) 
	Select 
		/* itemno - varchar(18) */ m.ItemNo,
		/* branchno - smallint */ b.branchno,
		/* CreditPrice - money */ 1,
		/* CashPrice - money */ 1,
		/* DutyFreePrice - money */ 0,
		/* CostPrice - money */ 1,
		/* Refcode - varchar(3) */ '',
		/* DateActivated - datetime */ '2011-7-12 11:37:27.2',
		/* ID - int */ ISNULL(s.id,0)
		
	From #missingStock m LEFT outer JOIN StockInfo s on m.itemno=s.itemno,
				branch b 
				
	IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trig_StockPrice_InsertUpdate]'))
	Enable TRIGGER [dbo].[Trig_StockPrice_InsertUpdate] on dbo.StockPrice
	
go
