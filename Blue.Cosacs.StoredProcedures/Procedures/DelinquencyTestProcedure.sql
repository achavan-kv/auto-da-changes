IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'CheckDelinquency')
DROP PROCEDURE CheckDelinquency
GO 
CREATE PROCEDURE CheckDelinquency @fieldname VARCHAR(44), @value VARCHAR(4), @not BIT =0, @comparitor VARCHAR(3) = ' = '
AS 

DECLARE @statement sqltext,@rowcount INT 
SET NOCOUNT ON 
SET @statement = ' SELECT top 100 * FROM Delinquency WHERE '  + @fieldname + @comparitor  + @value
EXEC sp_executesql @statement
SET @rowcount = @@ROWCOUNT
IF @not = 0 
BEGIN
	IF @rowcount = 0
	BEGIN
		PRINT 'fail ' + @fieldname + ' No values for ' + @value
	END
	ELSE 
	BEGIN
		PRINT 'success ' + @fieldname + ' ' +@value + ' had ' + CONVERT(VARCHAR,@rowcount) 
	END
END
IF @not = 1 
BEGIN
	IF @rowcount = 0
	BEGIN
		PRINT 'success ' + @fieldname + ' No values for ' + @value
	END
	ELSE 
	BEGIN
		PRINT 'fail ' + @fieldname + ' ' +@value + ' had ' + CONVERT(VARCHAR,@rowcount) 
	END
END

GO
IF EXISTS (SELECT * FROM sysobjects  WHERE name= 'DelinquencyTest')
DROP PROCEDURE DelinquencyTest
GO 
CREATE Procedure DelinquencyTest AS  
EXEC CheckDelinquency 	@fieldname = 'numacctsarrears', @value = '-1' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'numactiveaccts', @value = '-1' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'numactiveaccts', @value = 'null',@not=1 -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'numacctsarrears', @value = 'null',@not=1 -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'arrearstotalPercent3months', @value = '-3' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'arrearstotalPercent3months', @value = '-2' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'arrearstotalPercent3months', @value = '-1' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'arrearstotalPercent9months', @value = '-3' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'arrearstotalPercent9months', @value = '-2' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'arrearstotalPercent9months', @value = '-1' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'balanceTotalPercent3months', @value = '-3' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'balanceTotalPercent3months', @value = '0' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'balanceTotalPercent3months', @value = '-1',@not=1 -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'balanceTotalPercent3months', @value = 'NULL',@not=1 -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'balanceTotalPercent9months', @value = '0' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'balanceTotalPercent9months', @value = '-1',@not=1 -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'balanceTotalPercent9months', @value = '-3' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'worstcurrentstatusChangelast9Months', @value = '-2' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'worstcurrentstatusChangelast9Months', @value = '0' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'worstcurrentstatusChangelast9Months', @value = '-3' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'worstcurrentstatusChangelast9Months', @value = '2',@comparitor =' > ' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'worstcurrentstatusChangelast9Months', @value = '30',@comparitor =' > ',@not =1 -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'worstcurrentstatusChangelast12Months', @value = '-2' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'worstcurrentstatusChangelast12Months', @value = '0' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'worstcurrentstatusChangelast12Months', @value = '-3' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'worstcurrentstatusChangelast12Months', @value = '2',@comparitor =' > ' -- VARCHAR(4)
EXEC CheckDelinquency 	@fieldname = 'worstcurrentstatusChangelast12Months', @value = '30',@comparitor =' > ',@not =1 -- VARCHAR(4)
GO 
-- EXEC DelinquencyTest Uncomment to Run
