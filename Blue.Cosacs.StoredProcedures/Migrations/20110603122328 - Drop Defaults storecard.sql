ALTER TABLE StoreCard
ADD CustID VARCHAR(20) NOT NULL DEFAULT ''


DECLARE @name NVARCHAR(1000)

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


 