-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


--Update deleted Non Stocks to inactive for migrated data.
UPDATE NonStocks.NonStock
SET Active = CASE 
                WHEN isnull(si.prodstatus, '') = 'D'
                    THEN 0
                ELSE 1  
             END
FROM 
    Stockinfo si
INNER JOIN 
    NonStocks.NonStock n ON si.SKU = n.SKU
    and si.IUPC = n.IUPC


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'NonStock' AND Column_Name = 'VendorUPC'
           AND TABLE_SCHEMA = 'NonStocks')
BEGIN
	ALTER TABLE NonStocks.NonStock ADD VendorUPC VARCHAR(18) NULL
END
GO

UPDATE NonStocks.NonStock
SET SKU = si.itemno,
    VendorUPC = si.VendorEAN
FROM 
    Stockinfo si
INNER JOIN 
    NonStocks.NonStock n ON si.SKU = n.SKU
    and si.IUPC = n.IUPC


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_TYPE='UNIQUE' AND TABLE_SCHEMA='NonStocks'
        AND TABLE_NAME = 'NonStock' AND CONSTRAINT_NAME = 'UQ_NonStocks_NonStock_IUPC')
BEGIN
    ALTER TABLE [NonStocks].[NonStock] DROP CONSTRAINT [UQ_NonStocks_NonStock_IUPC]
END
GO


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'NonStock' AND Column_Name = 'IUPC'
           AND TABLE_SCHEMA = 'NonStocks')
BEGIN
	ALTER TABLE NonStocks.NonStock DROP COLUMN IUPC
END
GO



