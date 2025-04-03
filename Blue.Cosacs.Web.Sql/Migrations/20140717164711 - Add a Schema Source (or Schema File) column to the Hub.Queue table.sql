 
ALTER TABLE [Hub].[Queue]
ADD SchemaSource VARCHAR(20) NOT NULL DEFAULT('')
GO

UPDATE Hub.Queue
SET SchemaSource = 'Warehouse.xsd'
WHERE Id IN (1,2,3,4)

UPDATE Hub.Queue
SET SchemaSource = 'Service.xsd'
WHERE Id IN (5,6,7,8,9,11,15)

UPDATE Hub.Queue
SET SchemaSource = 'Warranty.xsd'
WHERE Id IN (10,12,14,16,17,18)
