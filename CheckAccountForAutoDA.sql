IF OBJECT_ID('dbo.CheckAccountForAutoDA', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.CheckAccountForAutoDA;
END
GO

CREATE PROCEDURE CheckAccountForAutoDA    
    @acctno VARCHAR(12), 
    @IsAutoDA BIT OUTPUT
AS
BEGIN    
    DECLARE @deposit MONEY;    
    DECLARE @totalPaid MONEY;    
    DECLARE @accttype CHAR;    
    SET @totalPaid = 0;    

	SELECT @accttype = accttype    
    FROM acct    
    WHERE acctno = @acctno;    

    SELECT @deposit = deposit
    FROM agreement    
    WHERE acctno = @acctno;    

    SELECT @totalPaid = SUM(-transvalue)    
    FROM fintrans    
    WHERE acctno = @acctno AND transtypecode = 'PAY';    

	IF (@accttype = 'C')
	BEGIN
        SET @IsAutoDA = 0;    
	END
    ELSE IF (@deposit > 0 AND @totalPaid >= @deposit)    
    BEGIN        
        SET @IsAutoDA = 1;     
    END    
    ELSE IF (@deposit IS NULL OR @deposit = 0)    
    BEGIN        
        SET @IsAutoDA = 1;     
    END    
    ELSE    
    BEGIN        
        SET @IsAutoDA = 0;    
    END 
END
GO
