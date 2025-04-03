-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


INSERT INTO [service].SupplierCost
SELECT c.codedescript, p.Product, p.[Year], p.PartType, p.PartPercent, p.PartLimit, 
	p.LabourPercent, p.LabourLimit, p.AdditionalPercent, p.AdditionalLimit FROM dbo.SR_PriceIndexMatrix p
INNER JOIN code c ON p.supplier = c.codedescript
WHERE NOT EXISTS(SELECT * FROM [service].SupplierCost s1
					WHERE s1.Supplier = p.Supplier
					AND s1.Product = p.Product
					AND s1.[Year] = p.[Year]
					AND s1.PartType = p.PartType)
AND c.category = 'SRSUPPLIER'

IF NOT EXISTS(SELECT * FROM admin.Permission WHERE id = 1602)
BEGIN
	exec Admin.AddPermission 1602, 'Supplier Cost Matrix', 16, 'Allows access to the Supplier Cost Matrix screen'
END
