IF OBJECT_ID('CacheGetChange') IS NOT NULL
BEGIN
	DROP PROCEDURE CacheGetChange
END
GO

CREATE PROCEDURE CacheGetChange
@TableName VARCHAR(50)
AS
SELECT ChangedOn
FROM [dbo].[CacheTableChange]
WHERE TableName = @TableName