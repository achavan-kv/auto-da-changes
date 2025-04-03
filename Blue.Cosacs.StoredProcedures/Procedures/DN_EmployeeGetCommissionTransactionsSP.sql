SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EmployeeGetCommissionTransactionsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EmployeeGetCommissionTransactionsSP]
GO

CREATE PROCEDURE 	dbo.DN_EmployeeGetCommissionTransactionsSP
			@empeeno int,
			--@type char(1),
			@return int OUTPUT

AS
/*#############################################################################
Procedure:  DN_EmployeeGetCommissionTransactionsSP

Script filename: DN_EmployeeGetCommissionTransactionsSP.PRC

Description: Returns all the required data for the Calculate Bailiff Commission screen


Created by: ?
Created date: ?
Modified by: Jez Hemans
Modified date: 28/07/2008
Modified Description: UAT 499 To improve performance all data brought back in one go

#############################################################################*/
	DECLARE	@arrearsonpay char(1)

	SET 	@return = 0			--initialise return code
	
	SELECT	@arrearsonpay = arrearsonpay
	FROM	country
	
	--IF(@type = 'P' OR @type = 'H')
	--BEGIN
		SELECT	b.empeeno,
				b.transrefno,
				b.acctno,
				b.datetrans,
				b.transvalue,
				b.chequecolln,
				b.status,
	 			CONVERT(VARCHAR(4), '') as code,
				CONVERT(FLOAT,0) as amtcommpaidon,
				CONVERT(FLOAT,0) as actionvalue
		INTO	#payable
		FROM    bailiffcommn b
		WHERE	b.empeeno = @empeeno
		AND	    (b.status = 'P' OR b.status = 'H' OR b.status = '')
		AND	    b.datetrans > CONVERT(DATETIME, '01-jan-1900',105)
	
		-- UAT 188 (3.5.2.1) DSR.
		-- This update used to be an outer join with the temp table creation above,
		-- but this could sometimes result in duplication from bailiffcommn.
		-- The join to bailaction has been refined as well, to join on exact
		-- datetime (as is now stored on both records) and to only pick up
		-- PAY and REP codes.
		UPDATE	#payable
		SET		code = ISNULL(a.code, ''),
				amtcommpaidon = ISNULL(a.amtcommpaidon, 0),
				actionvalue = ISNULL(a.actionvalue, 0) 
		FROM	bailaction a
		WHERE	a.empeeno = @empeeno 
		AND     a.acctno = #payable.acctno
		AND		a.dateadded = #payable.datetrans
		AND	    a.dateadded > CONVERT(DATETIME, '01-jan-1900',105)
		AND     a.Code IN ('PAY', 'REP', 'COM')

		IF(@arrearsonpay = 'Y')
		BEGIN
			UPDATE	#payable
			SET	actionvalue = amtcommpaidon
			WHERE	code = 'PAY'
		END
	
		SELECT	empeeno,
			transrefno,
			acctno,
			datetrans,
			transvalue,
			chequecolln,
			status,
			code,
			amtcommpaidon,
			actionvalue
		FROM	#payable
		WHERE status = 'P'
		
		SELECT	empeeno,
			transrefno,
			acctno,
			datetrans,
			transvalue,
			chequecolln,
			status,
			code,
			amtcommpaidon,
			actionvalue
		FROM	#payable
		WHERE status = 'H'
	--END 

	--IP - 17/01/2008 - Livewire(69492) - Retrieve deleted bailiff commissions
	--IF(@type = 'D')
	--BEGIN
		SELECT	b.empeeno,
				b.transrefno,
				b.acctno,
				b.datetrans,
				b.transvalue,
				b.chequecolln,
				b.status,
	 			CONVERT(VARCHAR(4), '') as code,
				CONVERT(FLOAT,0) as amtcommpaidon,
				CONVERT(FLOAT,0) as actionvalue
		INTO	#deleted
		FROM    BailiffCommnDeleted b
		WHERE	b.empeeno = @empeeno
		--AND	    (b.status = @type OR b.status = '')
		AND	    b.datetrans > CONVERT(DATETIME, '01-jan-1900',105)
	
		UPDATE	#deleted
		SET		code = ISNULL(a.code, ''),
				amtcommpaidon = ISNULL(a.amtcommpaidon, 0),
				actionvalue = ISNULL(a.actionvalue, 0) 
		FROM	bailaction a
		WHERE	a.acctno = #deleted.acctno
		AND		a.dateadded = #deleted.datetrans
		AND	    a.dateadded > CONVERT(DATETIME, '01-jan-1900',105)
		AND     a.Code IN ('PAY', 'REP', 'COM')
	
		IF(@arrearsonpay = 'Y')
		BEGIN
			UPDATE	#deleted
			SET	actionvalue = amtcommpaidon
			WHERE	code = 'PAY'
		END
	
		SELECT	empeeno,
			transrefno,
			acctno,
			datetrans,
			transvalue,
			chequecolln,
			status,
			code,
			amtcommpaidon,
			actionvalue
		FROM	#deleted
	--END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO	
