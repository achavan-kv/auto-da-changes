SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierTotalsBreakdownGetForEmployeeSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierTotalsBreakdownGetForEmployeeSP]
GO

CREATE PROCEDURE 	dbo.DN_CashierTotalsBreakdownGetForEmployeeSP
			@employeeno int,
			@datefrom datetime,
			@dateto datetime,
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code

	SET 	@datefrom  =convert (datetime,convert (varchar(20),@datefrom,120))
	SET 	@dateto  =convert (datetime,convert (varchar(20),@dateto,120))

	SELECT 	code, 
			category,
			codedescript, 
			convert(money,0) AS systemtotal,
			convert(money,0) AS usertotal,
			convert(money,0) AS difference,
			convert(varchar(4),'') AS paymethod,
			convert(varchar(100),'') AS reason,
			convert(money,0) AS deposit
	INTO		#totals
	FROM 		code
	WHERE 	category = 'FPM'
	AND		code != 0
	
	UPDATE	#totals
	SET		systemtotal = CTB.systemtotal,
			usertotal = CTB.usertotal,
			paymethod = CTB.paymethod,
			difference = CTB.difference,
			reason = CTB.reason,
			deposit = CTB.deposit
	FROM		CashierTotals CT INNER JOIN CashierTotalsBreakdown CTB 
	ON		CT.id = CTB.cashiertotalid INNER JOIN #totals t
	ON		CTB.paymethod = t.code 
	AND		t.category = 'FPM'
	WHERE	CT.empeeno = @employeeno
	AND		CT.datefrom = @datefrom
	AND		CT.dateto = @dateto
	
	SELECT	codedescript,
			systemtotal,
			usertotal,
			paymethod,
			difference,
			reason,
			deposit
	FROM 		#totals
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

