if exists(select * from sys.columns where Name = N'CorporatePoNumber' and Object_ID = Object_ID(N'Merchandising.PurchaseOrder'))
begin
   
alter table Merchandising.PurchaseOrder 
drop constraint UX_Merchandising_PurchaseOrder_PoNumber
   
alter table Merchandising.PurchaseOrder
drop column
	 [OriginSystem]			
	,[CorporatePoNumber]	
	,[ShipDate]				
	,[ShipVia]				
	,[PortOfLoading]		
	,[Attributes]			
	,[CommissionChargeFlag]
	,[CommissionPercentage]

alter table Merchandising.PurchaseOrderProduct
drop constraint
	 DF_Merchandising_PurchaseOrder_PreLandedUnitCost
	,DF_Merchandising_PurchaseOrder_PreLandedExtendedUnitCost

alter table Merchandising.PurchaseOrderProduct
drop column
	 [PreLandedUnitCost]	
	,[PreLandedExtendedCost]
	
end

