DECLARE @ConstraintName VARCHAR(256)
SET @ConstraintName = (
     SELECT             obj.name
     FROM               sys.columns col 

     LEFT OUTER JOIN    sys.objects obj 
     ON                 obj.object_id = col.default_object_id 
     AND                obj.type = 'D' 

     WHERE              col.object_id = OBJECT_ID('Merchandising.SupplierStatus') 
     AND                obj.name IS NOT NULL
     AND                col.name = 'IsActive'
)   

IF(@ConstraintName IS NOT NULL)
BEGIN
    EXEC ('ALTER TABLE [Merchandising].[SupplierStatus] DROP CONSTRAINT ['+@ConstraintName+']')
END

alter table merchandising.supplierstatus
drop column isactive

update Merchandising.Supplier
set Status = 1
where Status > 2

delete from Merchandising.SupplierStatus
where id > 2
