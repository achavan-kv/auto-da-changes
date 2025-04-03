-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

  UPDATE
            StockInfo
        SET
            WarrantyLength = w.[Length]
        FROM
            StockInfo si
        INNER JOIN 
            Warranty.Warranty w on w.Number = si.itemno
        WHERE
            si.WarrantyLength != w.[Length]