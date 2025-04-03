-- transaction: true

DECLARE @Command varchar(500) = ''

-- Drop default constraint
SELECT @Command = 'ALTER TABLE Service.Request DROP CONSTRAINT ' + dc.name
FROM sys.tables t
JOIN sys.default_constraints dc
    ON dc.parent_object_id = t.object_id
JOIN sys.columns c
    ON c.object_id = t.object_id
    AND c.column_id = dc.parent_column_id
WHERE t.name='Request'
    AND c.name='ManWarrantyLength'

-- Drop default constraint
EXECUTE (@Command)

-- Change column [Service].[Request].ManWarrantyLength type to tinyint
ALTER TABLE [Service].[Request]
ALTER COLUMN ManWarrantyLength TINYINT

-- Recreate default constraint
ALTER TABLE [Service].[Request]
ADD CONSTRAINT DF__Service_Request__ManWarrantyLength DEFAULT 0 FOR ManWarrantyLength


-- Change column [Service].[Request].WarrantyLength type to tinyint
ALTER TABLE [Service].[Request]
ALTER COLUMN WarrantyLength TINYINT
