IF EXISTS (SELECT * 
           FROM   sysobjects 
           WHERE  NAME = 'VE_TriggerStockTransfer' 
                  AND type = 'TR') 
  BEGIN 
      DROP TRIGGER [Merchandising].[VE_TriggerStockTransfer] 
  END 

go 

CREATE TRIGGER [Merchandising].[VE_TriggerStockTransfer] 
ON [Merchandising].[stocktransferproduct] 
after INSERT 
AS 
    DECLARE @Id VARCHAR(50) 
    DECLARE @ProductId VARCHAR(50) 

    SELECT @Id = T0.stocktransferid, 
           @ProductId = T0.productid 
    FROM   inserted T0 

    IF EXISTS (SELECT 1 
               FROM   merchandising.product Pro 
               WHERE  pro.id IN (SELECT productid 
                                 FROM   [Merchandising].[producthierarchyview] 
                                 WHERE  tagid = '17' 
                                        AND levelid = '1') 
                      AND Pro.producttype = 'RegularStock' 
                      AND Pro.id = @ProductId) 
      BEGIN 
          IF @Id != '' 
            BEGIN 
                IF NOT EXISTS (SELECT * 
                               FROM   ve_taskschedular 
                               WHERE  servicecode = 'StkTrf' 
                                      AND code = @Id 
                                      AND status = '0') 
                  BEGIN 
                      INSERT INTO ve_taskschedular 
                                  (servicecode, 
                                   code, 
                                   isinsertrecord, 
                                   iseodrecords, 
                                   status) 
                      VALUES      ( 'StkTrf', 
                                    @Id, 
                                    1, 
                                    0, 
                                    0 ) 
                  END 
            END 
      END 