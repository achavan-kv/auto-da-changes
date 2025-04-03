SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_JournalEnquiryGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_JournalEnquiryGetSP]
GO

CREATE PROCEDURE 	dbo.DN_JournalEnquiryGetSP
			@datefirst datetime,
			@datelast datetime,
			@firstrefno int,
			@lastrefno int,
			@empeeno int,
			@branch int,
			@combination int,
			@return int OUTPUT

AS

	DECLARE	@select varchar(500), 
			@unionselect varchar(500), 
			@where varchar(500),
			@final_statement SQLText

	SET 		@return = 0			--initialise return code

	CREATE TABLE #journal( BranchNo smallint, 
			   	  EmpeeNo int, 
				  DateTrans datetime, 
				  TransRefNo int, 
				  Acctno varchar(30), 
				  TransTypeCode varchar(3), 
				  TransValue money,
				  Paymethod smallint,
				  CodeDescript varchar(50))

	SET @select =	'INSERT INTO #journal ' + 
			' SELECT BranchNo, EmpeeNo, DateTrans, TransRefNo, Acctno, TransTypeCode, TransValue, ' +
			' Paymethod, ''Not Applicable'' FROM FINTRANS '

	SET @unionselect =	' UNION SELECT BranchNo, EmpeeNo, DateTrans, TransRefNo, RemRefNo, TransTypeCode, ' +
			   	' TransValue, -1, ''Not Applicable'' FROM REMEXPRESS '

	IF @combination = 1
	BEGIN
		SET @where =	' datetrans >= convert(datetime,''' + convert(varchar, @datefirst,113) +
			     	' '',113) and datetrans <= convert(datetime,'' ' + convert(varchar, @datelast,113) +  ' '',113)   '
	END

	IF @combination = 2
	BEGIN
		SET @where =	' transrefno between ' + convert(varchar, @firstrefno) +
			     	' and ' + convert(varchar, @lastrefno)
	END

	IF @combination = 3
	BEGIN
		SET @where =	' empeeno = ' + convert(varchar, @empeeno)
	END

	IF @combination = 4
	BEGIN
		SET @where =	' branchno = ' + convert(varchar, @branch)
	END

	IF @combination = 5
	BEGIN
		SET @where =	' datetrans >= convert(datetime,''' + convert(varchar, @datefirst,113) +
			     	' '',113) and datetrans <= convert(datetime,'' ' + convert(varchar, @datelast,113) +  ' '',113)   ' +
				' and transrefno between ' + convert(varchar, @firstrefno) +
			     	' and ' + convert(varchar, @lastrefno)	
	END

	IF @combination = 6
	BEGIN
		SET @where =	' datetrans >= convert(datetime,''' + convert(varchar, @datefirst,113) +
			     	' '',113) and datetrans <= convert(datetime,'' ' + convert(varchar, @datelast,113) +  ' '',113)   ' +
                     			' and empeeno = ' + convert(varchar, @empeeno)
	END

	IF @combination = 7
	BEGIN
		SET @where =	' datetrans >= convert(datetime,''' + convert(varchar, @datefirst,113) +
			     	' '',113) and datetrans <= convert(datetime,'' ' + convert(varchar, @datelast,113) +  ' '',113)   ' +
                     			' and branchno = ' + convert(varchar, @branch)
	END

	IF @combination = 8
	BEGIN
		SET @where =	' transrefno between ' + convert(varchar, @firstrefno) +
			     	' and ' + convert(varchar, @lastrefno) +
                     			' and empeeno = ' + convert(varchar, @empeeno)
	END

	IF @combination = 9
	BEGIN
		SET @where =	' transrefno between ' + convert(varchar, @firstrefno) +
			     	' and ' + convert(varchar, @lastrefno) +
                     			' and branchno = ' + convert(varchar, @branch)
	END

	IF @combination = 10
	BEGIN
		SET @where =	' datetrans >= convert(datetime,''' + convert(varchar, @datefirst,113) +
			     	' '',113) and datetrans <= convert(datetime,'' ' + convert(varchar, @datelast,113) +  ' '',113)   ' +
                     			' and transrefno between ' + convert(varchar, @firstrefno) +
			     	' and ' + convert(varchar, @lastrefno) +
                     			' and empeeno = ' + convert(varchar, @empeeno)
	END

	IF @combination = 11
	BEGIN
		SET @where =	' datetrans >= convert(datetime,''' + convert(varchar, @datefirst,113) +
			     	' '',113) and datetrans <= convert(datetime,'' ' + convert(varchar, @datelast,113) +  ' '',113)   ' +
                     			' and transrefno between ' + convert(varchar, @firstrefno) +
			     	' and ' + convert(varchar, @lastrefno) +
                     			' and branchno = ' + convert(varchar, @branch)
	END

	IF @combination = 12
	BEGIN
		SET @where =	' datetrans >= convert(datetime,''' + convert(varchar, @datefirst,113) +
			     	' '',113) and datetrans <= convert(datetime,'' ' + convert(varchar, @datelast,113) +  ' '',113)   ' +
		     		' and empeeno = ' + convert(varchar, @empeeno) +
                     			' and branchno = ' + convert(varchar, @branch)
	END

	IF @combination = 13
	BEGIN
		SET @where =	' transrefno between ' + convert(varchar, @firstrefno) +
			     	' and ' + convert(varchar, @lastrefno) +
		     		' and empeeno = ' + convert(varchar, @empeeno) +
                     			' and branchno = ' + convert(varchar, @branch)
	END

	IF @combination = 14
	BEGIN
		SET @where =	' datetrans >= convert(datetime,''' + convert(varchar, @datefirst,113) +
			     	' '',113) and datetrans <= convert(datetime,'' ' + convert(varchar, @datelast,113) +  ' '',113)   ' +
				' and transrefno between ' + convert(varchar, @firstrefno) +
			     	' and ' + convert(varchar, @lastrefno)	+
		     		' and empeeno = ' + convert(varchar, @empeeno) +
                     			' and branchno = ' + convert(varchar, @branch)
	END

	IF @combination = 15
	BEGIN
		SET @where =	' empeeno = ' + convert(varchar, @empeeno) +	
			    	' and branchno = ' + convert(varchar, @branch)
	END

	SET  @final_statement = @select + 'WHERE ' + ' ' +
				@where + ' ' +
				@unionselect + 'WHERE ' +
				@where

	EXECUTE sp_executesql @final_statement

	UPDATE #journal
	SET CodeDescript = c.codedescript
	FROM code c
	WHERE #journal.Paymethod = c.code
	AND c.category = 'FPM'
	AND #journal.Paymethod > 0

	SELECT	Branchno,
	    		EmpeeNo,
	    		DateTrans,
	    		TransRefNo,
	    		isnull(AcctNo,'') as 'AcctNo',	
	    		TransTypeCode,
	    		TransValue,
			CodeDescript
	FROM    	#journal

	SELECT	SUM(TransValue) as TransValue,
	    		TransTypeCode
	FROM    	#journal
	GROUP BY	TransTypeCode

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

