-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists(select null from sys.columns c
inner join sys.objects o on o.object_id = c.object_id
inner join sys.schemas s on s.schema_id = o.schema_id
where c.name = 'LastLandedCostUpdated' 
	and object_name(c.object_id) = 'CostPrice' 
	and s.name = 'Merchandising')
BEGIN

	EXEC sp_RENAME 'Merchandising.CostPrice.LastUpdated' , 'LastLandedCostUpdated', 'COLUMN'

	ALTER TABLE Merchandising.CostPrice
	ADD AverageWeightedCostUpdated DATETIME 
END



