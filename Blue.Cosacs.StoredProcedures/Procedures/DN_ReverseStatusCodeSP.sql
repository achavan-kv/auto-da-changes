SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ReverseStatusCodeSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ReverseStatusCodeSP]
GO

CREATE PROCEDURE 	dbo.DN_ReverseStatusCodeSP
			@acctno varchar(12),
			@status char(1),
			@user int,
			@return int OUTPUT

AS

	DECLARE	@currstaus char(1)
	SET 		@return = 0			--initialise return code

	SELECT	@currstaus = currstatus
	FROM		acct
	WHERE	acctno = @acctno

	IF(@currstaus = @status)
	BEGIN
		SELECT TOP 1 	@currstaus = statuscode
		FROM		status
		WHERE	acctno = @acctno
		AND		statuscode != @status
		ORDER BY	datestatchge desc

		UPDATE	acct
		SET		currstatus = @currstaus,
				lastupdatedby = @user
		WHERE	acctno = @acctno
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

