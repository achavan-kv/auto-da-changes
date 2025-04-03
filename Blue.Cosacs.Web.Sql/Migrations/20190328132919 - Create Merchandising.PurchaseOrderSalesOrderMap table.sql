IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'PurchaseOrderSalesOrderMap'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN

CREATE table Merchandising.PurchaseOrderSalesOrderMap
(
	salesOrder int,
	purchaseOrder int,
	createddate datetime default getdate()
)
END
GO