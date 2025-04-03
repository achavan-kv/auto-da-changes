Declare @Cons_Name NVARCHAR(100)
Declare @Str NVARCHAR(500)

SELECT @Cons_Name=name
FROM sys.objects
WHERE type='UQ' AND OBJECT_NAME(parent_object_id) = N'Tag';

---- Delete the unique constraint.
SET @Str='ALTER TABLE Warranty.Tag DROP CONSTRAINT ' + @Cons_Name;
Exec (@Str)
GO

ALTER TABLE Warranty.Tag
ADD CONSTRAINT UQ_Level_Tag UNIQUE(LevelId, Name)
GO
