IF OBJECT_ID('[Warranty].[WarrantyPriceRepositoryBulkInsertTestScript]') IS NOT NULL
BEGIN
    -- Exec SP to delete the values...
    EXEC [Warranty].[WarrantyPriceRepositoryBulkInsertTestScript] -1, '2014'
    DROP PROCEDURE [Warranty].[WarrantyPriceRepositoryBulkInsertTestScript]
END
