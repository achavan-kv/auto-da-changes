SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetCodesOnSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetCodesOnSP]
GO






CREATE PROCEDURE 	dbo.DN_AccountGetCodesOnSP
			@acctNo varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@acctNo = acctno 
	FROM		acct
	WHERE	acctno = @acctNo

	IF(@@rowcount > 0)
	BEGIN
		SELECT	A.code as Code,
				A.datecoded as 'Date Added',
				A.empeenocode as 'Added By',
				C.codedescript as Description
		FROM		acctcode A, code C
		WHERE	A.acctno = @acctNo
		AND		A.code = C.code
		AND		C.category in ('AC1', 'AC2')
      AND A.datedeleted is null 
	END
	ELSE
	BEGIN
		SET	@return = -1
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

