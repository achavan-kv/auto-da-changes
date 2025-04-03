SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetApplicationTypesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetApplicationTypesSP]
GO





CREATE PROCEDURE 	dbo.DN_GetApplicationTypesSP
			@custid varchar(20),
			@acctno varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	/* JJ 	change this so that we return the same options regardless of 
		what values are contained in custacct	*/
	
	SELECT	code,
			codedescript
	FROM		code
	WHERE	category = 'LCT'
	AND		code in ('J', 'S')
	AND		statusflag = 'L'	

	/*
	IF(isnull(@acctno,'')=N'')
	BEGIN
		SELECT	C.code,
				C.codedescript
		FROM		code C INNER JOIN customerlinks CL
		ON		C.code = CL.relationship
		WHERE	C.category = 'LCT'
		AND		C.statusflag = 'L'
		AND		CL.holder = @custid
		AND		CL.relationship in ('S','J')
	END
	ELSE
	BEGIN
		SELECT	C.code,
				C.codedescript
		FROM		code C INNER JOIN custacct CA
		ON		C.code = CA.hldorjnt
		WHERE	C.category = 'LCT'
		AND		C.statusflag = 'L'
		AND		CA.acctno = @acctno
		AND		CA.hldorjnt != 'H'	
		AND		CA.hldorjnt in ('S','J')
	END	
	*/	
	

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

