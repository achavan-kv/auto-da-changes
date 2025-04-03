IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'DelinquencyBalArrsDynamic')
DROP PROCEDURE DelinquencyBalArrsDynamic 
GO 
CREATE PROCEDURE dbo.DelinquencyBalArrsDynamic @column VARCHAR(64),@period CHAR(1)
AS
declare @dbcolumn varchar(24) 
if @column = 'balance' 
 set @dbcolumn = 'outstbal'
else
  set @dbcolumn = @column
DECLARE @statement sqltext
SET @statement =  -- First update totals
 	' UPDATE #BehaveDetails SET ' + @column + 'tot' + CONVERT(VARCHAR,@period)
 	 + ' = ' +
	' ISNULL((SELECT SUM( ' + @dbcolumn + ') FROM accountMonths2 m , custacct ca,#AccountDates d 
	WHERE ca.hldorjnt= ''H'' AND m.acctno= ca.acctno ' + 
	' AND m.currentMonth =  d.MONTH' + @period + 'date ' + 
	' AND ca.custid = #behavedetails.custid and outstbal <>0),0) '
 
EXECUTE sp_executesql @statement 
 PRINT @statement
-- set to default for null...	
SET @statement = '	UPDATE #BehaveDetails SET ' + @column 
+ 'totalPercent' + CONVERT(VARCHAR,@period) + 'Months = -3 '
-- + ' WHERE ' + @column + 'totalPercent' + CONVERT(VARCHAR,@period) + 'Months ' + ' IS NULL'	
EXECUTE sp_executesql @statement 
IF @@ERROR !=0
	PRINT @statement
SET @statement='UPDATE #BehaveDetails ' +
	' SET ' + @column + 'totalPercent' + @Period + 'Months = ' + 
	' current' + @column + 'total/' + @column + 'tot' + @period + '* 100 ' +
	' WHERE ISNULL(' + @column + 'tot'+ @period + ',0) <>0'
EXECUTE sp_executesql @statement 

-- set to -3 if no accounts qualify
BEGIN 
	SET @statement='UPDATE #BehaveDetails ' +
		' SET ' + @column + 'totalPercent' + @Period + 'Months = -3'  +
		' WHERE ISNULL(balancetot'+ @period + ',0) <=0' 
		 
	EXECUTE sp_executesql @statement 
	IF @@ERROR !=0
		PRINT @statement
END 



IF @@ERROR !=0
	PRINT @statement
IF @column = 'arrears' -- set to 2 if in advance and previously in advance
BEGIN 
	SET @statement='UPDATE #BehaveDetails ' +
		' SET ' + @column + 'totalPercent' + @Period + 'Months = -2'  +
		' WHERE ISNULL(' + @column + 'tot'+ @period + ',0) <=0' +
		' AND  current' + @column + 'total < 0' 
		+ ' AND ISNULL(balancetot'+ @period + ',0) >0'  -- but only where had accounts with balance >0
	EXECUTE sp_executesql @statement 
	IF @@ERROR !=0
		PRINT @statement
END 

IF @column = 'Arrears' -- SETTING TO -1 if currently in arrears but not previously
BEGIN
	SET @statement='UPDATE #BehaveDetails ' +
	' SET ' + @column + 'totalPercent' + @Period + 'Months =-1 ' + 
	' WHERE ISNULL(' + @column + 'tot'+ @period + ',0) <=0 AND' +
	' current' + @column + 'total>0' 
	+ ' AND ISNULL(balancetot'+ @period + ',0) >0'
	EXECUTE sp_executesql @statement
	IF @@ERROR !=0
		PRINT @statement
	 
END 


GO 