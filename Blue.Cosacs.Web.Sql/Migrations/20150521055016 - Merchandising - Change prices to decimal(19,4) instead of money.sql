-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


----------------------
-- PURCHASE ORDER PRODUCT
----------------------
ALTER TABLE Merchandising.PurchaseOrderProduct
drop constraint DF_Merchandising_PurchaseOrder_PreLandedExtendedUnitCost

ALTER TABLE Merchandising.PurchaseOrderProduct
drop constraint DF_Merchandising_PurchaseOrder_PreLandedUnitCost

ALTER TABLE Merchandising.PurchaseOrderProduct
ALTER COLUMN UnitCost Decimal(19,4) NOT NULL

ALTER TABLE Merchandising.PurchaseOrderProduct
ALTER COLUMN PreLandedUnitCost Decimal(19,4) NOT NULL 

ALTER TABLE Merchandising.PurchaseOrderProduct
ADD constraint DF_Merchandising_PurchaseOrder_PreLandedUnitCost DEFAULT (0) FOR PreLandedUnitCost

ALTER TABLE Merchandising.PurchaseOrderProduct
ALTER COLUMN PreLandedExtendedCost  Decimal(19,4) NOT NULL

ALTER TABLE Merchandising.PurchaseOrderProduct
ADD CONSTRAINT DF_Merchandising_PurchaseOrder_PreLandedExtendedUnitCost DEFAULT (0) FOR PreLandedExtendedCost

---------------------
-- Goods Receipt Product
---------------------

Update merchandising.GoodsReceiptProduct set lastlandedcost = 0 where lastlandedcost is null

ALTER TABLE Merchandising.GoodsReceiptProduct
ALTER COLUMN LastLandedCost Decimal(19,4) NOT NULL 


ALTER TABLE Merchandising.GoodsReceiptDirectProduct
ALTER COLUMN UnitLandedCost Decimal(19,4) NOT NULL	


---------------------
-- INCOTERM
---------------------	


ALTER TABLE Merchandising.Incoterm
ALTER COLUMN SupplierUnitCost Decimal(19,4) NULL	


--Remove existing Constraints
DECLARE @ObjectName NVARCHAR(100)



-----------------------
-- StockRequisitionProduct
-----------------------


SELECT @ObjectName = OBJECT_NAME([default_object_id]) FROM SYS.COLUMNS
WHERE [object_id] = OBJECT_ID('[Merchandising].[StockRequisitionProduct]') AND [name] = 'AverageWeightedCost';

if @objectName is not Null
EXEC('ALTER TABLE [Merchandising].[StockRequisitionProduct] DROP CONSTRAINT ' + @ObjectName)

ALTER TABLE Merchandising.StockRequisitionProduct
ALTER COLUMN AverageWeightedCost Decimal(19,4) NOT NULL	


ALTER TABLE Merchandising.StockRequisitionProduct
ADD CONSTRAINT DF_Merchandising_StockRequisitionProduct_AverageWeightedCost DEFAULT (0) FOR AverageWeightedCost

------------------------
--StockAdjustmentProduct
------------------------

SELECT @ObjectName = OBJECT_NAME([default_object_id]) FROM SYS.COLUMNS
WHERE [object_id] = OBJECT_ID('[Merchandising].[StockAdjustmentProduct]') AND [name] = 'AverageWeightedCost';
if @objectName is not Null
EXEC('ALTER TABLE [Merchandising].[StockAdjustmentProduct] DROP CONSTRAINT ' + @ObjectName)

ALTER TABLE Merchandising.StockAdjustmentProduct
ALTER COLUMN AverageWeightedCost Decimal(19,4) NOT NULL	


ALTER TABLE Merchandising.StockAdjustmentProduct
ADD CONSTRAINT DF_Merchandising_StockAdjustmentProduct_AverageWeightedCost DEFAULT (0) FOR AverageWeightedCost

------------------------
--StockAllocationProduct
------------------------

SELECT @ObjectName = OBJECT_NAME([default_object_id]) FROM SYS.COLUMNS
WHERE [object_id] = OBJECT_ID('[Merchandising].[StockAllocationProduct]') AND [name] = 'AverageWeightedCost';
if @objectName is not Null
EXEC('ALTER TABLE [Merchandising].[StockAllocationProduct] DROP CONSTRAINT ' + @ObjectName)


ALTER TABLE Merchandising.StockAllocationProduct
ALTER COLUMN AverageWeightedCost Decimal(19,4) NOT NULL	


ALTER TABLE Merchandising.StockAllocationProduct
ADD CONSTRAINT DF_Merchandising_StockAllocationProduct_AverageWeightedCost DEFAULT (0) FOR AverageWeightedCost

------------------------
--GoodsOnLoanProduct
------------------------

SELECT @ObjectName = OBJECT_NAME([default_object_id]) FROM SYS.COLUMNS
WHERE [object_id] = OBJECT_ID('[Merchandising].[GoodsOnLoanProduct]') AND [name] = 'AverageWeightedCost';
if @objectName is not Null
EXEC('ALTER TABLE [Merchandising].[GoodsOnLoanProduct] DROP CONSTRAINT ' + @ObjectName)


ALTER TABLE Merchandising.GoodsOnLoanProduct
ALTER COLUMN AverageWeightedCost Decimal(19,4) NOT NULL	


ALTER TABLE Merchandising.GoodsOnLoanProduct
ADD CONSTRAINT DF_Merchandising_GoodsOnLoanProduct_AverageWeightedCost DEFAULT (0) FOR AverageWeightedCost

------------------------
--StockTransferProduct
------------------------

SELECT @ObjectName = OBJECT_NAME([default_object_id]) FROM SYS.COLUMNS
WHERE [object_id] = OBJECT_ID('[Merchandising].[StockTransferProduct]') AND [name] = 'AverageWeightedCost';
if @objectName is not Null
EXEC('ALTER TABLE [Merchandising].[StockTransferProduct] DROP CONSTRAINT ' + @ObjectName)


ALTER TABLE Merchandising.StockTransferProduct
ALTER COLUMN AverageWeightedCost Decimal(19,4) NOT NULL



ALTER TABLE Merchandising.StockTransferProduct
ADD CONSTRAINT DF_Merchandising_StockTransferProduct_AverageWeightedCost DEFAULT (0) FOR AverageWeightedCost

