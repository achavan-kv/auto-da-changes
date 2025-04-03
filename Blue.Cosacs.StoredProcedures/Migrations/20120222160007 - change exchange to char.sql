DECLARE @name NVARCHAR(100)


SET @name = (SELECT TOP 1  name FROM sysobjects
				 WHERE xtype = 'D'
				 AND OBJECT_NAME(parent_obj) = 'ExchangeRate')

WHILE @name IS NOT NULL
BEGIN
	EXEC ('ALTER TABLE ExchangeRate
	       DROP CONSTRAINT ' + @name)
	
	
	SET @name = (SELECT TOP 1  name FROM sysobjects
				 WHERE xtype = 'D'
				 AND OBJECT_NAME(parent_obj) = 'ExchangeRate')       
	       
END



SET @Name = (SELECT TOP 1 name FROM sysobjects 
			WHERE OBJECT_NAME(parent_obj) = 'ExchangeRate'
			AND xtype = 'C')

WHILE LEN(@name) > 0
BEGIN 

	SET @name = 'ALTER TABLE ExchangeRate DROP CONSTRAINT ' + @name
    EXEC sp_executesql @name
    
SET @Name = (SELECT TOP 1 name FROM sysobjects 
			WHERE OBJECT_NAME(parent_obj) = 'ExchangeRate'
			AND xtype = 'C')
END
GO



ALTER TABLE ExchangeRate
ALTER COLUMN status CHAR(1)

ALTER TABLE [dbo].[ExchangeRate]  WITH CHECK ADD CHECK  (([Status] = 'C' or [Status] = 'H'))
GO


ALTER TABLE [dbo].[ExchangeRate] ADD  DEFAULT ('C') FOR [Status]
GO


