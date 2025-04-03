-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Populate Audit tables with current base table data


INSERT INTO StockInfoAudit(ItemNo, Itemdescr1,Itemdescr2, Category, Supplier, ProdStatus, SupplierCode,
			Warrantable,Itemtype,WarrantyRenewalFlag, Leadtime, AssemblyRequired, Taxrate ,DateChange)
	Select ItemNo, Itemdescr1,Itemdescr2, Category, Supplier, ProdStatus, SupplierCode,
			Warrantable,Itemtype,WarrantyRenewalFlag, Leadtime, AssemblyRequired, Taxrate , '1900-01-01'
	From StockInfo

go	

INSERT INTO StockPriceAudit(ItemNo, BranchNo,CreditPrice, CashPrice, DutyFreePrice, CostPrice, Refcode,DateChange)
	Select ItemNo, BranchNo,CreditPrice, CashPrice, DutyFreePrice, CostPrice, Refcode, '1900-01-01'
	From StockPrice
	
go
