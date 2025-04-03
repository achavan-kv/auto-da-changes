if exists (select * from sysobjects  where name =  'dn_FACT2000controlfiledetails' )
drop procedure dn_FACT2000controlfiledetails
go

CREATE PROCEDURE	dn_FACT2000controlfiledetails 
					@configurationname varchar (32),
					@ControlFile varchar(512) OUTPUT, 
					@return int OUTPUT
AS
	DECLARE @effectivedate varchar(12), @fullproduct char(1), @excludezerostock char(1),
			@processEOD char(1), @processEOW char(1), @processEOP char(1),
			@processCINT char(1), @processfint  char(1), @doprocessfint varchar(10),
			@datesaved datetime, @datestart datetime, @datetoday datetime,
			@tmpdate datetime

	SET NOCOUNT ON

	SET @return =0 --UAT 332
	SET @datetoday = CONVERT(DATETIME,CONVERT(VARCHAR(10), GETDATE(), 103), 103)

	SELECT	@doprocessfint = value 
	FROM	countrymaintenance 
	WHERE	name = 'Export Financial Data to FACT'
	
	IF @doprocessfint='True'
		SET @processfint='Y'
	ELSE
		SET @processfint='N'

	SELECT	@effectivedate = effectivedate,
			@fullproduct = fullproduct,
			@excludezerostock = excludezerostock,
			@processEOD = processEOD,
			@processEOW = processEOW,
			@processEOP = processEOP,
			@processCINT = processCINT,
			@datesaved = datesaved
	FROM	FACTAUTO

	
	
    SELECT	@datestart = MAX(datestart)
    FROM	interfacecontrol 
    WHERE	interface = 'COS FACT'
    AND		result = 'P'
    
    SET @datestart = CONVERT(DATETIME,CONVERT(VARCHAR(10), @datestart, 103), 103)
    SET @tmpdate = CONVERT(DATETIME, @effectivedate, 103)
    
    IF(@datesaved < @datestart AND @tmpdate != @datetoday)
    BEGIN
		IF(DATEPART(HOUR,GETDATE()) < 8)
			SET @effectivedate = CONVERT(VARCHAR(10), DATEADD(DAY, -1, GETDATE()), 103)	
		ELSE
			SET @effectivedate = CONVERT(VARCHAR(10), GETDATE(), 103)	
	
		-- need to set defaults. 
		SET @fullproduct='N'
		SET @excludezerostock='N'
		SET @processEOD ='Y'
		SET @processEOW = 'N'
		SET @processEOP='N'
		SET @processCINT ='Y'
		SET @effectivedate = replace(CONVERT(VARCHAR,DATEADD(HOUR,-4,GETDATE()),105),'-','/')   
	END

	IF @processEOP ='Y' -- 68960 if doing end of period then end of day and end of week are true.
	BEGIN
		SET @processCINT ='Y'
		SET @processEOW = 'Y'
		SET @processEOD = 'Y'
	END

	SET @ControlFile= @effectivedate + ',Y,' + @processCINT + ',' + @fullproduct + ',' + @excludezerostock + ',' + 	@processfint + ',' +  @processEOW + ',' + @processEOP 

--select @ControlFile
return @return

GO

