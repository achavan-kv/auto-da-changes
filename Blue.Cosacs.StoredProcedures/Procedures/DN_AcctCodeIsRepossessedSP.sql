SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AcctCodeIsRepossessedSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AcctCodeIsRepossessedSP]
GO

CREATE PROCEDURE dbo.DN_AcctCodeIsRepossessedSP
		 @acctno varchar(12),
		 @repo smallint OUT,
   		 @return INT OUTPUT
 
AS
 	SET  @return = 0 --initialise return code
	SET  @repo = 0

	SELECT	@repo = count(*)
	FROM		acctcode
	WHERE	acctno = @acctno
	AND		code in ('R','FREP','PREP')
   AND datedeleted is null

 	IF (@@error != 0)
 	BEGIN
  		SET @return = @@error
 	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

