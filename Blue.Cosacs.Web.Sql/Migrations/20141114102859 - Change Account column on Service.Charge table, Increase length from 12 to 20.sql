-- transaction: true

-- Increase column size from 12 to 20
ALTER TABLE [Service].[Charge]
ALTER COLUMN Account VARCHAR(20) NULL
