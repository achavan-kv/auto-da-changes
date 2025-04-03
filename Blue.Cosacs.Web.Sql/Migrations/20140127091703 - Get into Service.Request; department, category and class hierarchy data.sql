-- Clean the Service.Request->ProductLevel fields before 'alter table' data type change statements
UPDATE [Service].[Request]
SET ProductLevel_1 = NULL,
    ProductLevel_2 = NULL,
    ProductLevel_3 = NULL

-- Change the data type of the ProductLevel fields (Service.Request table)
ALTER TABLE [Service].[Request]
ALTER COLUMN [ProductLevel_1] VARCHAR(12) NULL; -- data type like in table dbo.Code column 'category'
ALTER TABLE [Service].[Request]
ALTER COLUMN [ProductLevel_2] SMALLINT NULL; -- data type like in table dbo.StockInfo column 'category'
ALTER TABLE [Service].[Request]
ALTER COLUMN [ProductLevel_3] CHAR(3) NULL; -- data type like in table dbo.StockInfo column 'Class'

-- Build temp table with all existing product hierarchical data
IF OBJECT_ID('tempdb..#ServiceRequestHierarchicalData') IS NOT NULL
BEGIN
	DROP TABLE #ServiceRequestHierarchicalData
END

SELECT r.[id] AS Id, c.[category] AS Department, s.[category] AS Category, s.[class] AS Class
INTO #ServiceRequestHierarchicalData
FROM [Service].[Request] r
INNER JOIN StockInfo s ON r.ItemNumber LIKE s.itemno
INNER JOIN Code c ON s.category = c.code AND c.category IN ('PCE', 'PCW', 'PCF', 'PCO')
WHERE s.itemno IN (SELECT DISTINCT ItemNumber FROM [Service].[Request]) AND
      (r.[Type]='SI' OR r.[Type]='S' OR r.[Type]='II')
GROUP BY r.[id], c.[category], s.[category], s.[class]

UPDATE [Service].[Request]
SET ProductLevel_1 = tmp.Department,
    ProductLevel_2 = tmp.Category,
    ProductLevel_3 = tmp.Class
FROM [Service].[Request] sr
INNER JOIN #ServiceRequestHierarchicalData tmp
ON sr.Id = tmp.Id
