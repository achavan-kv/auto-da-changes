SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemContractUniqueSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemContractUniqueSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemContractUniqueSP
			@contract varchar(10),
			@acctno varchar(12),
			@agreementno int,
			@unique smallint OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	DECLARE 	@custid varchar(20)

	SELECT	@custid = custid
	FROM		custacct 
	WHERE	acctno = @acctno 
	AND		hldorjnt = 'H'

	IF(@custid = 'PAID & TAKEN')
	BEGIN
		SELECT	@unique = count(*)
		FROM		lineitem
		WHERE	contractno = @contract
	END
	ELSE
	BEGIN
		SELECT	@unique = count(*)
		FROM		lineitem
		WHERE	contractno = @contract
		AND		acctno != @acctno
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

