SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountDoesExistSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountDoesExistSP]
GO






CREATE PROCEDURE 	dbo.DN_AccountDoesExistSP
			@accountNumber varchar(12),
			@exists int OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	IF EXISTS(SELECT 1 
		    FROM acct 
 		    WHERE acctno = @accountNumber)
	BEGIN
		SET	@exists = 1
	END
	ELSE
	BEGIN
		SET 	@exists = 0
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

