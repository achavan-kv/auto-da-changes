
-- change data type for columns CostPrice and RetailPrice
IF EXISTS(SELECT * FROM sys.columns
          WHERE Name = N'CostPrice' and OBJECT_ID = OBJECT_ID(N'Warranty.WarrantyPrice'))
BEGIN
    ALTER TABLE Warranty.WarrantyPrice
    ALTER COLUMN CostPrice BlueAmount NULL
END

IF EXISTS(SELECT * FROM sys.columns
          WHERE Name = N'RetailPrice' and OBJECT_ID = OBJECT_ID(N'Warranty.WarrantyPrice'))
BEGIN
    ALTER TABLE Warranty.WarrantyPrice
    ALTER COLUMN RetailPrice BlueAmount NULL
END



-- add columns CostPriceChange and CostPricePercentChange
IF NOT EXISTS(SELECT * FROM sys.columns
              WHERE Name = N'CostPriceChange' AND OBJECT_ID = OBJECT_ID(N'Warranty.WarrantyPrice'))
BEGIN
    ALTER TABLE Warranty.WarrantyPrice
    ADD CostPriceChange BlueAmount NULL
END

IF NOT EXISTS(SELECT * FROM sys.columns
              WHERE Name = N'CostPricePercentChange' AND OBJECT_ID = OBJECT_ID(N'Warranty.WarrantyPrice'))
BEGIN
    ALTER TABLE Warranty.WarrantyPrice
    ADD CostPricePercentChange BluePercentage NULL
END



-- add columns RetailPriceChange and RetailPricePercentChange
IF NOT EXISTS(SELECT * FROM sys.columns
              WHERE Name = N'RetailPriceChange' AND OBJECT_ID = OBJECT_ID(N'Warranty.WarrantyPrice'))
BEGIN
    ALTER TABLE Warranty.WarrantyPrice
    ADD RetailPriceChange BlueAmount NULL
END

IF NOT EXISTS(SELECT * FROM sys.columns
              WHERE Name = N'RetailPricePercentChange' AND OBJECT_ID = OBJECT_ID(N'Warranty.WarrantyPrice'))
BEGIN
    ALTER TABLE Warranty.WarrantyPrice
    ADD RetailPricePercentChange BluePercentage NULL
END



-- add columns TaxInclusivePriceChange and TaxInclusivePricePercentChange
IF NOT EXISTS(SELECT * FROM sys.columns
              WHERE Name = N'TaxInclusivePriceChange' AND OBJECT_ID = OBJECT_ID(N'Warranty.WarrantyPrice'))
BEGIN
    ALTER TABLE Warranty.WarrantyPrice
    ADD TaxInclusivePriceChange BlueAmount NULL
END

IF NOT EXISTS(SELECT * FROM sys.columns
              WHERE Name = N'TaxInclusivePricePercentChange' AND OBJECT_ID = OBJECT_ID(N'Warranty.WarrantyPrice'))
BEGIN
    ALTER TABLE Warranty.WarrantyPrice
    ADD TaxInclusivePricePercentChange BlueAmount NULL
END



-- add column BulkEditId
IF NOT EXISTS(SELECT * FROM sys.columns
              WHERE Name = N'BulkEditId' AND OBJECT_ID = OBJECT_ID(N'Warranty.WarrantyPrice'))
BEGIN
    ALTER TABLE Warranty.WarrantyPrice
    ADD BulkEditId INT NULL
END
