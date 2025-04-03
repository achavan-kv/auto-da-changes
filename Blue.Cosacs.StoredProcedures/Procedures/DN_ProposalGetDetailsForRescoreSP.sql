SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalGetDetailsForRescoreSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalGetDetailsForRescoreSP]
GO



CREATE PROCEDURE 	dbo.DN_ProposalGetDetailsForRescoreSP
			@acctno varchar(12),
			@custid varchar(20),
			@datelastscored datetime OUT,
			@highstatus varchar(2) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@highstatus = MAX(ISNULL(a.highststatus, '0'))
	FROM	acct a, custacct c
	WHERE	c.custid = @custid
	AND		a.acctno = c.acctno
	AND		a.acctno != @acctno

	SELECT	@datelastscored = ISNULL(datelastscored, GETDATE())
	FROM	customer
	WHERE	custid = @custid

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

