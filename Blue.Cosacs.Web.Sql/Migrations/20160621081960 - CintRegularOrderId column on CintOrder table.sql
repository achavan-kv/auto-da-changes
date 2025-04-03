-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT 1 FROM sys.columns c where c.name = 'CintRegularOrderId' and object_id = OBJECT_ID('Merchandising.CintOrder'))
	ALTER TABLE Merchandising.CintOrder
		ADD CintRegularOrderId	Int NULL CONSTRAINT FK_CintOrder_ID_CintRegularOrderId FOREIGN KEY REFERENCES Merchandising.CintOrder(Id)
GO

UPDATE Merchandising.CintOrder
	SET CintRegularOrderId = Data.RegularOrderId
FROM 
(
	SELECT 
		c.id, 
		co.id AS RegularOrderId
	from
		Merchandising.CintOrder c
		INNER Join Merchandising.CintOrder AS co 
			ON c.Id <> co.Id 
			AND co.PrimaryReference = c.PrimaryReference 
			AND co.StockLocation = c.StockLocation 
			AND co.Sku = c.Sku 
			AND co.[Type] = 'RegularOrder' 
			AND c.[Type] in ('Delivery', 'Return', 'Repossession')
			AND (
					(
						co.ReferenceType = 'invoice' 
						AND co.SecondaryReference = c.SecondaryReference
					) OR co.ReferenceType != 'invoice'
				) 
			AND co.TransactionDate =
			(	
				SELECT MAX(TransactionDate) AS Expr1
				FROM  Merchandising.CintOrder AS co3
				WHERE 
					co3.[Type] = 'RegularOrder'
					AND co3.PrimaryReference = co.PrimaryReference
					AND co3.StockLocation = co.StockLocation
					AND co3.Sku = co.Sku
					AND
					(
						(
							co3.ReferenceType = 'invoice' 
							AND co3.SecondaryReference = co.SecondaryReference
						) OR co3.ReferenceType != 'invoice'
					) 
			)
) Data
WHERE 
	Merchandising.CintOrder.Id = data.Id