IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'TestPickingRand'
            AND xtype = 'FN')
BEGIN 
DROP FUNCTION dbo.TestPickingRand
END
GO

Create FUNCTION [dbo].TestPickingRand()
RETURNS int
AS
BEGIN
RETURN (SELECT id
FROM TestGetRandPickingView)
END
GO


