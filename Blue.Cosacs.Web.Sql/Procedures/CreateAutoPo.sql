
GO

IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreateAutoPO]') AND type in (N'P', N'PC'))
BEGIN
  DROP PROCEDURE [dbo].[CreateAutoPO]
END

GO 


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[CreateAutoPO]
AS
BEGIN 

-- ============================================================================================================================
-- Version:		<002> 
-- ============================================================================================================================ 
-- Change Control
-- --------------
-- Date			By           Description
-- ----			--           -----------
-- 05-08-2020	Santosh		 Apply location filter as per AshleyDefaultStockLocation country setting 	
-- ============================================================================================================================ 

 select  * into  #AutoPO from (
 select a.*,case When (a.Diffrent<=0) then a.AvrageCount+ (a.Diffrent)-(a.Diffrent * 2) else a.TotalpoQuantity end As NewTotalpoQuantity  from (
 Select o.Id as BookingID,s.ID AS VendorID,s.Name AS VendorName, DATEADD(DAY,s.LeadTime,GETDATE()) RequestedDeliveryDate,pa.ProductId,Pa.SKU,
		P.LongDescription,t.StockOnHand,t.StockOnOrder,t.StockAvailable,o.Quantity,o.ACctNo,
		t.Locationid  as ReceivingLocationId,  ss.Name AS ReceivingLocation, 
		'[]' As ReferenceNumbers,'USD' AS Currency, 'UnApproved' As [Status], GETDATE() OriginalPrint, GETDATE() CreatedDate,
		'-77777' CreatedById, 'Auto PO' AS CreatedBy, s.PaymentTerms, 'Auto PO' OriginSystem,
		'' as CorporatePoNumber, Null ShipDate, Null ShipVia, Null PortOfLoading, Null Attributes, Null CommissionChargeFlag, Null CommissionPercentage,  
		Getdate()+(Select  ValueInt From config.setting Where id='DaysUntilAutoCancelPurchaseOrder') As ExpiredDate,0 IsPOCreated, 0 PurchaseOrderID,
		(PSR.MinVal+Psr.MaxVal)/2 As AvrageCount,'R' As TypesofProduct,
		CASE WHEN t.StockOnOrder-(select sum(Quantity) TotalQuantity  From [Warehouse].[Booking] T 
				  WHERE T.ItemNo=P.sku AND Convert(date,OrderedOn,102) >= Convert(Date,Getdate()-90,102) GROUP BY T.ItemNo ) > 0 THEN  
				  (PSR.MinVal+Psr.MaxVal)/2-(t.StockOnOrder-(Select Sum(t.Quantity) TotalQuantity 
				  From [Warehouse].[Booking] T WHERE T.ItemNo=P.sku   AND Convert(Date,OrderedOn,102) >= Convert(Date,Getdate()-90,102) GROUP BY T.ItemNo ))
		ELSE o.Quantity-t.StockOnHand+(PSR.MinVal+Psr.MaxVal)/2-t.StockOnOrder  END As TotalpoQuantity,null As POStatus,
		t.StockOnOrder-(select sum(Quantity) TotalQuantity  From [Warehouse].[Booking] T  WHERE T.ItemNo=P.sku  
		AND Convert(Date,OrderedOn,102) >= Convert(Date,Getdate()-90,102)) as Diffrent
	From   
		[Merchandising].ProductStockLevel t  with (Nolock)
		Inner Join [Merchandising].[Product] P with (Nolock)  
			On t.ProductId=p.id 
		Inner Join [Warehouse].[Booking] o with (Nolock)
			On p.SKU= o.ItemNo 
		Inner Join [Merchandising].location ss  with (Nolock)
			On ss.salesid=o.stockbranch and o.ItemNo=p.SKU and t.Locationid=ss.id
		Inner Join [Merchandising].[Supplier] S WITH(nOLOCK)
			ON p.PrimaryVendorId = s.id
		Inner JOIN [Merchandising].[ProductAttributes] pa WITH(nOLOCK)
			On p.id=Pa.ProductId
		Inner Join  [Merchandising].[ProductStockRanges] PSR WITH(nOLOCK)
			on Pa.ProductID=PSR.ProductId
		--inner join [dbo].[StockQuantity] SRS  with (Nolock)
		--	on P.Sku=SRS.ItemNo and srs.stocklocn=o.StockBranch
	Where convert(date,OrderedOn,102) >= convert(date,getdate()-90,102) 
	   and pa.IsAshleyProduct = 1 AND pa.IsSpecialProduct =0 
	   and PSR.MinVal>=T.StockOnOrder-(select sum(Quantity) TotalQuantity  From [Warehouse].[Booking] T  WHERE T.ItemNo=P.sku  
		--AND Convert(Date,OrderedOn,102) >= Convert(Date,Getdate()-8,102)
		) 

)a
Union All
	Select o.Id as BookingID,s.ID AS VendorID,s.Name AS VendorName, DATEADD(DAY,s.LeadTime,GETDATE()) RequestedDeliveryDate, pa.ProductId,Pa.SKU,P.LongDescription,
		t.StockOnHand,t.StockOnOrder,t.StockAvailable,o.Quantity,o.ACctNo,
		t.Locationid  as ReceivingLocationId,  ss.Name AS ReceivingLocation, '[]' as ReferenceNumbers,
		'USD' AS Currency, 'UnApproved' As [Status], GETDATE() OriginalPrint, GETDATE() CreatedDate,
		'-77777' CreatedById, 'Auto PO' AS CreatedBy, s.PaymentTerms, 'Auto PO' OriginSystem,
		Null CorporatePoNumber, null ShipDate, null ShipVia, null PortOfLoading, null Attributes, 
		Null CommissionChargeFlag, null CommissionPercentage,  getdate()+(select  ValueInt from config.setting where id='DaysUntilAutoCancelPurchaseOrder') as ExpiredDate,0 IsPOCreated, 0 PurchaseOrderID,
		o.Quantity as AvrageCount,'S' as TypesofProduct,o.Quantity  as TotalpoQuantity,
		(Select Top 1 rr.[Status] From [Merchandising].[PurchaseOrder] rr inner join  [Merchandising].[PurchaseOrderProduct] yy on rr.id=yy.PurchaseOrderId
		And yy.sku=o.ItemNo  Inner Join [Merchandising].[AutoPOMapping] q on q.CreatedPONo=rr.id and rr.Status Not In  ('Cancelled','Rejected')) as POStatus ,0 as Diffrent,o.Quantity  as NewTotalpoQuantity
	 From  
		[Merchandising].ProductStockLevel t 
		Inner Join [Merchandising].[Product] P  on t.ProductId=p.id
		Inner Join [Warehouse].[Booking] o On p.SKU= o.ItemNo 
		Inner Join [Merchandising].location ss  On ss.salesid=o.stockbranch and  o.ItemNo=p.SKU and t.Locationid=ss.id
		Inner JOIN [Merchandising].[ProductAttributes] pa On p.id=Pa.ProductId
		Inner JOIN [Merchandising].[Supplier] S ON p.PrimaryVendorId = s.id 
