IF EXISTS (SELECT * FROM sysobjects 
		   WHERE name = 'CheckConfig'
		   AND xtype = 'P')
BEGIN 
DROP PROCEDURE CheckConfig
END
GO

CREATE PROCEDURE CheckConfig
@country CHAR(1),
@branch INT,
@return INT OUTPUT

AS
BEGIN
	IF EXISTS (SELECT * FROM country
			   WHERE countrycode = @country)
	AND EXISTS (SELECT * FROM branch
				WHERE branchno = @branch)
	BEGIN 
		SELECT 1
	END
	ELSE
	BEGIN
		SELECT 0
	END
END
GO

