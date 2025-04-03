-- transaction: true

IF EXISTS ( SELECT 1 FROM sys.indexes
            WHERE name='UIDX_Warranty_WarrantyReturn__unique_return_codes'
                AND object_id = OBJECT_ID('Warranty.WarrantyReturn' ) )
    DROP INDEX UIDX_Warranty_WarrantyReturn__unique_return_codes ON Warranty.WarrantyReturn
	
IF EXISTS ( SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA='Warranty' AND Table_Name='WarrantyReturn' AND Column_Name='FreeWarrantyLength' )
    CREATE UNIQUE INDEX UIDX_Warranty_WarrantyReturn__unique_return_codes 
    ON Warranty.WarrantyReturn (WarrantyLength, FreeWarrantyLength, ElapsedMonths, Level_1, BranchNumber)

