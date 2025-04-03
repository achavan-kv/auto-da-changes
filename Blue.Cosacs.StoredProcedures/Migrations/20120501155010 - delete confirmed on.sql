IF EXISTS (SELECT * FROM syscolumns 
WHERE name = 'deliveryconfirmedon')

ALTER TABLE Warehouse.load
DROP COLUMN deliveryconfirmedon
GO

IF EXISTS (SELECT * FROM syscolumns 
WHERE name = 'deliveryconfirmedbyname')

ALTER TABLE Warehouse.load
DROP COLUMN deliveryconfirmedbyname
GO

