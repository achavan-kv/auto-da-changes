-- transaction: true
IF EXISTS (SELECT * FROM sys.indexes 
           WHERE name='IDX_WarrantySale_CustId_ItemId_StockLocation' AND OBJECT_ID = OBJECT_ID('Warranty.WarrantySale'))
BEGIN
    DROP INDEX IDX_WarrantySale_CustId_ItemId_StockLocation ON [Warranty].[WarrantySale]
END

CREATE NONCLUSTERED INDEX IDX_WarrantySale_CustId_ItemId_StockLocation
    ON [Warranty].[WarrantySale] ([CustomerId], [ItemId], [StockLocation]) INCLUDE (
        [Id], [SoldOn], [SoldBy], [CustomerAccount], [CustomerTitle], [CustomerFirstName], [CustomerLastName],
        [CustomerAddressLine1], [CustomerAddressLine2], [CustomerAddressLine3], [CustomerPostcode], [ItemNumber],
        [ItemUPC], [ItemPrice], [ItemDescription], [ItemSupplier], [WarrantyContractNo], [WarrantyNumber],
        [WarrantyLength], [Status], [CustomerNotes], [ItemCostPrice], [ItemDeliveredOn], [WarrantyGroupId],
        [SoldById], [WarrantyType]
    )
GO
