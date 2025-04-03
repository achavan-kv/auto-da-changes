-- transaction: true

UPDATE [Service].[Request]
SET ManWarrantyLength = 12
WHERE ManWarrantyLength > 99

UPDATE [Service].[Request]
SET WarrantyLength = 12
WHERE WarrantyLength > 99
