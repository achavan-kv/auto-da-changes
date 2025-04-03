SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dbdatenextdue]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dbdatenextdue]
GO

CREATE procedure dbdatenextdue @arrears money = 0, 
    @instalamount money, 
    @acctno varchar(12) = ' ', 
    @datenextdue DATETIME = '1900-01-01', 
    @nsd integer = 0, 
    @datefirst DATETIME = '1900-01-01', 
    @outstbal money = 0, 
    @datedel DATETIME = '1900-01-01'   
AS    
-- AA- 14/07/03 change for ready finance to exclude instalment.datefirst < 1910
-- SC 25/06-08 changed to make date next due to the upcoming day for that or next month.

declare	@dnd DATETIME, @year SMALLINT,@month SMALLINT
    

    if  substring (@acctno, 4, 1) = '4'
		BEGIN
			SET @dnd = @datedel 
		END
    ELSE
		BEGIN
			SET @dnd = @datefirst; 
		END

	if @datefirst >= getdate()
	begin
		UPDATE agreement 
		SET datenextdue = @dnd 
		WHERE acctno = @acctno; 
		return;
	end


    if @outstbal < = 0 
    BEGIN
        return; 
    END


IF DATEPART(DAY,@dnd) >= DATEPART(DAY,GETDATE()) -- set the date to this month
		SET @dnd = CASE WHEN ISDATE ( CAST(DATEPART(yy,GETDATE()) * 10000 + 
		                                   DATEPART(m,GETDATE()) * 100 +  
		                                   DATEPART(dd, @dnd) AS CHAR(8))
								     ) = 1 --check ISDATE valid if 31-dec but not 31-feb
					  THEN CAST( CAST( (DATEPART(yy,GETDATE()) * 10000 + 
					                    DATEPART(m,GETDATE()) * 100 + 
					                    DATEPART(d, @dnd)) AS CHAR(8)) AS DATETIME)
					  ELSE CAST( CAST( (DATEPART(yy,GETDATE()) * 10000 + 
					                    DATEPART(m,GETDATE()) * 100 + 
					                    01) AS CHAR(8)) AS DATETIME) 
					  END
		
	
	
	IF DATEPART(DAY,@dnd) < DATEPART(DAY,getdate())  -- date next due will be next month
	BEGIN
	
	    SET @year = DATEPART(yy,getdate())
	    SET @month = DATEPART(m,getdate())
	   
	    IF @month= 12 
	    
		SET @year = @year + 1 -- due date going to be next year 70427
	
	
			
		SET @dnd = CASE WHEN ISDATE(CAST( (@year * 10000 + 
		                                   DATEPART(m,DATEADD(m,1,getdate())) * 100 + 
		                                   DATEPART(d, @dnd)) AS CHAR(8))) = 1 --check ISDATE valid if 31-dec but not 31-feb
					  THEN CAST( CAST( (@year * 10000 + 
					       DATEPART(m,DATEADD(m,1,getdate())) * 100 + 
					       datepart(day, @dnd)) AS CHAR(8)) AS DATETIME)
					  ELSE CAST( CAST( (@year * 10000 + 
					       DATEPART(m,DATEADD(m,2,getdate())) * 100 + 
					       01) AS CHAR(8)) AS DATETIME) 
					  END 
						
	END 
	
				
	IF @dnd != @datenextdue
	BEGIN
		UPDATE agreement 
		SET datenextdue = @dnd 
		WHERE acctno = @acctno; 
	END 
	
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
