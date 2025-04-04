IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'StripTimeMinusSecond'
            AND xtype = 'FN')
BEGIN 
DROP FUNCTION dbo.StripTimeMinusSecond
END
GO

CREATE FUNCTION StripTimeMinusSecond (@date DATETIME)
RETURNS DATETIME
AS
BEGIN
	RETURN(SELECT DATEADD(second,-1,CAST(FLOOR( CAST( @date AS FLOAT ) ) AS DATETIME)))
END
GO