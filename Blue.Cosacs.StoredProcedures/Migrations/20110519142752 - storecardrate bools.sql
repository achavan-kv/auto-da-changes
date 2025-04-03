DECLARE @name NVARCHAR(1000)

SELECT @name = name FROM sysobjects s
WHERE xtype = 'd'
AND name LIKE '%storecard%'
AND OBJECT_NAME(parent_obj) = 'StoreCardrate'
AND crdate = (SELECT MAX(crdate) FROM sysobjects so
              WHERE so.name = s.name
              AND s.parent_obj = so.parent_obj
              AND s.xtype = so.xtype)

WHILE LEN(@name) != 0
BEGIN
  
SET @name = 'ALTER TABLE StoreCardRate DROP CONSTRAINT ' + @name

EXEC sp_executesql @name

SET @Name = (SELECT name FROM sysobjects 
WHERE xtype = 'd'
AND name LIKE '%storecard%'
AND OBJECT_NAME(parent_obj) = 'StoreCardrate')

END
GO

DELETE FROM StoreCardRate
GO

ALTER TABLE storecardrate
DROP COLUMN RateFixed
GO

ALTER TABLE storecardrate
DROP COLUMN isDefaultRate
GO

ALTER TABLE StoreCardRate
ADD RateFixed BIT NOT NULL 
GO

ALTER TABLE StoreCardRate
ADD IsDefaultRate BIT NOT NULL 
GO