Where  Convert(Date,OrderedOn,102) >= Convert(Date,Getdate()-90,102) 
	And pa.IsAshleyProduct = 1 AND pa.IsSpecialProduct =1 and ISAutoPO=1
)b  Where b.NewTotalpoQuantity>0 
 
delete from #AutoPO where POStatus in ('UnApproved','Approved','PartiallyApproved') or CAST(Bookingid as varchar) +  sku + CAST(AcctNo as varchar) in 
 (select CAST(Bookingid as varchar) +  Itemno + CAST(AcctNo as varchar)
 From [Merchandising].[AutoPOMapping] )
 
--Drop table #AutoPO1
Select Distinct  VendorID,VendorName,RequestedDeliveryDate,ProductId,SKU,LongDescription,StockOnHand,StockOnOrder,StockAvailable,
		ReceivingLocationId,ReceivingLocation,ReferenceNumbers,Currency,[Status],OriginalPrint 
		,CreatedDate,CreatedById,CreatedBy,PaymentTerms,OriginSystem,CorporatePoNumber  	
		,ShipDate,ShipVia,PortOfLoading,Attributes,CommissionChargeFlag,CommissionPercentage,ExpiredDate,IsPOCreated,PurchaseOrderID,TypesofProduct, 
		Case  When (TypesofProduct='s') Then  (Select sum (NewTotalpoQuantity) From #AutoPO t  Where t.ProductId=#AutoPO.ProductId ) Else NewTotalpoQuantity End As NewTotalpoQuantity
	INTO #AutoPO1  
	From #AutoPO  

--Drop table #AutoPOFinal
SELECT ROW_NUMBER() OVER(ORDER BY x.VendorID ASC) AS Row#,x.* INTO #AutoPOFinal FROM (SELECT * FROM #AutoPO1) X

	  
--Drop table #t
Select count(Row#) SalesorderCountItem, VendorId,VendorName, RequestedDeliveryDate,ReceivingLocationId,ReceivingLocation,ReferenceNumbers,Currency,[Status],OriginalPrint,CreatedDate,CreatedById,CreatedBy,PaymentTerms,OriginSystem,CorporatePoNumber,ExpiredDate
INTO #AutoPofinal_New From #AutoPOFinal
Group By VendorId,VendorName, RequestedDeliveryDate,ReceivingLocationId,ReceivingLocation,ReferenceNumbers,Currency,[Status],OriginalPrint,CreatedDate,CreatedById,CreatedBy,PaymentTerms,OriginSystem,CorporatePoNumber,ExpiredDate

---Drop Table #t2
Select ROW_NUMBER() OVER(ORDER BY VendorId ASC) as roeNo,VendorId into #AutoPofinal_New2 from #AutoPofinal_New


BEGIN TRAN
	DECLARE @site_value INT;
	declare @site_value1  int =1;
	select @site_value=count(*)  from #AutoPofinal_New2 
WHILE  @site_value>=@site_value1
BEGIN
  		PRINT ('Inserting Data into [Merchandising].[PurchaseOrder] table')

INSERT INTO [Merchandising].[PurchaseOrder] 
		(VendorId, Vendor, RequestedDeliveryDate, ReceivingLocationId, ReceivingLocation,ReferenceNumbers, Currency, [Status], OriginalPrint, CreatedDate, 
		CreatedById, CreatedBy, PaymentTerms, OriginSystem,CorporatePoNumber,ExpiredDate)
	Select VendorId,VendorName, RequestedDeliveryDate,ReceivingLocationId,ReceivingLocation,ReferenceNumbers,Currency,[Status],OriginalPrint,CreatedDate,CreatedById,CreatedBy,
	PaymentTerms,OriginSystem,CorporatePoNumber,ExpiredDate From #AutoPofinal_New  where VendorId in (Select VendorId From #AutoPofinal_New2 Where RoeNo=@site_value1 )



Declare @PoId int 
SET @PoId = @@identity
print @PoId 



PRINT ('Inserting Data into [Merchandising].[ForceIndexAutoPurchaseOrder] Table')
-----Auto Re-Indexing 
INSERT INTO [Merchandising].[ForceIndexAutoPurchaseOrder] (PurchaseId,AutoManual,IsReIndexed)
Select @PoId as PurchaseOrderId,'A',0 

PRINT ('Inserting Data into [Merchandising].[PurchaseOrderProduct] Table')
INSERT INTO [Merchandising].[PurchaseOrderProduct] (ProductId, Sku, [Description],RequestedDeliveryDate, QuantityOrdered,UnitCost, EstimatedDeliveryDate, PurchaseOrderId, PreLandedUnitCost, LabelRequired)
Select p.ProductId,p.Sku,LongDescription As [Description],RequestedDeliveryDate,
	   NewTotalpoQuantity as QuantityOrdered,ss.SupplierCost As UnitCost,RequestedDeliveryDate,@PoId as PurchaseOrderId,LastLandedCost AS PreLandedUnitCost,1 AS LabelRequired
From #AutoPOFinal p 
inner join [Merchandising].[CurrentCostPriceView] ss 
	On p.ProductId=ss.ProductId 
Where VendorId in (Select VendorId From #AutoPofinal_New2 Where RoeNo=@site_value1 )   ---AND P.TypesofProduct='s'--- and p.ProductId='45490']

----Mapping for the Sales and Purchase order 
PRINT ('Inserting Data into [Merchandising].[AUTOMapping] Table')
 Insert INTO [Merchandising].[AutoPOMapping]
 Select PP.id As BookingID,PP.AcctNo,PP.ItemNo,T.NewTotalpoQuantity,TypesofProduct,T.[Status], @PoId as CreatedPONo,0 as XMLFileStatus,Null as XMLFileCreatedDate
 from #AutoPO t inner join  [Warehouse].[Booking] pp  with(Nolock) on t.BookingID=pp.id and pp.ItemNo=t.SKU 
 Where VendorId in (select VendorId From #AutoPofinal_New2 Where RoeNo=@site_value1 )

 ------Update Data on [Merchandising].ProductStockLevel as per Item and Location 
 UPDATE [Merchandising].ProductStockLevel
		SET StockOnOrder = StockOnOrder + POP.QuantityOrdered
		From 	 
		[Merchandising].[PurchaseOrderProduct] POP
		INNER JOIN
		[Merchandising].ProductStockLevel PSL 
		ON POP.ProductId = PSL.ProductId AND PSL.LocationId = (select top 1 o.id from CountryMaintenance t inner join Merchandising.Location o on t.Value=o.SalesId where codename = 'AshleyDefaultStockLocation')
		WHERE  PurchaseOrderId = @PoId    
set @site_value1=@site_value1+1
END
Commit
 	
END