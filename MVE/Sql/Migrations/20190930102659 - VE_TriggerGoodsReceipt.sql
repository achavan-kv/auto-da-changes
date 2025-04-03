
IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'VE_TriggerGoodsReceipt' AND type = 'TR')
	BEGIN
		DROP TRIGGER [Merchandising].[VE_TriggerGoodsReceipt]
	END
GO

Create TRIGGER [Merchandising].[VE_TriggerGoodsReceipt]
ON [Merchandising].[GoodsReceipt]
AFTER UPDATE
AS 
IF UPDATE(ApprovedById)
BEGIN
    DECLARE @Id VARCHAR(50),
    @SupplierCode VARCHAR(50)
    SELECT top 1
        @Id=T0.Id,
        @SupplierCode=T3.VendorId
    FROM INSERTED T0 
    JOIN DELETED T1 on T0.Id= T1.Id
    INNER JOIN merchandising.GoodsReceipt T00 ON T00.Id=T0.Id
    INNER JOIN Merchandising.GoodsReceiptProduct T11 ON T00.Id=T11.GoodsReceiptId
    LEFT OUTER JOIN [Merchandising].[PurchaseOrderProduct] T2 ON T11.PurchaseOrderProductId=T2.Id
    LEFT OUTER JOIN [Merchandising].[PurchaseOrder] T3 ON T2.PurchaseOrderId=T3.Id
    WHERE T0.Id=T1.Id
    IF EXISTS(
            SELECT 1
            FROM Merchandising.Supplier MS
            WHERE MS.id in 
                    (
                        SELECT DISTINCT PrimaryVendorId AS PrimaryVendor 
                        FROM  merchandising.Product Pro 
                        WHERE MS.Id= Pro.PrimaryVendorId and
                        pro.Id IN 
                        (
                            SELECT ProductId FROM [Merchandising].[ProductHierarchyView] 
                            WHERE tagid ='17' and levelid ='1'
                        ) 
                        and Pro.ProductType='RegularStock'                
                    ) and Status=1 AND MS.Id = @SupplierCode
        )
    BEGIN
        IF @Id!=''
        BEGIN
IF NOT EXISTS (SELECT ServiceCode FROM VE_taskschedular WHERE Code=@Id AND Status='0')
BEGIN
        INSERT INTO VE_taskschedular (ServiceCode,Code,IsInsertRecord,IsEODRecords,Status) 
        VALUES('grn',@Id,1,0,0)
END
ELSE
BEGIN 
UPDATE VE_taskschedular SET Message='',Status='0' 
WHERE Code=@Id AND Status='1'
        END 
END
    END
END