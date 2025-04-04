if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_PurchaseOrderSave]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_PurchaseOrderSave]
END
GO
 
 
 
 
Create PROCEDURE [dbo].[VE_PurchaseOrderSave]
@PurchaseOrderxml XML
,@IsUpdate bit
,@Message VARCHAR(MAX) OUTPUT
,@StatusCode INT OUTPUT    
AS  
BEGIN  
  
SET NOCOUNT ON;   
SET @Message = '';
SET @StatusCode = 0

DECLARE @MVEPOId INT = N'',
@VendorId INT = N'',
@VendorCode VARCHAR(60) = N'',
@ReceivingLocationId INT,
@Currency VARCHAR(20) = N'',
@Comments VARCHAR(20) = N'',
@Status VARCHAR(20) = N'',
@CreatedDate datetime,
@CreatedById VARCHAR(20) = N'',
@PaymentTerms VARCHAR(60) = N'',
@OriginSystem VARCHAR(10) = N'',
@CommissionChargeFlag CHAR(1) = N'',
@CommissionPercentage VARCHAR(20) = N'',
@ExpiredDate datetime,
@VendorName VARCHAR(100) = N'',
@ReceivingLocationName VARCHAR(100) = N'',
@RequestedDeliveryDate date,
@CosacsPOId INT,
@Flag BIT = 0

SELECT @RequestedDeliveryDate = T.c.value('RequestedDeliveryDate[1]','date')
FROM @PurchaseOrderxml.nodes('/PurchaseOrderModel/PurchaseOrderItem/PurchaseOrderItems') T(c)
IF(@RequestedDeliveryDate IS NULL)
SET @RequestedDeliveryDate  = null

--NOTE: GET account No TO IDENTIFY THE DML OPERATIONs ONT THE WHICH account no BASIS
SELECT 
@MVEPOId = T.c.value('PurchaseOrderId[1]','INT'),
@VendorCode = T.c.value('VendorId[1]','VARCHAR(60)'),
@ReceivingLocationId = T.c.value('ReceivingLocationId[1]','INT'),
@Currency = T.c.value('Currency[1]','VARCHAR(20)'),
@Comments = T.c.value('Comments[1]','VARCHAR(20)'),
@Status = T.c.value('Status[1]','VARCHAR(20)'),
@CreatedDate = T.c.value('CreatedDate[1]','[datetime]'),
@CreatedById = 99995,
--  @CreatedById = (select CosacsUserId from VE_MVEUser where MVEUserId = @CreatedById), 
@PaymentTerms = T.c.value('PaymentTerms[1]','VARCHAR(20)'),
@OriginSystem = T.c.value('OriginSystem[1]','VARCHAR(20)'),
@CommissionChargeFlag = T.c.value('CommissionChargeFlag[1]','CHAR(1)'),
@CommissionPercentage = T.c.value('CommissionPercentage[1]','VARCHAR(20)'),
@ExpiredDate = T.c.value('ExpiredDate[1]','[datetime]')

FROM @PurchaseOrderxml.nodes('/PurchaseOrderModel') T(c)

if (@Status = 'Open')
BEGIN 
	SET @Status = 'New'
END
if (@Status = 'Hold')
BEGIN 
SET @Flag = 1
SET @Message = @Message + 'Purchase Order on Hold.'
SET @StatusCode = 404; 
END

IF(@CommissionChargeFlag IS NULL OR @CommissionChargeFlag = '')
SET @CommissionChargeFlag  = null
Select @VendorId = ID from merchandising.supplier Where code = @VendorCode
-----------------------------------------------------------
-- VALIDATE REQUIRED PurchaseOrderId -- START
IF(@MVEPOId = '' AND @MVEPOId IS NOT NULL)
BEGIN 
SET @Flag = 1
SET @Message = @Message + 'Purchase Order Id is Required.'
SET @StatusCode = 404; 
END   
-- VALIDATE REQUIRED PurchaseOrderId -- END
-----------------------------------------------------------
-- VALIDATE DUPLICATE PurchaseOrderId -- START
if NOT EXISTS(Select TOP 1 Id from [merchandising].[purchaseorder] mpo Where mpo.corporateponumber = CAST(@MVEPOId AS VArchar(20))) and @IsUpdate = 'true' and @Flag != 1
BEGIN 
SET @Flag = 1
SET @Message = @Message + 'Purchase Order Id not exists.'
SET @StatusCode = 404; 
END   
-- VALIDATE DUPLICATE PurchaseOrderId -- END
-----------------------------------------------------------
-- VALIDATE PurchaseOrderId -- START
if EXISTS(Select TOP 1 Id from [merchandising].[purchaseorder] mpo Where mpo.corporateponumber = CAST(@MVEPOId AS VArchar(20))) and @IsUpdate = 'false' and @Flag != 1
    BEGIN 
