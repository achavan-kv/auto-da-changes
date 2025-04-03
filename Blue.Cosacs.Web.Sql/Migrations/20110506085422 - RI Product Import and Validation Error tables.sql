-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RItemp_RawProductImport') AND type in (N'U'))
BEGIN
	CREATE TABLE RItemp_RawProductImport
	(
		ItemIUPC	 	VARCHAR(50) ,
		ItemDescr1	 	VARCHAR(50) ,
		ItemDescr2	 	VARCHAR(50) ,
		ItemPOSDescr 	VARCHAR(50) ,
		Category 		VARCHAR(50) ,
		SupplierName 	VARCHAR(50) ,
		ProdStatus	 	VARCHAR(50) ,
		SupplierCode 	VARCHAR(50) ,
		Warrantable 	VARCHAR(50) ,
		ItemType	 	VARCHAR(50) ,
		AssemblyReq	 	VARCHAR(50) ,
		RefCode	 		VARCHAR(50) ,
		Deleted	 		VARCHAR(50) ,
		SKU		 		VARCHAR(50) ,
		ItemNo	 		VARCHAR(50) ,
		VendorUPC 		VARCHAR(50) ,
		BranchNo 		VARCHAR(50) ,
		CreditPrice		VARCHAR(50) ,
		CashPrice 		VARCHAR(50) ,
		DutyFreePrice	VARCHAR(50) ,
		CostPrice		VARCHAR(50) ,
		VendorWarranty	VARCHAR(50) ,			
		FYWperiod		VARCHAR(50) ,
		brandname		VARCHAR(50) ,		
		ColourName		VARCHAR(50) ,
		ColourCode		VARCHAR(50) ,
		VendorStyle		VARCHAR(50) ,		
		VendorStylelong	VARCHAR(50) ,		
		Boxes			VARCHAR(50) ,
		Class			VARCHAR(50) ,
		SubClass		VARCHAR(50) ,		
		LeadTime			VARCHAR(50) ,
		WarrantyRenewable	VARCHAR(50) 
	)
	
	Select * into RItemp_RawProductImportRepo from RItemp_RawProductImport
		
END

go
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RItemp_RawProductImportError') AND type in (N'U'))
BEGIN
select  CAST(' ' as VARCHAR(30)) as ColumnName,CAST(' ' as VARCHAR(30)) as ErrorDescr,
		CAST(' ' as VARCHAR(50)) as ErrorData,CAST(0 as tinyint) as Repo,* 
into RItemp_RawProductImportError  
from RItemp_RawProductImport

truncate TABLE RItemp_RawProductImportError

End
 