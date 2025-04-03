-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


drop TABLE RItemp_RawProductLoad

go
-- Raw Product data
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('RItemp_RawProductload') AND type in (N'U'))
BEGIN
	CREATE TABLE RItemp_RawProductload
	(
		ItemIUPC	 	VARCHAR(18) ,
		ItemDescr1	 	VARCHAR(25) ,
		ItemDescr2	 	VARCHAR(30) ,
		ItemPOSDescr 	VARCHAR(18) ,
		Category 		VARCHAR(3) ,
		SupplierName 	VARCHAR(25) ,
		ProdStatus	 	VARCHAR(1) ,
		SupplierCode 	VARCHAR(6) ,
		Warrantable 	VARCHAR(1) ,
		ItemType	 	VARCHAR(1) ,
		AssemblyReq	 	VARCHAR(1) ,
		RefCode	 		VARCHAR(3) ,
		Deleted	 		VARCHAR(1) ,
		SKU		 		VARCHAR(11) ,
		ItemNo	 		VARCHAR(11) ,
		VendorUPC 		VARCHAR(18) ,
		BranchNo 		VARCHAR(3) ,
		CreditPrice		VARCHAR(11) ,
		CashPrice 		VARCHAR(11) ,
		DutyFreePrice	VARCHAR(11) ,
		CostPrice		VARCHAR(11) ,
		VendorWarranty	VARCHAR(1) ,		
		FYWperiod		VARCHAR(2) ,
		BrandName		VARCHAR(25) ,		
		ColourName		VARCHAR(3) ,
		ColourCode		VARCHAR(3) ,
		VendorStyle		VARCHAR(12) ,		
		VendorStyleLong	VARCHAR(25) ,		
		Boxes			VARCHAR(3) ,
		Class			VARCHAR(3) ,
		SubClass		VARCHAR(5) ,
		LeadTime        VARCHAR(2)
	)
End
go

drop TABLE RItemp_RawProductLoadRepo

go

select * into RItemp_RawProductLoadRepo from RItemp_RawProductLoad

