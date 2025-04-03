-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


--Remove existing Constraints
DECLARE @ObjectName NVARCHAR(100)
SELECT @ObjectName = OBJECT_NAME([default_object_id]) FROM SYS.COLUMNS
WHERE [object_id] = OBJECT_ID('[Merchandising].[costprice]') AND [name] = 'SupplierCost';
EXEC('ALTER TABLE [Merchandising].[costprice] DROP CONSTRAINT ' + @ObjectName)

SELECT @ObjectName = OBJECT_NAME([default_object_id]) FROM SYS.COLUMNS
WHERE [object_id] = OBJECT_ID('[Merchandising].[costprice]') AND [name] = 'LastLandedCost';
EXEC('ALTER TABLE [Merchandising].[costprice] DROP CONSTRAINT ' + @ObjectName)

SELECT @ObjectName = OBJECT_NAME([default_object_id]) FROM SYS.COLUMNS
WHERE [object_id] = OBJECT_ID('[Merchandising].[costprice]') AND [name] = 'AverageWeightedCost';
EXEC('ALTER TABLE [Merchandising].[costprice] DROP CONSTRAINT ' + @ObjectName)

--Alter column type
ALTER TABLE Merchandising.costprice 
ALTER column SupplierCost Decimal(19,4) not null 

ALTER TABLE Merchandising.costprice 
ALTER column LastLandedCost Decimal(19,4) not null

ALTER TABLE Merchandising.costprice 
ALTER column AverageWeightedCost Decimal(19,4) not null

--Add named Default constraints
ALTER TABLE Merchandising.costprice 
ADD CONSTRAINT DF_SupplierCost DEFAULT (0) for SupplierCost

ALTER TABLE Merchandising.costprice 
ADD CONSTRAINT DF_LastLandedCost DEFAULT (0) for LastLandedCost

ALTER TABLE Merchandising.costprice 
ADD CONSTRAINT DF_AverageWeightedCost DEFAULT (0) for AverageWeightedCost