SET @Flag = 1
SET @Message = @Message + 'Purchase Order Id is already exists.'
SET @StatusCode = 404; 
END 
-- VALIDATE PurchaseOrderId -- END 
-----------------------------------------------------------
-- VALIDATE VendorId -- START
IF(@VendorId = '' AND @VendorId IS NOT NULL)
BEGIN 
SET @Flag = 1
SET @Message = @Message + 'VendorId is Required.'
SET @StatusCode = 404; 
END   
-- VALIDATE VendorId -- END
-----------------------------------------------------------
-- VALIDATE ReceivingLocationId -- START
IF(@ReceivingLocationId = '' AND @ReceivingLocationId IS NOT NULL)
BEGIN 
SET @Flag = 1
SET @Message = @Message + 'ReceivingLocationId is Required.'
SET @StatusCode = 404; 
END   
-- VALIDATE ReceivingLocationId -- END
-----------------------------------------------------------
-- VALIDATE Currency -- START
IF(@Currency = '' AND @Currency IS NOT NULL)
BEGIN 
SET @Flag = 1
SET @Message = @Message + 'Currency is Required.'
SET @StatusCode = 404; 
END   
-- VALIDATE Currency -- END
-----------------------------------------------------------
-- VALIDATE RequestedDeliveryDate -- START
IF(@RequestedDeliveryDate = '' AND @RequestedDeliveryDate IS NOT NULL)
BEGIN 
SET @Flag = 1
SET @Message = @Message + 'RequestedDeliveryDate is Required.'
SET @StatusCode = 404; 
END   
-- VALIDATE RequestedDeliveryDate -- END
-----------------------------------------------------------
-- VALIDATE PaymentTerms -- START
IF(@PaymentTerms = '' AND @PaymentTerms IS NOT NULL)
BEGIN 
SET @Flag = 1
SET @Message = @Message + 'PaymentTerms is Required.'
SET @StatusCode = 404; 
END   
-- VALIDATE PaymentTerms -- END
----------------------------------------------------------- 

(Select @VendorName = Name from merchandising.supplier Where id = @VendorId)
(Select @ReceivingLocationId = Id from merchandising.Location  Where Salesid = @ReceivingLocationId)
(Select @ReceivingLocationName = Name from merchandising.location Where id = @ReceivingLocationId)
--NOTE: CHECK temp TABLE ALREADY EXISTS 
IF OBJECT_ID('tempdb..#temp') IS NOT NULL
DROP TABLE #temp

Select 
  t.c.value('ProductCode[1]' ,'[varchar](20)') AS ProductCode
, t.c.value('RequestedDeliveryDate[1]' ,'date') AS RequestedDeliveryDate
, t.c.value('Quantity[1]' ,'int') AS Quantity
, t.c.value('VendorUnitCost[1]' ,'money') AS VendorUnitCost  
, t.c.value('VendorLineCost[1]' ,'money') AS VendorLineCost
, t.c.value('Comments[1]' ,'[varchar](20)') AS LineComments 
, mp.Id as ProductId
, mp.SKU as SKU
, mp.LongDescription as LongDescription
, '' as DMLAction
INTO #temp
FROM @PurchaseOrderxml.nodes('/PurchaseOrderModel/PurchaseOrderItem/PurchaseOrderItems') T(c) 
INNER JOIN merchandising.product mp ON t.c.value('ProductCode[1]' ,'[varchar](20)') = mp.SKU

	--NOTE: SET 'U' THE DMLACTION COLUMN VALUE WHICH ARE FOR UPDATE 		
	Update #temp SET DMLAction = 'U'		
	FROM #temp t INNER JOIN [Merchandising].[PurchaseOrderProduct] POP  		
	ON POP.productid = t.productid AND POP.PurchaseOrderId in (select Id from [Merchandising].[PurchaseOrder] where CorporatePoNumber = CAST(@MVEPOId AS VArchar(20)))		
		
	--NOTE: SET 'I' THE DMLACTION COLUMN VALUE WHICH ARE FOR INSERT 		
	Update #temp SET DMLAction = 'I'		
	Where DMLAction != 'U'		
	Select '#temp', * from #temp

