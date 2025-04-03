SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_PostCodeLookUpSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_PostCodeLookUpSP]
GO





CREATE PROCEDURE 	dbo.DN_PostCodeLookUpSP
			@postCode varchar(6),
			@addType varchar(1) OUT,
			@buildNo varchar(7) OUT,
			@bkey varchar(6) OUT,
			@bname varchar(45) OUT,
			@sname varchar(32) OUT,
			@country varchar(20) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@country = countryname
	FROM		country

	SELECT	@addType = P.addtype,
			@buildNo = P.buildno,
			@bkey = P.bkey,
			@bname = ISNULL(B.bname,''),
			@sname = ISNULL(S.sname,'')			
	FROM		pocode P LEFT OUTER  JOIN
			building B ON P.bkey = B.bkey LEFT OUTER  JOIN
			streets S ON S.skey = P.skey
	WHERE	P.cuspocode = @postCode


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

