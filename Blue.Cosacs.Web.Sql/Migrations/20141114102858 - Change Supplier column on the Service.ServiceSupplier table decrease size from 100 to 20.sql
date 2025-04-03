-- transaction: true

-- Trim supplier names if too big. The new max size is 20 characters
UPDATE [Service].[ServiceSupplier]
SET Supplier = LEFT(Supplier, 20)
WHERE LEN(Supplier) >= 20

-- Reduce column size from 100 to 20
ALTER TABLE [Service].[ServiceSupplier]
ALTER COLUMN Supplier VARCHAR(20) NOT NULL
