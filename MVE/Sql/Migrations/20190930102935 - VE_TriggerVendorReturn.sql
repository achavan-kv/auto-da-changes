IF EXISTS (SELECT * 
           FROM   sysobjects 
           WHERE  NAME = 'VE_TriggerVendorReturn' 
                  AND type = 'TR') 
  BEGIN 
      DROP TRIGGER [Merchandising].[VE_TriggerVendorReturn] 
  END 

go 

CREATE TRIGGER [Merchandising].[VE_TriggerVendorReturn] 
ON [Merchandising].[vendorreturn] 
after UPDATE 
AS 
    IF UPDATE (approvedbyid) 
      BEGIN 
          DECLARE @Id           VARCHAR(50), 
                  @SupplierCode VARCHAR(50) 

          SELECT TOP 1 @Id = T0.id, 
                       @SupplierCode = PO.vendorid 
          FROM   inserted T0 
                 JOIN deleted T1 
                   ON T0.id = T1.id --add this line 
                 INNER JOIN [Merchandising].[vendorreturnproduct] vrp 
                         ON T0.id = vrp.vendorreturnid 
                 INNER JOIN [Merchandising].[goodsreceiptproduct] grp 
                         ON T0.goodsreceiptid = grp.goodsreceiptid 
                 INNER JOIN [Merchandising].[purchaseorderproduct] pop 
                         ON pop.id = grp.purchaseorderproductid 
                 INNER JOIN [Merchandising].[purchaseorder] Po 
                         ON PO.id = pop.purchaseorderid 
          WHERE  T0.id = T1.id 

          IF EXISTS (SELECT 1 
                     FROM   merchandising.supplier MS 
                     WHERE  MS.id IN (SELECT DISTINCT primaryvendorid AS 
                                                      PrimaryVendor 
                                      FROM   merchandising.product Pro 
                                      WHERE  MS.id = Pro.primaryvendorid 
                                             AND pro.id IN (SELECT productid 
                                                            FROM 
      [Merchandising].[producthierarchyview] 
                 WHERE  tagid = '17' 
                        AND levelid = '1') 
      AND Pro.producttype = 'RegularStock') 
      AND status = 1 
      AND MS.id = @SupplierCode) 
      BEGIN 
      IF @Id != '' 
      BEGIN 
      IF NOT EXISTS (SELECT servicecode 
      FROM   ve_taskschedular 
      WHERE  code = @Id 
      AND status = '0') 
      BEGIN 
      INSERT INTO ve_taskschedular 
      (servicecode, 
      code, 
      isinsertrecord, 
      iseodrecords, 
      status) 
      VALUES      ( 'vrn', 
      @Id, 
      1, 
      0, 
      0 ) 
      END 
      END 
      ELSE 
      BEGIN 
      UPDATE ve_taskschedular 
      SET    message = '', 
      status = '0' 
      WHERE  code = @Id 
      AND status = '1' 
      END 
      END 
      END 