DECLARE @name NVARCHAR(100)

SET @Name = (SELECT TOP 1 name FROM sysobjects 
			WHERE OBJECT_NAME(parent_obj) = 'Storecard'
			AND xtype = 'D')

WHILE LEN(@name) > 0
BEGIN 

	SET @name = 'ALTER TABLE StoreCard DROP CONSTRAINT ' + @name
    EXEC sp_executesql @name
    
SET @Name = (SELECT TOP 1 name FROM sysobjects 
			WHERE OBJECT_NAME(parent_obj) = 'Storecard'
			AND xtype = 'D')
END
GO


ALTER TABLE StoreCard
DROP COLUMN [$Version]
GO

ALTER TABLE StoreCard
DROP COLUMN [$isdeleted]
GO

ALTER TABLE StoreCard
DROP COLUMN LostorStolenOn
GO

ALTER TABLE StoreCard
DROP COLUMN id
GO

ALTER TABLE StoreCard
DROP COLUMN CardIssued
GO

ALTER TABLE StoreCard
DROP COLUMN ExportRunNo
GO

ALTER TABLE StoreCard
ADD ExportRunNo INT NULL
GO

