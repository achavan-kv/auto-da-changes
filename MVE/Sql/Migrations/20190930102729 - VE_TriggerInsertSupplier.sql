IF EXISTS (SELECT * 
           FROM   sysobjects 
           WHERE  NAME = 'VE_TriggerInsertSupplier' 
                  AND type = 'TR') 
  BEGIN 
      DROP TRIGGER [Merchandising].[VE_TriggerInsertSupplier] 
  END 

go 

CREATE TRIGGER [Merchandising].[VE_TriggerInsertSupplier] 
ON [Merchandising].[product] 
FOR UPDATE 
AS 
  BEGIN 
      DECLARE @SupplierCode VARCHAR(50) 
      DECLARE @CMDSQL VARCHAR(1000) 

      SELECT @SupplierCode = t1.code 
      FROM   inserted T0 
             INNER JOIN merchandising.supplier T1 
                     ON T0.primaryvendorid = t1.id 
      /* * 1. ServiceCode - * {vdr-Vendor}, * {pyt-Payment}, * {grn-GRN} * 2. Code - * {vdr-Vendor code}, * {pyt-Account number}, * {grn-grnId} * 3. isInsertRecord - * {true : Insert record}, * {false : Update records} * 4. isEODRecords - * {true : EOD record}, * {false :RealTime record} */

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
  AND MS.code = @SupplierCode) 
  BEGIN 
  IF @SupplierCode != '' 
  BEGIN 
  INSERT INTO ve_taskschedular 
  (servicecode, 
  code, 
  isinsertrecord, 
  iseodrecords, 
  status) 
  VALUES      ( 'vdr', 
  @SupplierCode, 
  1, 
  0, 
  0 ) 
  END 
  END 
  END 