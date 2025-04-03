-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
DECLARE @Name varchar(max)

Set @Name = (SELECT name AS ConstraintName
FROM sys.default_constraints
Where OBJECT_NAME(parent_object_id) = 'ProductStatus' and name like '%isava%')


exec('ALTER TABLE Merchandising.ProductStatus Drop Constraint ' + @Name)
