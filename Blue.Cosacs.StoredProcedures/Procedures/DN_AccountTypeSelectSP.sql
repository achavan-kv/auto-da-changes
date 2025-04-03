SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountTypeSelectSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountTypeSelectSP]
GO

CREATE PROCEDURE 	dbo.DN_AccountTypeSelectSP
			@accttype varchar(1),
			@countryCode varchar(2),
			@mthsintfree smallint OUT,
			@mthsdeferred smallint OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@mthsintfree = mthsintfree,
			@mthsdeferred = mthsdeferred
	FROM		accttype
	WHERE	accttype = @accttype
	AND		countrycode = @countrycode

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

