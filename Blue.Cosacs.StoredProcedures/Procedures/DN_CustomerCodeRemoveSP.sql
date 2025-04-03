SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerCodeRemoveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerCodeRemoveSP]
GO





CREATE PROCEDURE 	dbo.DN_CustomerCodeRemoveSP
			@custid varchar(20),
			@code varchar(4),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DELETE
	FROM		custcatcode
	WHERE	custid = @custid
	AND		code = @code

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

