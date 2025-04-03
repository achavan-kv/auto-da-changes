SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalGetStatusSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalGetStatusSP]
GO


/****** Object:  StoredProcedure [dbo].[DN_ProposalGetStatusSP]    Script Date: 11/05/2007 12:14:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE 	[dbo].[DN_ProposalGetStatusSP]
			@acctno varchar(12),
			@S1 varchar(1) OUT,
			@S2 varchar(1) OUT,
			@DC varchar(1) OUT,
			@AD varchar(1) OUT,
			@UW varchar(1) OUT,
			@appstatus varchar(4) OUT,
			@manualrefer varchar(4) OUT,
			@adreqd varchar(1) OUT,			
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE 	@checktype varchar(5)
	DECLARE	@datecleared datetime

	SET		@datecleared = null

	--verify whether each check type has a date 
	SELECT	@checktype = checktype,
			@datecleared = datecleared
	FROM		proposalflag PF, proposal P
	WHERE	P.acctno = @acctno
	AND		p.acctno = pf.acctno
	AND		checktype = 'S1'

	IF(@datecleared is null)
	BEGIN
		SET	@S1 = 'N'
	END
	ELSE
	BEGIN
		SET	@S1 = 'Y'
		SET	@datecleared = null
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

	SELECT	@checktype = checktype,
			@datecleared = datecleared
	FROM		proposalflag PF, proposal P
	WHERE	P.acctno = @acctno
	AND		p.acctno = pf.acctno
	AND		checktype = 'S2'

	IF(@datecleared is null)
	BEGIN
		SET	@S2 = 'N'
	END
	ELSE
	BEGIN
		SET	@S2 = 'Y'
		SET	@datecleared = null
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

	SELECT	@checktype = checktype,
			@datecleared = datecleared
	FROM		proposalflag PF, proposal P
	WHERE	P.acctno = @acctno
	AND		p.acctno = pf.acctno
	AND		checktype = 'DC'

	IF(@datecleared is null)
	BEGIN
		SET	@DC = 'N'
	END
	ELSE
	BEGIN
		SET	@DC = 'Y'
		SET	@datecleared = null
	END

	SELECT	@checktype = checktype,
			@datecleared = datecleared
	FROM		proposalflag PF, proposal P
	WHERE	P.acctno = @acctno
	AND		p.acctno = pf.acctno
	AND		checktype = 'AD'

	IF(@datecleared is null)
	BEGIN
		SET	@AD = 'N'
	END
	ELSE
	BEGIN
		SET	@AD = 'Y'
		SET	@datecleared = null
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

	SELECT	@checktype = checktype,
			@datecleared = datecleared
	FROM		proposalflag PF, proposal P
	WHERE	P.acctno = @acctno
	AND		p.acctno = pf.acctno
	AND		checktype = 'R'

	IF(@datecleared is null)
	BEGIN
		SET	@UW = 'N'
	END
	ELSE
	BEGIN
		SET	@UW = 'Y'
		SET	@datecleared = null
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

	SELECT	@appstatus = appstatus,
			@manualrefer = manualrefer,
			@adreqd = adreqd
	FROM		proposal P, propresult PR
	WHERE	P.acctno = @acctno
	AND		P.acctno = PR.acctno
		

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END






