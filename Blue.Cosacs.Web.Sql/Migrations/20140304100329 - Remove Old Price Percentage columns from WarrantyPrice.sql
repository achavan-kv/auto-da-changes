
IF EXISTS(SELECT * FROM sys.columns
          WHERE Name = N'CostPricePercentageChange' AND OBJECT_ID = OBJECT_ID(N'Warranty.WarrantyPrice'))
BEGIN
    ALTER TABLE Warranty.WarrantyPrice
    DROP COLUMN CostPricePercentageChange
END

IF EXISTS(SELECT * FROM sys.columns
          WHERE Name = N'RetailPricePercentageChange' AND OBJECT_ID = OBJECT_ID(N'Warranty.WarrantyPrice'))
BEGIN
    ALTER TABLE Warranty.WarrantyPrice
    DROP COLUMN RetailPricePercentageChange
END

IF EXISTS(SELECT * FROM sys.columns
          WHERE Name = N'TaxInclusivePricePercentageChange' AND OBJECT_ID = OBJECT_ID(N'Warranty.WarrantyPrice'))
BEGIN
    ALTER TABLE Warranty.WarrantyPrice
    DROP COLUMN TaxInclusivePricePercentageChange
END