BEGIN TRANSACTION
IF( @Flag = 0)
BEGIN 
IF (@IsUpdate = 'false')
-------------------------------------INSERT START
BEGIN
INSERT INTO [merchandising].[purchaseorder] 
(VendorId,
Vendor, RequestedDeliveryDate,
ReceivingLocationId, ReceivingLocation,
ReferenceNumbers, Currency,
Comments, [Status],
OriginalPrint,
CreatedDate,
CreatedById, CreatedBy,
PaymentTerms, OriginSystem,
CorporatePoNumber, -- THIS COLUMN USED TO SAVE MVE POId at COSACS end
ShipDate,
ShipVia, PortOfLoading,
Attributes,
CommissionChargeFlag,
CommissionPercentage, ExpiredDate
)

Select 
@VendorId, @VendorName, @RequestedDeliveryDate, @ReceivingLocationId, @ReceivingLocationName, NULL, @Currency, @Comments, @Status, NULL,
@CreatedDate, @CreatedById, 'MVE', @PaymentTerms, @OriginSystem, 
@MVEPOId, 
NULL, NULL, NULL, '[]',
@CommissionChargeFlag, @CommissionPercentage, CASE WHEN CONVERT(DATE, @ExpiredDate) = '1900-01-01' THEn NULL ELSE CONVERT(DATE, @ExpiredDate) END 
SELECT @CosacsPOId = IDENT_CURRENT('merchandising.purchaseorder')
Select @CosacsPOId as CosacsPOId

print 'ProductStockLevel1 '
update Merchandising.ProductStockLevel 
SET StockOnOrder = StockOnOrder + quantity 
FROM Merchandising.ProductStockLevel PSL inner join #temp t ON PSL.productid = t.productid and PSL.LocationId = @ReceivingLocationId and t.DMLAction = 'I'

print 'ProductStockLevel11 '
END
-------------------------------------INSERT END
 
DECLARE @ProductCode VARCHAR(20) = N'',
 
@Quantity INT,
@VendorUnitCost money,
@VendorLineCost money,
@LineComments VARCHAR(20) = N''
 
IF(@ProductCode IS NULL)
SET @ProductCode = 0; 
IF(@Quantity IS NULL) SET @Quantity = 1

IF(@VendorUnitCost IS NULL) SET @VendorUnitCost = 0

IF(@VendorLineCost IS NULL) SET @VendorLineCost = 0

IF(@LineComments IS NULL) SET @LineComments = N''

IF (@IsUpdate = 'false')
-------------------------------------INSERT LineItem START
BEGIN
INSERT INTO [Merchandising].[PurchaseOrderProduct]
(ProductId,
Sku, [Description],
RequestedDeliveryDate, QuantityOrdered,
UnitCost, Comments,
EstimatedDeliveryDate, 
PurchaseOrderId,
PreLandedUnitCost,
PreLandedExtendedCost,
LabelRequired, QuantityCancelled)
Select 
ProductId, SKU, LongDescription, RequestedDeliveryDate, Quantity, VendorUnitCost, LineComments, RequestedDeliveryDate AS EstimatedDeliveryDate,
@CosacsPOId, 0 as PreLandedUnitCost,
0 as PreLandedExtendedCost, 1 AS LabelRequired, Null   from #temp where DMLAction = 'I'
END
-------------------------------------INSERT LineItem END
ELSE IF (@IsUpdate = 'true' and @Status != 'Cancelled')
-------------------------------------UPDATE START
BEGIN
Select @CosacsPOId = ID from [merchandising].[purchaseorder] Where corporateponumber = CAST(@MVEPOId AS VArchar(20))

