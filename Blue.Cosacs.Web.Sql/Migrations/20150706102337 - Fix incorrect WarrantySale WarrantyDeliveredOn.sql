-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE 
    Warranty.WarrantySale
SET
    WarrantyDeliveredOn = d.datedel
FROM
    Warranty.WarrantySale ws
INNER JOIN 
    Delivery d on ws.CustomerAccount = d.acctno
    and d.contractno = ws.WarrantyContractNo
    and ws.WarrantyDeliveredOn != d.datedel
WHERE
    ws.[Status] = 'Active'
    and ws.WarrantyType != 'F'

