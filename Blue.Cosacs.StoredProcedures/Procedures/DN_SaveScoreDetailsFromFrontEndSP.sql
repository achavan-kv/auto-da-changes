SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SaveScoreDetailsFromFrontEndSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SaveScoreDetailsFromFrontEndSP]
GO

CREATE PROCEDURE 	 	dbo.DN_SaveScoreDetailsFromFrontEndSP
			@custid varchar(20),
			@acctno varchar(12),
			@datescored datetime,
			@operandname varchar(4000),
			@operandvalue varchar(100),
			@points varchar(100),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	SET NOCOUNT ON -- dont want communicating back till end
    DECLARE @name varchar(150)
    DECLARE @value varchar(50)
    DECLARE @point INT
    
    WHILE (CHARINDEX('|',@operandname) > 0)
    BEGIN
        -- so Charindex will get the name  from operand name e.g @operandname='Age: 31y - 35y|Time at current bank:4y-6y11m|Marital Status: Other|'
        -- first charindex should get Age: 31y - 35y
        SET @name = SUBSTRING(@operandname,0,CHARINDEX('|',@operandname))
        -- e.g value @operandvalue=N'32|56|S|2|22|1|2|Y|46|FW|' --first should should get 32
        SET @value = SUBSTRING(@operandvalue,0,CHARINDEX('|',@operandvalue))
        -- @points=N'15|19|4|25|21|42|46|23|9|10|' so should get 15 to begin with
        SET @point = CONVERT(INT,SUBSTRING(@points,0,CHARINDEX('|',@points)))


        INSERT INTO ScoringDetails 
		(custid, acctno, datescored, operandname, operandvalue, points)
	    VALUES (@custid, @acctno, @datescored, @name, @value, @point)


        -- now remove the previously calculated bits from the left of the string but only if length>0 
        if (DATALENGTH(@operandname) - DATALENGTH(@name) - 1) >0 
        AND	(DATALENGTH(@operandvalue) - DATALENGTH(@value) - 1) >0 AND
         (LEN(@points) - LEN(@point) - 1) >0
        BEGIN
            SET @operandname = RIGHT(@operandname,DATALENGTH(@operandname) - DATALENGTH(@name) - 1)
            SET @operandvalue = RIGHT(@operandvalue,DATALENGTH(@operandvalue) - DATALENGTH(@value) - 1)
            SET @points = RIGHT(@points,LEN(@points) - LEN(@point) - 1)
         END
         ELSE
           BREAK -- have reached the end of this        
    END
    
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