Select * from #temp
Print 'Update'
INSERT INTO [Merchandising].[PurchaseOrderProduct]
(ProductId,
Sku, [Description],
RequestedDeliveryDate, QuantityOrdered,
UnitCost, Comments,
EstimatedDeliveryDate,
PurchaseOrderId,
PreLandedUnitCost,
PreLandedExtendedCost,
LabelRequired, QuantityCancelled)
Select 
ProductId, SKU, LongDescription, RequestedDeliveryDate, Quantity, VendorUnitCost, LineComments, RequestedDeliveryDate AS EstimatedDeliveryDate,
@CosacsPOId, 0 as PreLandedUnitCost,0 as PreLandedExtendedCost, 1 AS LabelRequired, Null   from #temp where DMLAction = 'I'
 
update Merchandising.ProductStockLevel 
SET StockOnOrder = StockOnOrder + quantity 
FROM Merchandising.ProductStockLevel PSL inner join #temp t ON PSL.productid = t.productid and PSL.LocationId = @ReceivingLocationId and t.DMLAction = 'I'

update Merchandising.ProductStockLevel SET StockOnOrder = (CASE WHEN t.quantity = 0 THEN (StockOnOrder - QuantityOrdered) WHEN quantity != 0 THEN (StockOnOrder - QuantityOrdered) +t.quantity END)
FROM Merchandising.ProductStockLevel PSL inner Join [Merchandising].[PurchaseOrderProduct] POP on PSL.productid = POP.productid and PSL.LocationId = @ReceivingLocationId  
inner join #temp t on t.productid = POP.productid 
and POP.PurchaseOrderId = (select ID from [merchandising].[purchaseorder] Where corporateponumber = CAST(@MVEPOId AS VArchar(20))) and DMLAction = 'U'


Update [Merchandising].[PurchaseOrderProduct] SET QuantityOrdered = Quantity, UnitCost = VendorUnitCost
From [Merchandising].[PurchaseOrderProduct] POP Inner Join #temp t 
ON POP.ProductId = t.ProductId
where PurchaseOrderId = (select ID from [merchandising].[purchaseorder] Where corporateponumber = CAST(@MVEPOId AS VArchar(20))) and DMLAction = 'U'

DELETE w from Merchandising.PurchaseOrderProduct w inner join #temp t ON w.productid = t.productid and DMLAction = 'U' and QuantityOrdered = 0

END
-------------------------------------UPDATE END

IF (@IsUpdate = 'true' and @Status = 'Cancelled')
BEGIN
	update Merchandising.ProductStockLevel 
	SET StockOnOrder = StockOnOrder - mpo.QuantityOrdered 
	FROM Merchandising.ProductStockLevel PSL inner join [Merchandising].[PurchaseOrderProduct] mpo 
		ON PSL.productid = mpo.productid and PSL.LocationId = @ReceivingLocationId 
		where PurchaseOrderId = (select ID from [merchandising].[purchaseorder] Where corporateponumber = CAST(@MVEPOId AS VArchar(20)))

	update [Merchandising].[PurchaseOrderProduct] SET QuantityCancelled = QuantityOrdered FROM [Merchandising].[PurchaseOrderProduct] mpo 
	where PurchaseOrderId = (select ID from [merchandising].[purchaseorder] Where corporateponumber = CAST(@MVEPOId AS VArchar(20)))

	update [Merchandising].[PurchaseOrder] SET [Status] = 'Cancelled' FROM [Merchandising].[PurchaseOrder] mpo Where mpo.corporateponumber = CAST(@MVEPOId AS VArchar(20))

END
END

PRINT @Message
IF (@@error != 0)
BEGIN
ROLLBACK
print'Transaction rolled back'
SET @StatusCode = 500;
 
END
ELSE
BEGIN

if @isUpdate  = 'false' and @Flag = 0
BEGIN
SET @StatusCode = 201;
 
SET @Message = CAST(@CosacsPOId as VARCHAR(MAX))    
END
else if @isUpdate  = 'true' and @Flag = 0
BEGIN
SET @StatusCode = 202;  
SET @Message = CAST(@CosacsPOId as VARCHAR(MAX))          
END
                      
COMMIT

IF OBJECT_ID('tempdb..#temp') IS NOT NULL
DROP TABLE #temp

print'Transaction committed'
PRINT @Message
END
END