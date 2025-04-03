IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'TestGetRandId'
            AND xtype = 'FN')
BEGIN 
DROP FUNCTION dbo.TestGetRandId
END
GO

Create FUNCTION [dbo].TestGetRandId()
RETURNS int
AS
BEGIN
RETURN (SELECT empeeno
FROM TestGetRandPersonView)
END
GO


