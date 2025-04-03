-- transaction: true

IF NOT EXISTS ( SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA='Warranty' AND Table_Name='WarrantyReturn' AND Column_Name='FreeWarrantyLength')
    ALTER TABLE Warranty.WarrantyReturn
    ADD FreeWarrantyLength TINYINT NULL
